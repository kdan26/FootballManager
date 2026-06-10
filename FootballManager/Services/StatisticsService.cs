using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext _db;

        public StatisticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var today = DateTime.Today;

            var totalTeams = await _db.Teams.CountAsync();
            var totalMatches = await _db.Matches.CountAsync();
            var completedMatches = await _db.Matches.CountAsync(m => m.Status == MatchStatus.Completed);
            var upcomingMatchCount = await _db.Matches.CountAsync(m =>
                m.Status == MatchStatus.Scheduled && m.MatchDate >= today);

            var upcomingMatches = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Status == MatchStatus.Scheduled && m.MatchDate >= today)
                .OrderBy(m => m.MatchDate)
                .Take(5)
                .Select(m => new MatchListItemViewModel
                {
                    Id = m.Id,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamName = m.AwayTeam.Name,
                    MatchDate = m.MatchDate,
                    Venue = m.Venue,
                    Status = m.Status
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalTeams = totalTeams,
                TotalMatches = totalMatches,
                CompletedMatches = completedMatches,
                UpcomingMatchCount = upcomingMatchCount,
                UpcomingMatches = upcomingMatches
            };
        }
    }
}
