using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IMatchEventService
    {
        Task<MatchEventsViewModel?> GetMatchEventsAsync(int matchId);
        Task<AddMatchEventViewModel?> GetAddEventFormAsync(int matchId);
        Task<(bool Success, string? ErrorMessage)> AddEventAsync(AddMatchEventViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteEventAsync(int eventId);
    }
}
