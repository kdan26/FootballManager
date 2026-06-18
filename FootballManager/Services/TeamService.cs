using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _db;

        public TeamService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TeamListItemViewModel>> GetAllTeamsAsync()
        {
            return await _db.Teams
                .Include(t => t.HomeMatches)
                .Include(t => t.AwayMatches)
                .Select(t => new TeamListItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    HomeGround = t.HomeGround,
                    MatchCount = t.HomeMatches.Count + t.AwayMatches.Count
                })
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<TeamDetailsViewModel?> GetTeamDetailsAsync(int id)
        {
            var team = await _db.Teams
                .Include(t => t.HomeMatches).ThenInclude(m => m.AwayTeam)
                .Include(t => t.AwayMatches).ThenInclude(m => m.HomeTeam)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null) return null;

            var matches = team.HomeMatches.Select(m => new MatchListItemViewModel
            {
                Id = m.Id,
                HomeTeamName = team.Name,
                AwayTeamName = m.AwayTeam.Name,
                MatchDate = m.MatchDate,
                Venue = m.Venue,
                Status = m.Status,
                HomeScore = m.HomeScore,
                AwayScore = m.AwayScore
            }).Concat(team.AwayMatches.Select(m => new MatchListItemViewModel
            {
                Id = m.Id,
                HomeTeamName = m.HomeTeam.Name,
                AwayTeamName = team.Name,
                MatchDate = m.MatchDate,
                Venue = m.Venue,
                Status = m.Status,
                HomeScore = m.HomeScore,
                AwayScore = m.AwayScore
            })).OrderByDescending(m => m.MatchDate).ToList();

            return new TeamDetailsViewModel
            {
                Id = team.Id,
                Name = team.Name,
                HomeGround = team.HomeGround,
                Description = team.Description,
                Matches = matches
            };
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _db.Teams.FindAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateTeamAsync(TeamCreateViewModel model)
        {
            var exists = await _db.Teams.AnyAsync(t => t.Name == model.Name);
            if (exists)
                return (false, "Tên đội đã tồn tại");

            _db.Teams.Add(new Team
            {
                Name = model.Name,
                HomeGround = model.HomeGround,
                Description = model.Description
            });

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateTeamAsync(int id, TeamEditViewModel model)
        {
            var team = await _db.Teams.FindAsync(id);
            if (team == null)
                return (false, "Không tìm thấy đội bóng");

            // Kiểm tra tên trùng với đội khác
            var exists = await _db.Teams.AnyAsync(t => t.Name == model.Name && t.Id != id);
            if (exists)
                return (false, "Tên đội đã tồn tại");

            team.Name = model.Name;
            team.HomeGround = model.HomeGround;
            team.Description = model.Description;

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteTeamAsync(int id)
        {
            var team = await _db.Teams.FindAsync(id);
            if (team == null)
                return (false, "Không tìm thấy đội bóng");

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1. Lấy tất cả match liên quan
                var matchIds = await _db.Matches
                    .Where(m => m.HomeTeamId == id || m.AwayTeamId == id)
                    .Select(m => m.Id)
                    .ToListAsync();

                if (matchIds.Any())
                {
                    // Xóa MatchEvent của các trận
                    _db.MatchEvents.RemoveRange(
                        _db.MatchEvents.Where(e => matchIds.Contains(e.MatchId)));

                    // Xóa PlayerAttendance của các trận
                    _db.PlayerAttendances.RemoveRange(
                        _db.PlayerAttendances.Where(a => matchIds.Contains(a.MatchId)));

                    // Xóa PlayerMatchStats của các trận
                    _db.PlayerMatchStats.RemoveRange(
                        _db.PlayerMatchStats.Where(s => matchIds.Contains(s.MatchId)));

                    // Xóa PerformanceRating gắn với trận
                    _db.PerformanceRatings.RemoveRange(
                        _db.PerformanceRatings.Where(r => r.MatchId != null && matchIds.Contains(r.MatchId!.Value)));

                    await _db.SaveChangesAsync();

                    // Xóa các trận đấu
                    _db.Matches.RemoveRange(
                        _db.Matches.Where(m => matchIds.Contains(m.Id)));
                    await _db.SaveChangesAsync();
                }

                // 2. Lấy tất cả cầu thủ của đội
                var playerIds = await _db.Players
                    .Where(p => p.TeamId == id)
                    .Select(p => p.Id)
                    .ToListAsync();

                if (playerIds.Any())
                {
                    // Xóa dữ liệu còn lại của cầu thủ
                    _db.PlayerTrainingStats.RemoveRange(
                        _db.PlayerTrainingStats.Where(s => playerIds.Contains(s.PlayerId)));

                    _db.TrainingAttendances.RemoveRange(
                        _db.TrainingAttendances.Where(a => playerIds.Contains(a.PlayerId)));

                    _db.PerformanceRatings.RemoveRange(
                        _db.PerformanceRatings.Where(r => playerIds.Contains(r.PlayerId)));

                    await _db.SaveChangesAsync();

                    // Xóa cầu thủ
                    _db.Players.RemoveRange(
                        _db.Players.Where(p => playerIds.Contains(p.Id)));
                    await _db.SaveChangesAsync();
                }

                // 3. Xóa TrainingSession của đội
                var sessionIds = await _db.TrainingSessions
                    .Where(s => s.TeamId == id)
                    .Select(s => s.Id)
                    .ToListAsync();

                if (sessionIds.Any())
                {
                    _db.TrainingSessionDrills.RemoveRange(
                        _db.TrainingSessionDrills.Where(sd => sessionIds.Contains(sd.TrainingSessionId)));

                    _db.TrainingAttendances.RemoveRange(
                        _db.TrainingAttendances.Where(a => sessionIds.Contains(a.TrainingSessionId)));

                    await _db.SaveChangesAsync();

                    _db.TrainingSessions.RemoveRange(
                        _db.TrainingSessions.Where(s => sessionIds.Contains(s.Id)));
                    await _db.SaveChangesAsync();
                }

                // 4. Xóa TeamTactics
                _db.TeamTactics.RemoveRange(
                    _db.TeamTactics.Where(t => t.TeamId == id));
                await _db.SaveChangesAsync();

                // 5. Xóa đội
                _db.Teams.Remove(team);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return (false, $"Không thể xóa đội bóng: {ex.Message}");
            }
        }
    }
}
