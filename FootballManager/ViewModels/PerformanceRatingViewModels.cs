using FootballManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    // ===== FORM THÊM / SỬA RATING =====
    public class PerformanceRatingFormViewModel
    {
        public int Id { get; set; }  // 0 = thêm mới

        [Required]
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;

        [Display(Name = "Loại đánh giá")]
        public RatingType RatingType { get; set; } = RatingType.AfterMatch;

        [Display(Name = "Chọn trận đấu")]
        public int? MatchId { get; set; }

        [Display(Name = "Ngày đánh giá")]
        [Required]
        public DateTime RatingDate { get; set; } = DateTime.Today;

        // ===== ĐIỂM SỐ =====

        [Required]
        [Display(Name = "Điểm tổng thể")]
        [Range(1, 10, ErrorMessage = "Điểm phải từ 1 đến 10")]
        public decimal OverallRating { get; set; }

        [Display(Name = "Thái độ / Nỗ lực")]
        [Range(1, 10, ErrorMessage = "Điểm phải từ 1 đến 10")]
        public decimal? AttitudeRating { get; set; }

        [Display(Name = "Thể lực")]
        [Range(1, 10, ErrorMessage = "Điểm phải từ 1 đến 10")]
        public decimal? FitnessRating { get; set; }

        [Display(Name = "Kỹ thuật")]
        [Range(1, 10, ErrorMessage = "Điểm phải từ 1 đến 10")]
        public decimal? TechnicalRating { get; set; }

        [Display(Name = "Chiến thuật")]
        [Range(1, 10, ErrorMessage = "Điểm phải từ 1 đến 10")]
        public decimal? TacticalRating { get; set; }

        [Display(Name = "Nhận xét")]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Display(Name = "Công bố cho cầu thủ xem")]
        public bool IsPublishedToPlayer { get; set; } = false;

        // Dropdown
        public List<SelectListItem> Matches { get; set; } = new();
    }

    // ===== ITEM TRONG DANH SÁCH =====
    public class PerformanceRatingListItem
    {
        public int Id { get; set; }
        public RatingType RatingType { get; set; }
        public string RatingTypeLabel => RatingType switch
        {
            RatingType.AfterMatch    => "Sau trận",
            RatingType.AfterTraining => "Sau buổi tập",
            RatingType.Weekly        => "Tuần",
            RatingType.Monthly       => "Tháng",
            _ => ""
        };
        public DateTime RatingDate { get; set; }
        public decimal OverallRating { get; set; }
        public decimal? AttitudeRating { get; set; }
        public decimal? FitnessRating { get; set; }
        public decimal? TechnicalRating { get; set; }
        public decimal? TacticalRating { get; set; }
        public string? Notes { get; set; }
        public string RatedByName { get; set; } = string.Empty;
        public string? MatchLabel { get; set; }
        public bool IsPublishedToPlayer { get; set; }
    }

    // ===== DỮ LIỆU BIỂU ĐỒ PHONG ĐỘ =====
    public class RatingChartPoint
    {
        public string Label { get; set; } = string.Empty;   // "15/05 - Sau trận"
        public decimal OverallRating { get; set; }
        public decimal? AttitudeRating { get; set; }
        public decimal? FitnessRating { get; set; }
        public decimal? TechnicalRating { get; set; }
        public decimal? TacticalRating { get; set; }
        public string RatingTypeLabel { get; set; } = string.Empty;
    }

    // ===== TRANG DANH SÁCH + BIỂU ĐỒ =====
    public class PerformanceRatingIndexViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string PositionLabel { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }

        public List<PerformanceRatingListItem> Ratings { get; set; } = new();
        public List<RatingChartPoint> ChartPoints { get; set; } = new();

        // Thống kê tóm tắt
        public decimal? AvgOverall { get; set; }
        public decimal? AvgAttitude { get; set; }
        public decimal? AvgFitness { get; set; }
        public decimal? AvgTechnical { get; set; }
        public decimal? AvgTactical { get; set; }

        // Xu hướng: so sánh 3 đánh giá gần nhất vs 3 trước đó
        public decimal? Trend { get; set; }   // dương = đang tăng, âm = đang giảm
        public string TrendLabel => Trend switch
        {
            > 0 => "Tăng",
            < 0 => "Giảm",
            _   => "Ổn định"
        };
        public string TrendClass => Trend switch
        {
            > 0 => "text-success",
            < 0 => "text-danger",
            _   => "text-muted"
        };
        public string TrendIcon => Trend switch
        {
            > 0 => "bi-arrow-up-circle-fill",
            < 0 => "bi-arrow-down-circle-fill",
            _   => "bi-dash-circle-fill"
        };

        public int TotalRatings { get; set; }
    }
}
