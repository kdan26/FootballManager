using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum RatingType
    {
        AfterMatch,     // Sau trận đấu
        AfterTraining,  // Sau buổi tập
        Weekly,         // Đánh giá tuần
        Monthly         // Đánh giá tháng
    }

    public class PerformanceRating
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        /// <summary>HLV hoặc chuyên viên phân tích thực hiện đánh giá</summary>
        [Required]
        public int RatedByUserId { get; set; }
        public User RatedByUser { get; set; } = null!;

        public RatingType RatingType { get; set; } = RatingType.AfterMatch;

        /// <summary>ID của Match (nếu RatingType = AfterMatch)</summary>
        public int? MatchId { get; set; }
        public Match? Match { get; set; }

        /// <summary>ID của PlayerTrainingStats (nếu RatingType = AfterTraining)</summary>
        public int? TrainingStatsId { get; set; }
        public PlayerTrainingStats? TrainingStats { get; set; }

        [Required]
        public DateTime RatingDate { get; set; } = DateTime.Now;

        // ===== CÁC TIÊU CHÍ CHẤM ĐIỂM (1-10) =====

        [Required, Range(1, 10)]
        public decimal OverallRating { get; set; }          // Điểm tổng thể

        [Range(1, 10)]
        public decimal? AttitudeRating { get; set; }        // Thái độ / nỗ lực

        [Range(1, 10)]
        public decimal? FitnessRating { get; set; }         // Thể lực / sức bền

        [Range(1, 10)]
        public decimal? TechnicalRating { get; set; }       // Kỹ thuật cá nhân

        [Range(1, 10)]
        public decimal? TacticalRating { get; set; }        // Hiểu chiến thuật / định vị

        [MaxLength(1000)]
        public string? Notes { get; set; }                  // Nhận xét chi tiết

        /// <summary>HLV kiểm soát — true thì cầu thủ mới thấy</summary>
        public bool IsPublishedToPlayer { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
