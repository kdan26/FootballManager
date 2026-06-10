using FootballManager.Models;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    public class PlayerTrainingStatsFormViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ngày tập")]
        public DateTime TrainingDate { get; set; } = DateTime.Today;

        [Display(Name = "Loại bài tập")]
        public TrainingCategory Category { get; set; }

        [Required, MaxLength(200)]
        [Display(Name = "Tên bài tập")]
        public string DrillName { get; set; } = string.Empty;

        // Chọn từ thư viện (tuỳ chọn)
        [Display(Name = "Chọn từ thư viện")]
        public int? DrillId { get; set; }

        // Thể lực
        [Display(Name = "Tốc độ chạy nước rút (km/h)")]
        public decimal? SprintSpeed { get; set; }

        [Display(Name = "Điểm sức bền (0-10)")]
        [Range(0, 10)]
        public decimal? EnduranceScore { get; set; }

        [Display(Name = "Quãng đường chạy (km)")]
        public decimal? DistanceRun { get; set; }

        // Kỹ thuật
        [Display(Name = "Điểm kỹ thuật (0-10)")]
        [Range(0, 10)]
        public decimal? TechniqueScore { get; set; }

        [Display(Name = "% Chuyền chính xác trong tập")]
        [Range(0, 100)]
        public decimal? PassAccuracy { get; set; }

        [Display(Name = "Số cú sút trúng đích")]
        public int? ShotsOnTarget { get; set; }

        // Chiến thuật
        [Display(Name = "Điểm chiến thuật (0-10)")]
        [Range(0, 10)]
        public decimal? TacticsScore { get; set; }

        // HLV đánh giá
        [Display(Name = "Điểm HLV chấm (1-10)")]
        [Range(1, 10)]
        public decimal? CoachRating { get; set; }

        [Display(Name = "Nhận xét của HLV")]
        [MaxLength(500)]
        public string? CoachNotes { get; set; }
    }

    public class PlayerTrainingStatsListItem
    {
        public int Id { get; set; }
        public DateTime TrainingDate { get; set; }
        public string DrillName { get; set; } = string.Empty;
        public TrainingCategory Category { get; set; }
        public string CategoryLabel => Category switch
        {
            TrainingCategory.Physical  => "Thể lực",
            TrainingCategory.Technical => "Kỹ thuật",
            TrainingCategory.Tactical  => "Chiến thuật",
            _ => ""
        };
        public decimal? CoachRating { get; set; }
        public string? CoachNotes { get; set; }
    }
}
