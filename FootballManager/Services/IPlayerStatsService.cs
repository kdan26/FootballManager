using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IPlayerStatsService
    {
        Task<PlayerStatsViewModel> GetStatsAsync(string? teamName = null);
        Task<PlayerStatsItem?> GetPlayerStatsAsync(int playerId);
    }
}
