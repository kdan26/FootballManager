using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    /// <summary>
    /// Lưu sơ đồ chiến thuật và ghi chú của đội bóng.
    /// Mỗi đội có tối đa 1 bản ghi (upsert).
    /// </summary>
    public class TeamTactics
    {
        public int Id { get; set; }

        [Required]
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Formation { get; set; } = "4-3-3"; // VD: 4-3-3, 4-4-2, 3-5-2

        [MaxLength(2000)]
        public string? Notes { get; set; } // Ghi chú chiến thuật tự do

        // JSON lưu tọa độ cầu thủ: [{id,name,jerseyNumber,x,y}]
        public string? PositionsJson { get; set; }

        // JSON lưu mũi tên di chuyển: [{x1,y1,x2,y2,color,dash}]
        public string? ArrowsJson { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
