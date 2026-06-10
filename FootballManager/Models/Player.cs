using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum PlayerPosition
    {
        GoalKeeper,      // Thủ môn
        CenterBack,      // Trung vệ
        FullBack,        // Hậu vệ biên
        DefensiveMid,    // Tiền vệ phòng ngự
        CentralMid,      // Tiền vệ trung tâm
        AttackingMid,    // Tiền vệ công
        Winger,          // Cầu thủ chạy cánh
        Striker          // Tiền đạo cắm
    }

    public enum PlayerHealthStatus
    {
        Fit,        // Sẵn sàng thi đấu
        Injured,    // Chấn thương
        Sick,       // Ốm đau
        Suspended,  // Treo giò
        Recovering  // Đang hồi phục
    }

    public class Player
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Range(1, 99)]
        public int? JerseyNumber { get; set; }

        public PlayerPosition Position { get; set; } = PlayerPosition.CentralMid;

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(200)]
        public string? Nationality { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        // Trạng thái sức khỏe
        public PlayerHealthStatus HealthStatus { get; set; } = PlayerHealthStatus.Fit;

        [MaxLength(500)]
        public string? HealthNote { get; set; } // Ghi chú chấn thương/ốm

        public DateTime? ExpectedReturnDate { get; set; } // Ngày dự kiến trở lại

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // FK → Team
        [Required]
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public ICollection<PlayerAttendance> Attendances { get; set; } = new List<PlayerAttendance>();
        public ICollection<MatchEvent> Events { get; set; } = new List<MatchEvent>();
        public ICollection<PlayerMatchStats> MatchStats { get; set; } = new List<PlayerMatchStats>();
        public ICollection<PlayerTrainingStats> TrainingStats { get; set; } = new List<PlayerTrainingStats>();
    }
}
