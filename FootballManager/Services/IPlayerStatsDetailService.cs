using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IPlayerStatsDetailService
    {
        // Match stats
        Task<PlayerMatchStatsFormViewModel?> GetMatchStatsFormAsync(int playerId, int? matchId = null);
        Task<List<PlayerMatchStatsListItem>> GetMatchStatsListAsync(int playerId);
        Task SaveMatchStatsAsync(PlayerMatchStatsFormViewModel model);

        // Training stats
        Task<List<PlayerTrainingStatsListItem>> GetTrainingStatsListAsync(int playerId);
        Task<PlayerTrainingStatsFormViewModel> GetTrainingStatsFormAsync(int playerId);
        Task SaveTrainingStatsAsync(PlayerTrainingStatsFormViewModel model);
        Task DeleteTrainingStatsAsync(int id);

        // Performance chart
        Task<PerformanceChartViewModel> GetPerformanceChartAsync(int playerId);
    }
}
