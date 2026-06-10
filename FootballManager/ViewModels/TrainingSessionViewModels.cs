using FootballManager.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    // ─── Danh sách buổi tập ───
    public class TrainingSessionListItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string? Location { get; set; }
        public TrainingSessionStatus Status { get; set; }
        public string StatusLabel => ViewModelHelpers.GetSessionStatusLabel(Status);
        public string StatusBadge => ViewModelHelpers.GetSessionStatusBadge(Status);
        public string TeamName { get; set; } = string.Empty;
        public int DrillCount { get; set; }
    }

    // ─── Index view ───
    public class TrainingSessionIndexViewModel
    {
        public List<TrainingSessionListItem> Sessions { get; set; } = new();
        public List<SelectListItem> Teams { get; set; } = new();
        public int? SelectedTeamId { get; set; }
    }

    // ─── Chi tiết buổi tập ───
    public class TrainingSessionDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string? Location { get; set; }
        public string? CoachNotes { get; set; }
        public TrainingSessionStatus Status { get; set; }
        public string StatusLabel => ViewModelHelpers.GetSessionStatusLabel(Status);
        public string StatusBadge => ViewModelHelpers.GetSessionStatusBadge(Status);
        public string TeamName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public List<SessionDrillItem> Drills { get; set; } = new();
    }

    public class SessionDrillItem
    {
        public int SessionDrillId { get; set; }
        public int DrillId { get; set; }
        public string DrillName { get; set; } = string.Empty;
        public string CategoryLabel { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public int? PlannedMinutes { get; set; }
        public int? ActualMinutes { get; set; }
        public string? Notes { get; set; }
    }

    // ─── Form tạo/sửa buổi tập ───
    public class TrainingSessionFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Chọn đội")]
        [Display(Name = "Đội bóng")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Nhập tiêu đề buổi tập")]
        [MaxLength(200)]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chọn ngày giờ")]
        [Display(Name = "Ngày & giờ")]
        public DateTime ScheduledAt { get; set; } = DateTime.Now.AddDays(1);

        [MaxLength(200)]
        [Display(Name = "Địa điểm")]
        public string? Location { get; set; }

        [Display(Name = "Ghi chú HLV")]
        [MaxLength(1000)]
        public string? CoachNotes { get; set; }

        // Drills được chọn (id list)
        public List<int> SelectedDrillIds { get; set; } = new();

        // Dropdown
        public List<SelectListItem> Teams { get; set; } = new();
        public List<DrillSelectItem> AvailableDrills { get; set; } = new();
    }
}
