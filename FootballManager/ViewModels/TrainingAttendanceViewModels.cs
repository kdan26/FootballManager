using FootballManager.Models;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    // ─── 1 dòng cầu thủ trong bảng điểm danh ───
    public class TrainingAttendanceItem
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public string PositionLabel { get; set; } = string.Empty;
        public TrainingAttendanceStatus Status { get; set; } = TrainingAttendanceStatus.Present;
        public int? LateMinutes { get; set; }
        public string? Note { get; set; }
        public DateTime? CheckInTime { get; set; }
    }

    // ─── Trang điểm danh 1 buổi tập ───
    public class TrainingAttendanceViewModel
    {
        public int SessionId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string? Location { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public TrainingSessionStatus SessionStatus { get; set; }

        public List<TrainingAttendanceItem> Players { get; set; } = new();

        // Tóm tắt
        public int PresentCount  => Players.Count(p => p.Status == TrainingAttendanceStatus.Present);
        public int LateCount     => Players.Count(p => p.Status == TrainingAttendanceStatus.Late);
        public int AbsentCount   => Players.Count(p => p.Status == TrainingAttendanceStatus.Absent);
        public int ExcusedCount  => Players.Count(p => p.Status == TrainingAttendanceStatus.Excused);
        public int InjuredCount  => Players.Count(p => p.Status == TrainingAttendanceStatus.Injured);
    }

    // ─── Gửi lên khi save điểm danh ───
    public class SaveTrainingAttendanceViewModel
    {
        public int SessionId { get; set; }
        public int TeamId { get; set; }
        public List<TrainingAttendanceItem> Players { get; set; } = new();
    }

    // ─── Dữ liệu event cho FullCalendar (JSON) ───
    public class CalendarEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;   // ISO 8601
        public string? End { get; set; }
        public string Color { get; set; } = "#0d6efd";
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;    // "training" | "match"
        public string StatusLabel { get; set; } = string.Empty;
    }
}
