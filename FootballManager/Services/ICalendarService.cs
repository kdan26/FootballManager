using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface ICalendarService
    {
        /// <summary>Lấy events cho FullCalendar (trả về JSON)</summary>
        Task<List<CalendarEventDto>> GetEventsAsync(int? teamId, DateTime start, DateTime end);

        /// <summary>Lấy trang điểm danh buổi tập</summary>
        Task<TrainingAttendanceViewModel?> GetAttendanceAsync(int sessionId);

        /// <summary>Lưu điểm danh</summary>
        Task SaveAttendanceAsync(SaveTrainingAttendanceViewModel model);
    }
}
