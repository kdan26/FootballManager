using FootballManager.Models;
using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface ITeamService
    {
        Task<List<TeamListItemViewModel>> GetAllTeamsAsync();
        Task<TeamDetailsViewModel?> GetTeamDetailsAsync(int id);
        Task<Team?> GetTeamByIdAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateTeamAsync(TeamCreateViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateTeamAsync(int id, TeamEditViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteTeamAsync(int id);
    }
}
