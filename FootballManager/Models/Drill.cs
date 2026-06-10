using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public class Drill
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public TrainingCategory Category { get; set; } = TrainingCategory.Physical;

        [Range(1, 120)]
        public int? DurationMinutes { get; set; }

        [Range(1, 5)]
        public int Difficulty { get; set; } = 3;

        public string? Instructions { get; set; } // Hướng dẫn chi tiết (nvarchar(max))

        [MaxLength(500)]
        public string? VideoUrl { get; set; } // Link YouTube / video minh họa

        [Required]
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public bool IsShared { get; set; } = false; // true = tất cả HLV thấy
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<TrainingSessionDrill> SessionDrills { get; set; } = new List<TrainingSessionDrill>();
        public ICollection<PlayerTrainingStats> TrainingStats { get; set; } = new List<PlayerTrainingStats>();
    }
}
