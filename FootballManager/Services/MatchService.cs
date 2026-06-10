using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _db;

        public MatchService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<MatchListItemViewModel>> GetAllMatchesAsync()
        {
            return await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .OrderByDescending(m => m.MatchDate)
                .Select(m => new MatchListItemViewModel
                {
                    Id = m.Id,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamName = m.AwayTeam.Name,
                    MatchDate = m.MatchDate,
                    Venue = m.Venue,
                    Status = m.Status,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore
                })
                .ToListAsync();
        }

        public async Task<MatchDetailsViewModel?> GetMatchDetailsAsync(int id)
        {
            var match = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null) return null;

            return new MatchDetailsViewModel
            {
                Id = match.Id,
                HomeTeamName = match.HomeTeam.Name,
                AwayTeamName = match.AwayTeam.Name,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                MatchDate = match.MatchDate,
                Venue = match.Venue,
                Notes = match.Notes,
                Status = match.Status,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateMatchAsync(MatchCreateViewModel model)
        {
            if (model.HomeTeamId == model.AwayTeamId)
                return (false, "Đội nhà và đội khách không được trùng nhau");

            _db.Matches.Add(new Match
            {
                HomeTeamId = model.HomeTeamId,
                AwayTeamId = model.AwayTeamId,
                MatchDate = model.MatchDate,
                Venue = model.Venue,
                Notes = model.Notes,
                Status = MatchStatus.Scheduled
            });

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateResultAsync(int id, MatchResultViewModel model)
        {
            var match = await _db.Matches.FindAsync(id);
            if (match == null)
                return (false, "Không tìm thấy trận đấu");

            if (match.Status != MatchStatus.Scheduled)
                return (false, "Trận đấu này đã kết thúc hoặc đã hủy");

            match.HomeScore = model.HomeScore;
            match.AwayScore = model.AwayScore;
            match.Status = MatchStatus.Completed;

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CancelMatchAsync(int id)
        {
            var match = await _db.Matches.FindAsync(id);
            if (match == null)
                return (false, "Không tìm thấy trận đấu");

            if (match.Status != MatchStatus.Scheduled)
                return (false, "Không thể hủy trận đấu đã kết thúc hoặc đã hủy trước đó");

            match.Status = MatchStatus.Cancelled;
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<Team>> GetAllTeamsForSelectAsync()
        {
            return await _db.Teams.OrderBy(t => t.Name).ToListAsync();
        }
    }
}
