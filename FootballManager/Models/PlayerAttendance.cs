using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum AttendanceStatus
    {
        Present,   // Có mặt
        Absent,    // Vắng mặt
        Injured,   // Chấn thương
        Unknown    // Chưa xác nhận
    }

    public class PlayerAttendance
    {
        public int Id { get; set; }

        [Required]
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        [Required]
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Unknown;

        [MaxLength(200)]
        public string? Note { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}
