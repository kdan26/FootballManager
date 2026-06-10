using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class TrainingSessionDrill
    {
        public int Id { get; set; }

        [Required]
        public int TrainingSessionId { get; set; }
        public TrainingSession TrainingSession { get; set; } = null!;

        [Required]
        public int DrillId { get; set; }
        public Drill Drill { get; set; } = null!;

        public int OrderIndex { get; set; } = 0; // Thứ tự trong buổi tập

        public int? ActualDurationMinutes { get; set; } // Thời gian thực tế

        [MaxLength(500)]
        public string? Notes { get; set; } // Ghi chú riêng cho bài này trong buổi đó
    }
}
