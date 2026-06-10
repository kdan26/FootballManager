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
            var team = await _db.Teams
                .Include(t => t.HomeMatches)
                .Include(t => t.AwayMatches)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return (false, "Không tìm thấy đội bóng");

            if (team.HomeMatches.Any() || team.AwayMatches.Any())
                return (false, "Không thể xóa đội đang có trận đấu liên quan");

            _db.Teams.Remove(team);
            await _db.SaveChangesAsync();
            return (true, null);
        }
    }
}
