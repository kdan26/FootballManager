using FootballManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    public class PlayerMatchStatsFormViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public PlayerPosition Position { get; set; }
        public int MatchId { get; set; }
        public string MatchLabel { get; set; } = string.Empty;

        // Chung
        [Display(Name = "Số phút thi đấu")]
        [Range(0, 120)]
        public int MinutesPlayed { get; set; }

        [Display(Name = "% Chuyền chính xác")]
        [Range(0, 100)]
        public decimal? PassCompletionPct { get; set; }

        // Thủ môn
        [Display(Name = "PSxG +/-")]
        public decimal? PsxGDiff { get; set; }

        [Display(Name = "% Cứu thua")]
        [Range(0, 100)]
        public decimal? SavePct { get; set; }

        [Display(Name = "% Chuyền dài chính xác")]
        [Range(0, 100)]
        public decimal? LaunchAccuracyPct { get; set; }

        // Hậu vệ biên
        [Display(Name = "Số quả tạt chính xác")]
        public int? AccurateCrosses { get; set; }

        [Display(Name = "% Tắc bóng thành công")]
        [Range(0, 100)]
        public decimal? TacklesWonPct { get; set; }

        [Display(Name = "Quãng đường di chuyển (km)")]
        public decimal? DistanceCovered { get; set; }

        // Trung vệ
        [Display(Name = "% Không chiến thắng")]
        [Range(0, 100)]
        public decimal? AerialDuelsPct { get; set; }

        [Display(Name = "Số lần giải nguy")]
        public int? Clearances { get; set; }

        [Display(Name = "Số lần cắt bóng")]
        public int? Interceptions { get; set; }

        [Display(Name = "% Chuyền dài chính xác")]
        [Range(0, 100)]
        public decimal? LongPassPct { get; set; }

        // Tiền vệ phòng ngự
        [Display(Name = "Số lần thu hồi bóng")]
        public int? BallRecoveries { get; set; }

        [Display(Name = "% Chuyền khi bị pressing")]
        [Range(0, 100)]
        public decimal? PressuredPassPct { get; set; }

        // Tiền vệ công / cánh
        [Display(Name = "Đường chuyền quyết định")]
        public int? KeyPasses { get; set; }

        [Display(Name = "Hành động tạo cơ hội sút")]
        public int? ShotCreatingActions { get; set; }

        [Display(Name = "% Đi bóng qua người")]
        [Range(0, 100)]
        public decimal? SuccessfulDribblesPct { get; set; }

        [Display(Name = "Chạm bóng trong vòng cấm")]
        public int? TouchesInPenaltyArea { get; set; }

        // Tiền đạo
        [Display(Name = "Số cú sút / 90 phút")]
        public decimal? ShotsPer90 { get; set; }

        [Display(Name = "% Sút trúng đích")]
        [Range(0, 100)]
        public decimal? ShotsOnTargetPct { get; set; }

        [Display(Name = "Tỷ lệ chuyển hóa bàn thắng (%)")]
        [Range(0, 100)]
        public decimal? ConversionRate { get; set; }

        [Display(Name = "Ghi chú")]
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Dropdown chọn trận
        public List<SelectListItem> Matches { get; set; } = new();
    }

    public class PlayerMatchStatsListItem
    {
        public int Id { get; set; }         // ID của PlayerMatchStats record
        public int MatchId { get; set; }    // ID của Match — dùng để link edit
        public string MatchLabel { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int MinutesPlayed { get; set; }
        public decimal? PassCompletionPct { get; set; }
        public string Summary { get; set; } = string.Empty;
    }
}
