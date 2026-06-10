using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum TrainingCategory
    {
        Physical,   // Thể lực
        Technical,  // Kỹ thuật
        Tactical    // Chiến thuật
    }

    /// <summary>
    /// Chỉ số bài tập đào tạo của cầu thủ theo từng buổi tập.
    /// Hiển thị khi click vào cầu thủ.
    /// </summary>
    public class PlayerTrainingStats
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        [Required]
        public DateTime TrainingDate { get; set; }

        public TrainingCategory Category { get; set; }

        [Required, MaxLength(200)]
        public string DrillName { get; set; } = string.Empty; // Tên bài tập (tự nhập hoặc lấy từ thư viện)

        // FK tuỳ chọn → Drill (null = nhập tay, không null = chọn từ thư viện)
        public int? DrillId { get; set; }
        public Drill? Drill { get; set; }

        // Thể lực
        public decimal? SprintSpeed { get; set; }       // Tốc độ chạy nước rút (km/h)
        public decimal? EnduranceScore { get; set; }    // Điểm sức bền (0-10)
        public decimal? DistanceRun { get; set; }       // Quãng đường chạy (km)

        // Kỹ thuật
        public decimal? TechniqueScore { get; set; }    // Điểm kỹ thuật (0-10)
        public decimal? PassAccuracy { get; set; }      // % chuyền chính xác trong tập
        public int? ShotsOnTarget { get; set; }         // Số cú sút trúng đích

        // Chiến thuật
        public decimal? TacticsScore { get; set; }      // Điểm chiến thuật (0-10)

        // Đánh giá tổng thể của HLV
        [Range(1, 10)]
        public decimal? CoachRating { get; set; }       // HLV chấm điểm (1-10)

        [MaxLength(500)]
        public string? CoachNotes { get; set; }         // Nhận xét của HLV

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}
