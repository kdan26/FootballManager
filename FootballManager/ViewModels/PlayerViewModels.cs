using FootballManager.Models;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    public class PlayerListItemViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public PlayerPosition Position { get; set; }
        public string PositionLabel => ViewModelHelpers.GetPositionLabel(Position);
        public PlayerHealthStatus HealthStatus { get; set; } = PlayerHealthStatus.Fit;
        public bool IsActive { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }

    public class PlayerCreateViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MaxLength(100)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Range(1, 99, ErrorMessage = "Số áo phải từ 1 đến 99")]
        [Display(Name = "Số áo")]
        public int? JerseyNumber { get; set; }

        [Display(Name = "Vị trí")]
        public PlayerPosition Position { get; set; } = PlayerPosition.CentralMid;

        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(200)]
        [Display(Name = "Quốc tịch")]
        public string? Nationality { get; set; }

        [MaxLength(500)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;

        [Required]
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
    }

    public class PlayerEditViewModel : PlayerCreateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Trạng thái sức khỏe")]
        public PlayerHealthStatus HealthStatus { get; set; } = PlayerHealthStatus.Fit;

        [MaxLength(500)]
        [Display(Name = "Ghi chú sức khỏe")]
        public string? HealthNote { get; set; }

        [Display(Name = "Ngày dự kiến trở lại")]
        public DateTime? ExpectedReturnDate { get; set; }
    }

    public class PlayerDetailsViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public PlayerPosition Position { get; set; }
        public string PositionLabel => ViewModelHelpers.GetPositionLabel(Position);
        public PlayerHealthStatus HealthStatus { get; set; } = PlayerHealthStatus.Fit;
        public string? HealthNote { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int? Age => DateOfBirth.HasValue
            ? (int)((DateTime.Today - DateOfBirth.Value).TotalDays / 365.25)
            : null;
    }
}
