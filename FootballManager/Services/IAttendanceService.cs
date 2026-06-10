using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IAttendanceService
    {
        Task<MatchAttendanceViewModel?> GetMatchAttendanceAsync(int matchId, int teamId);
        Task SaveAttendanceAsync(SaveAttendanceViewModel model);
    }
}
