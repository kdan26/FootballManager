using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    /// <summary>
    /// Chỉ số thi đấu của cầu thủ trong một trận cụ thể.
    /// Các chỉ số được nhập tay sau trận, phân theo vị trí.
    /// </summary>
    public class PlayerMatchStats
    {
        public int Id { get; set; }

        [Required]
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        [Required]
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        // === CHUNG CHO MỌI VỊ TRÍ ===
        public int MinutesPlayed { get; set; }
        public decimal? PassCompletionPct { get; set; }     // % chuyền chính xác

        // === THỦ MÔN (GoalKeeper) ===
        public decimal? PsxGDiff { get; set; }              // PSxG +/- (dương = xuất sắc)
        public decimal? SavePct { get; set; }               // % cứu thua
        public decimal? LaunchAccuracyPct { get; set; }     // % chuyền dài chính xác

        // === HẬU VỆ BIÊN (FullBack) ===
        public int? AccurateCrosses { get; set; }           // Số quả tạt chính xác
        public decimal? TacklesWonPct { get; set; }         // % tắc bóng thành công
        public decimal? DistanceCovered { get; set; }       // Quãng đường di chuyển (km)

        // === TRUNG VỆ (CenterBack) ===
        public decimal? AerialDuelsPct { get; set; }        // % không chiến thắng
        public int? Clearances { get; set; }                // Số lần giải nguy
        public int? Interceptions { get; set; }             // Số lần cắt bóng
        public decimal? LongPassPct { get; set; }           // % chuyền dài chính xác

        // === TIỀN VỆ PHÒNG NGỰ / TRUNG TÂM ===
        public int? BallRecoveries { get; set; }            // Số lần thu hồi bóng
        public decimal? PressuredPassPct { get; set; }      // % chuyền khi bị pressing

        // === TIỀN VỆ CÔNG / CÁNH ===
        public int? KeyPasses { get; set; }                 // Đường chuyền quyết định
        public int? ShotCreatingActions { get; set; }       // Hành động tạo cơ hội sút
        public decimal? SuccessfulDribblesPct { get; set; } // % đi bóng qua người
        public int? TouchesInPenaltyArea { get; set; }      // Chạm bóng trong vòng cấm

        // === TIỀN ĐẠO (Striker) ===
        public decimal? ShotsPer90 { get; set; }            // Số cú sút / 90 phút
        public decimal? ShotsOnTargetPct { get; set; }      // % sút trúng đích
        public decimal? ConversionRate { get; set; }        // % chuyển hóa cú sút thành bàn

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}
