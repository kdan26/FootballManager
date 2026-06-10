using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly ApplicationDbContext _db;

        public CalendarService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ─── Events cho FullCalendar ───
        public async Task<List<CalendarEventDto>> GetEventsAsync(
            int? teamId, DateTime start, DateTime end)
        {
            var events = new List<CalendarEventDto>();

            // 1. Buổi tập
            var sessionQuery = _db.TrainingSessions
                .Include(s => s.Team)
                .Where(s => s.ScheduledAt >= start && s.ScheduledAt <= end
                         && s.Status != TrainingSessionStatus.Cancelled);

            if (teamId.HasValue)
                sessionQuery = sessionQuery.Where(s => s.TeamId == teamId.Value);

            var sessions = await sessionQuery.ToListAsync();

            foreach (var s in sessions)
            {
                string color = s.Status switch
                {
                    TrainingSessionStatus.Completed => "#198754",  // green
                    TrainingSessionStatus.Scheduled  => "#0d6efd", // blue
                    _ => "#6c757d"
                };

                events.Add(new CalendarEventDto
                {
                    Id          = s.Id,
                    Title       = $"[Tập] {s.Title} — {s.Team.Name}",
                    Start       = s.ScheduledAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    End         = s.ScheduledAt.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ss"),
                    Color       = color,
                    Url         = $"/TrainingSession/Details/{s.Id}",
                    Type        = "training",
                    StatusLabel = s.Status switch
                    {
                        TrainingSessionStatus.Scheduled => "Sắp diễn ra",
                        TrainingSessionStatus.Completed => "Hoàn thành",
                        _ => ""
                    }
                });
            }

            // 2. Trận đấu
            var matchQuery = _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.MatchDate >= start && m.MatchDate <= end
                         && m.Status != MatchStatus.Cancelled);

            if (teamId.HasValue)
                matchQuery = matchQuery.Where(m =>
                    m.HomeTeamId == teamId.Value || m.AwayTeamId == teamId.Value);

            var matches = await matchQuery.ToListAsync();

            foreach (var m in matches)
            {
                string color = m.Status switch
                {
                    MatchStatus.Completed => "#fd7e14",  // orange
                    MatchStatus.Scheduled => "#dc3545",  // red
                    _ => "#6c757d"
                };

                events.Add(new CalendarEventDto
                {
                    Id          = m.Id,
                    Title       = $"[Trận] {m.HomeTeam.Name} vs {m.AwayTeam.Name}",
                    Start       = m.MatchDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    End         = m.MatchDate.AddHours(2).ToString("yyyy-MM-ddTHH:mm:ss"),
                    Color       = color,
                    Url         = $"/Match/Details/{m.Id}",
                    Type        = "match",
                    StatusLabel = m.Status switch
                    {
                        MatchStatus.Scheduled => "Sắp diễn ra",
                        MatchStatus.Completed => "Đã kết thúc",
                        _ => ""
                    }
                });
            }

            return events;
        }

        // ─── Trang điểm danh ───
        public async Task<TrainingAttendanceViewModel?> GetAttendanceAsync(int sessionId)
        {
            var session = await _db.TrainingSessions
                .Include(s => s.Team)
                .Include(s => s.Attendances).ThenInclude(a => a.Player)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null) return null;

            // Lấy tất cả cầu thủ active của đội
            var players = await _db.Players
                .Where(p => p.TeamId == session.TeamId && p.IsActive)
                .OrderBy(p => p.JerseyNumber)
                .ToListAsync();

            var items = players.Select(p =>
            {
                var existing = session.Attendances.FirstOrDefault(a => a.PlayerId == p.Id);
                return new TrainingAttendanceItem
                {
                    PlayerId      = p.Id,
                    FullName      = p.FullName,
                    JerseyNumber  = p.JerseyNumber,
                    PositionLabel = ViewModelHelpers.GetPositionShort(p.Position),
                    Status       = existing?.Status ?? TrainingAttendanceStatus.Present,
                    LateMinutes  = existing?.LateMinutes,
                    Note         = existing?.Note,
                    CheckInTime  = existing?.CheckInTime
                };
            }).ToList();

            return new TrainingAttendanceViewModel
            {
                SessionId     = session.Id,
                SessionTitle  = session.Title,
                ScheduledAt   = session.ScheduledAt,
                Location      = session.Location,
                TeamName      = session.Team.Name,
                TeamId        = session.TeamId,
                SessionStatus = session.Status,
                Players       = items
            };
        }

        // ─── Lưu điểm danh ───
        public async Task SaveAttendanceAsync(SaveTrainingAttendanceViewModel model)
        {
            // Xóa bản ghi cũ của buổi này
            var existing = await _db.TrainingAttendances
                .Where(a => a.TrainingSessionId == model.SessionId)
                .ToListAsync();
            _db.TrainingAttendances.RemoveRange(existing);

            // Thêm mới
            foreach (var p in model.Players)
            {
                _db.TrainingAttendances.Add(new TrainingAttendance
                {
                    TrainingSessionId = model.SessionId,
                    PlayerId          = p.PlayerId,
                    Status            = p.Status,
                    LateMinutes       = p.Status == TrainingAttendanceStatus.Late
                                        ? p.LateMinutes : null,
                    Note              = p.Note,
                    CheckInTime       = p.CheckInTime,
                    RecordedAt        = DateTime.Now
                });
            }

            await _db.SaveChangesAsync();
        }
    }
}
