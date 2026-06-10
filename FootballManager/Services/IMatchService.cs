using FootballManager.Models;
using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IMatchService
    {
        Task<List<MatchListItemViewModel>> GetAllMatchesAsync();
        Task<MatchDetailsViewModel?> GetMatchDetailsAsync(int id);
        Task<(bool Success, string? ErrorMessage)> CreateMatchAsync(MatchCreateViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateResultAsync(int id, MatchResultViewModel model);
        Task<(bool Success, string? ErrorMessage)> CancelMatchAsync(int id);
        Task<List<Team>> GetAllTeamsForSelectAsync();
    }
}
