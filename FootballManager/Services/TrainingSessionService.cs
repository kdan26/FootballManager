using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class TrainingSessionService : ITrainingSessionService
    {
        private readonly ApplicationDbContext _db;

        public TrainingSessionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<TrainingSessionIndexViewModel> GetSessionIndexAsync(int? teamId)
        {
            var teams = await _db.Teams.OrderBy(t => t.Name).ToListAsync();

            var query = _db.TrainingSessions
                .Include(s => s.Team)
                .Include(s => s.SessionDrills)
                .AsQueryable();

            if (teamId.HasValue)
                query = query.Where(s => s.TeamId == teamId.Value);

            var sessions = await query
                .OrderByDescending(s => s.ScheduledAt)
                .Select(s => new TrainingSessionListItem
                {
                    Id          = s.Id,
                    Title       = s.Title,
                    ScheduledAt = s.ScheduledAt,
                    Location    = s.Location,
                    Status      = s.Status,
                    TeamName    = s.Team.Name,
                    DrillCount  = s.SessionDrills.Count
                })
                .ToListAsync();

            return new TrainingSessionIndexViewModel
            {
                Sessions       = sessions,
                SelectedTeamId = teamId,
                Teams          = teams.Select(t => new SelectListItem(t.Name, t.Id.ToString())).ToList()
            };
        }

        public async Task<TrainingSessionDetailViewModel?> GetSessionDetailsAsync(int id)
        {
            var s = await _db.TrainingSessions
                .Include(x => x.Team)
                .Include(x => x.CreatedByUser)
                .Include(x => x.SessionDrills).ThenInclude(sd => sd.Drill)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (s == null) return null;

            return new TrainingSessionDetailViewModel
            {
                Id            = s.Id,
                Title         = s.Title,
                ScheduledAt   = s.ScheduledAt,
                Location      = s.Location,
                CoachNotes    = s.CoachNotes,
                Status        = s.Status,
                TeamName      = s.Team.Name,
                TeamId        = s.TeamId,
                CreatedByName = s.CreatedByUser.FullName,
                Drills        = s.SessionDrills
                    .OrderBy(sd => sd.OrderIndex)
                    .Select(sd => new SessionDrillItem
                    {
                        SessionDrillId = sd.Id,
                        DrillId        = sd.DrillId,
                        DrillName      = sd.Drill.Name,
                        CategoryLabel  = sd.Drill.Category switch
                        {
                            TrainingCategory.Physical  => "Thể lực",
                            TrainingCategory.Technical => "Kỹ thuật",
                            TrainingCategory.Tactical  => "Chiến thuật",
                            _                          => ""
                        },
                        OrderIndex     = sd.OrderIndex,
                        ActualMinutes  = sd.ActualDurationMinutes,
                        Notes          = sd.Notes
                    }).ToList()
            };
        }

        public async Task<TrainingSessionFormViewModel> GetSessionFormAsync(int? id, int? teamId)
        {
            var teams = await _db.Teams.OrderBy(t => t.Name)
                .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
                .ToListAsync();

            var drills = await _db.Drills
                .Where(d => d.IsActive)
                .OrderBy(d => d.Category).ThenBy(d => d.Name)
                .Select(d => new DrillSelectItem
                {
                    Id              = d.Id,
                    Name            = d.Name,
                    Category        = d.Category,
                    DurationMinutes = d.DurationMinutes,
                    Description     = d.Description
                })
                .ToListAsync();

            if (id == null || id == 0)
            {
                return new TrainingSessionFormViewModel
                {
                    ScheduledAt    = DateTime.Now.Date.AddDays(1).AddHours(9),
                    TeamId         = teamId ?? 0,
                    Teams          = teams,
                    AvailableDrills = drills
                };
            }

            var s = await _db.TrainingSessions
                .Include(x => x.SessionDrills)
                .FirstOrDefaultAsync(x => x.Id == id.Value);

            if (s == null)
                return new TrainingSessionFormViewModel { Teams = teams, AvailableDrills = drills };

            return new TrainingSessionFormViewModel
            {
                Id               = s.Id,
                TeamId           = s.TeamId,
                Title            = s.Title,
                ScheduledAt      = s.ScheduledAt,
                Location         = s.Location,
                CoachNotes       = s.CoachNotes,
                SelectedDrillIds = s.SessionDrills.Select(sd => sd.DrillId).ToList(),
                Teams            = teams,
                AvailableDrills  = drills
            };
        }

        public async Task<int> SaveSessionAsync(TrainingSessionFormViewModel model, int createdByUserId)
        {
            TrainingSession session;

            if (model.Id > 0)
            {
                session = await _db.TrainingSessions
                    .Include(s => s.SessionDrills)
                    .FirstOrDefaultAsync(s => s.Id == model.Id)
                    ?? throw new InvalidOperationException("Không tìm thấy buổi tập");

                // Xóa drills cũ rồi thêm lại
                _db.TrainingSessionDrills.RemoveRange(session.SessionDrills);
            }
            else
            {
                session = new TrainingSession { CreatedByUserId = createdByUserId };
                _db.TrainingSessions.Add(session);
            }

            session.TeamId     = model.TeamId;
            session.Title      = model.Title;
            session.ScheduledAt = model.ScheduledAt;
            session.Location   = model.Location;
            session.CoachNotes = model.CoachNotes;

            // Thêm drills
            for (int i = 0; i < model.SelectedDrillIds.Count; i++)
            {
                session.SessionDrills.Add(new TrainingSessionDrill
                {
                    DrillId    = model.SelectedDrillIds[i],
                    OrderIndex = i
                });
            }

            await _db.SaveChangesAsync();
            return session.Id;
        }

        public async Task DeleteSessionAsync(int id)
        {
            var s = await _db.TrainingSessions.FindAsync(id);
            if (s != null) { _db.TrainingSessions.Remove(s); await _db.SaveChangesAsync(); }
        }

        public async Task CompleteSessionAsync(int id)
        {
            var s = await _db.TrainingSessions.FindAsync(id);
            if (s != null) { s.Status = TrainingSessionStatus.Completed; await _db.SaveChangesAsync(); }
        }

        public async Task CancelSessionAsync(int id)
        {
            var s = await _db.TrainingSessions.FindAsync(id);
            if (s != null) { s.Status = TrainingSessionStatus.Cancelled; await _db.SaveChangesAsync(); }
        }

        public async Task RemoveDrillFromSessionAsync(int sessionDrillId)
        {
            var sd = await _db.TrainingSessionDrills.FindAsync(sessionDrillId);
            if (sd != null) { _db.TrainingSessionDrills.Remove(sd); await _db.SaveChangesAsync(); }
        }
    }
}
