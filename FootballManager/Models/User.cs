using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    /// <summary>
    /// Roles:
    /// Admin    — Toàn quyền, quản lý tài khoản
    /// Coach    — HLV: nhập chỉ số, chiến thuật, buổi tập, điểm danh
    /// Analyst  — Chuyên viên phân tích: nhập chỉ số thi đấu, xem báo cáo
    /// Player   — Cầu thủ: xem dữ liệu cá nhân, lịch tập, xác nhận điểm danh
    /// Member   — Chỉ xem (trợ lý, khách)
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Member"; // Admin | Coach | Analyst | Player | Member

        public bool IsActive { get; set; } = true;

        // Dành riêng cho role Player — link tới Player record
        public int? PlayerId { get; set; }
        public Player? PlayerProfile { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
