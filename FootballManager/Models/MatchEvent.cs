using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum MatchEventType
    {
        Goal,           // Bàn thắng
        OwnGoal,        // Phản lưới
        YellowCard,     // Thẻ vàng
        RedCard,        // Thẻ đỏ
        Substitution,   // Thay người
        Penalty,        // Phạt đền
        MissedPenalty   // Phạt đền hỏng
    }

    public class MatchEvent
    {
        public int Id { get; set; }

        [Required]
        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        [Required]
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        // Cầu thủ vào sân (chỉ dùng cho Substitution)
        public int? SubstitutePlayerId { get; set; }
        public Player? SubstitutePlayer { get; set; }

        public MatchEventType EventType { get; set; }

        [Range(1, 120)]
        public int? Minute { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
