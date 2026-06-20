using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    public class UserListItem
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string RoleBadge => Role switch
        {
            "Admin"   => "bg-danger",
            "Coach"   => "bg-success",
            "Analyst" => "bg-primary",
            "Player"  => "bg-warning text-dark",
            _         => "bg-secondary"
        };
        public string RoleLabel => Role switch
        {
            "Admin"   => "Quản trị",
            "Coach"   => "Huấn luyện viên",
            "Analyst" => "Chuyên viên phân tích",
            "Player"  => "Cầu thủ",
            "Member"  => "Thành viên",
            _         => Role
        };
        public bool IsActive { get; set; }
        public string? PlayerName { get; set; }   // null nếu không phải role Player
        public DateTime CreatedAt { get; set; }
    }

    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Nhập họ tên")]
        [MaxLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập tên đăng nhập")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9_\.]+$", ErrorMessage = "Chỉ dùng chữ, số, _ hoặc .")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Member";

        [Display(Name = "Liên kết cầu thủ (nếu role Player)")]
        public int? PlayerId { get; set; }

        // Dropdown danh sách cầu thủ chưa có tài khoản
        public List<PlayerSelectItem> AvailablePlayers { get; set; } = new();
    }

    public class UserEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nhập họ tên")]
        [MaxLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Member";

        [Display(Name = "Liên kết cầu thủ")]
        public int? PlayerId { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        public List<PlayerSelectItem> AvailablePlayers { get; set; } = new();
    }

    public class ChangePasswordViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 ký tự")]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu")]
        [Compare(nameof(NewPassword), ErrorMessage = "Mật khẩu không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class PlayerSelectItem
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
    }

    /// <summary>Trang cá nhân cầu thủ — role Player</summary>
    public class PlayerPortalViewModel
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PositionLabel { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public string HealthStatusLabel { get; set; } = string.Empty;
        public string HealthBadge { get; set; } = string.Empty;

        // Lịch buổi tập sắp tới (7 ngày)
        public List<UpcomingSessionItem> UpcomingSessions { get; set; } = new();

        // Điểm danh gần nhất
        public List<RecentAttendanceItem> RecentAttendances { get; set; } = new();

        // Chỉ số tập luyện gần nhất
        public decimal? LastCoachRating { get; set; }
        public decimal? AvgCoachRating { get; set; }
    }

    public class UpcomingSessionItem
    {
        public int SessionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string? Location { get; set; }
        public string MyStatus { get; set; } = string.Empty;    // Present/Late/Absent/...
        public string MyStatusBadge { get; set; } = string.Empty;
    }

    public class RecentAttendanceItem
    {
        public string SessionTitle { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusBadge { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
