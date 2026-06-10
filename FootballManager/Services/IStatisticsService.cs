using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IStatisticsService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
