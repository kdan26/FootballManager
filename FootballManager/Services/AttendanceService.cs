using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _db;

        public AttendanceService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<MatchAttendanceViewModel?> GetMatchAttendanceAsync(int matchId, int teamId)
        {
            var match = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) return null;

            var team = await _db.Teams.FindAsync(teamId);
            if (team == null) return null;

            // Lấy danh sách cầu thủ active của đội
            var players = await _db.Players
                .Where(p => p.TeamId == teamId && p.IsActive)
                .OrderBy(p => p.JerseyNumber)
                .ToListAsync();

            // Lấy điểm danh đã có
            var existing = await _db.PlayerAttendances
                .Where(pa => pa.MatchId == matchId && pa.Player.TeamId == teamId)
                .ToListAsync();

            var items = players.Select(p =>
            {
                var att = existing.FirstOrDefault(a => a.PlayerId == p.Id);
                return new AttendancePlayerItem
                {
                    PlayerId = p.Id,
                    FullName = p.FullName,
                    JerseyNumber = p.JerseyNumber,
                    PositionLabel = p.Position switch
                    {
                        PlayerPosition.GoalKeeper   => "Thủ môn",
                        PlayerPosition.CenterBack   => "Trung vệ",
                        PlayerPosition.FullBack     => "Hậu vệ biên",
                        PlayerPosition.DefensiveMid => "Tiền vệ phòng ngự",
                        PlayerPosition.CentralMid   => "Tiền vệ trung tâm",
                        PlayerPosition.AttackingMid => "Tiền vệ công",
                        PlayerPosition.Winger       => "Chạy cánh",
                        PlayerPosition.Striker      => "Tiền đạo",
                        _ => ""
                    },
                    Status = att?.Status ?? AttendanceStatus.Unknown,
                    Note = att?.Note
                };
            }).ToList();

            return new MatchAttendanceViewModel
            {
                MatchId = matchId,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                MatchDate = match.MatchDate,
                TeamId = teamId,
                TeamName = team.Name,
                Players = items
            };
        }

        public async Task SaveAttendanceAsync(SaveAttendanceViewModel model)
        {
            foreach (var item in model.Players)
            {
                var existing = await _db.PlayerAttendances
                    .FirstOrDefaultAsync(pa => pa.MatchId == model.MatchId && pa.PlayerId == item.PlayerId);

                if (existing == null)
                {
                    _db.PlayerAttendances.Add(new PlayerAttendance
                    {
                        MatchId = model.MatchId,
                        PlayerId = item.PlayerId,
                        Status = item.Status,
                        Note = item.Note,
                        RecordedAt = DateTime.Now
                    });
                }
                else
                {
                    existing.Status = item.Status;
                    existing.Note = item.Note;
                    existing.RecordedAt = DateTime.Now;
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
