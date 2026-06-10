using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum TrainingSessionStatus
    {
        Scheduled,   // Đã lên lịch
        Completed,   // Đã hoàn thành
        Cancelled    // Đã hủy
    }

    public class TrainingSession
    {
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledAt { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public TrainingSessionStatus Status { get; set; } = TrainingSessionStatus.Scheduled;

        [MaxLength(1000)]
        public string? CoachNotes { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<TrainingSessionDrill> SessionDrills { get; set; } = new List<TrainingSessionDrill>();
        public ICollection<TrainingAttendance> Attendances { get; set; } = new List<TrainingAttendance>();
    }
}
