using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum TrainingAttendanceStatus
    {
        Present,    // Có mặt
        Late,       // Đến muộn
        Absent,     // Vắng mặt
        Excused,    // Xin phép
        Injured     // Chấn thương
    }

    /// <summary>
    /// Điểm danh cầu thủ cho từng buổi tập / họp chiến thuật.
    /// </summary>
    public class TrainingAttendance
    {
        public int Id { get; set; }

        [Required]
        public int TrainingSessionId { get; set; }
        public TrainingSession TrainingSession { get; set; } = null!;

        [Required]
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public TrainingAttendanceStatus Status { get; set; } = TrainingAttendanceStatus.Present;

        public int? LateMinutes { get; set; }           // Đến muộn bao nhiêu phút

        [MaxLength(300)]
        public string? Note { get; set; }               // Lý do / ghi chú

        public DateTime? CheckInTime { get; set; }      // Giờ check-in thực tế

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}
