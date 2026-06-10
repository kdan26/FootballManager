# Tài Liệu Thiết Kế Kỹ Thuật

## Football Manager — Core Features

---

## Tổng Quan

Tài liệu này mô tả thiết kế kỹ thuật cho ba tính năng cốt lõi của hệ thống Football Manager:

1. **Quản lý Đội bóng** — CRUD đầy đủ cho entity `Team`
2. **Quản lý Trận đấu** — Lên lịch, cập nhật kết quả, hủy trận cho entity `Match`
3. **Trang Tổng quan (Dashboard)** — Thống kê tổng hợp qua `StatisticsService`

Stack kỹ thuật: ASP.NET Core MVC (.NET 10) → Service Layer → EF Core 9 → SQL Server (LocalDB).  
Xác thực đã có sẵn: Cookie Authentication với 3 vai trò `Admin`, `Coach`, `Member`.

---

## Kiến Trúc

```
Browser
  │
  ▼
Controllers (TeamController, MatchController, DashboardController)
  │  ← ViewModels (input/output DTOs)
  ▼
Services (ITeamService, IMatchService, IStatisticsService)
  │  ← Domain Models (Team, Match, Attendance, Comment)
  ▼
ApplicationDbContext (EF Core 9)
  │
  ▼
SQL Server LocalDB
```

Luồng xử lý chuẩn:
- Controller nhận HTTP request, validate ModelState, gọi Service.
- Service thực hiện business logic, truy cập DB qua `ApplicationDbContext`.
- Controller nhận kết quả từ Service, map sang ViewModel, trả về View hoặc redirect.

Phân quyền được áp dụng tại tầng Controller bằng `[Authorize(Roles = "...")]`.

---

## Components và Interfaces

### ITeamService

```csharp
public interface ITeamService
{
    Task<List<TeamListItemViewModel>> GetAllTeamsAsync();
    Task<TeamDetailsViewModel?> GetTeamDetailsAsync(int id);
    Task<Team?> GetTeamByIdAsync(int id);
    Task<(bool Success, string? ErrorMessage)> CreateTeamAsync(TeamCreateViewModel model);
    Task<(bool Success, string? ErrorMessage)> UpdateTeamAsync(int id, TeamEditViewModel model);
    Task<(bool Success, string? ErrorMessage)> DeleteTeamAsync(int id);
}
```

### IMatchService

```csharp
public interface IMatchService
{
    Task<List<MatchListItemViewModel>> GetAllMatchesAsync();
    Task<MatchDetailsViewModel?> GetMatchDetailsAsync(int id);
    Task<(bool Success, string? ErrorMessage)> CreateMatchAsync(MatchCreateViewModel model);
    Task<(bool Success, string? ErrorMessage)> UpdateResultAsync(int id, MatchResultViewModel model);
    Task<(bool Success, string? ErrorMessage)> CancelMatchAsync(int id);
    Task<List<Team>> GetAllTeamsForSelectAsync();
}
```

### IStatisticsService

```csharp
public interface IStatisticsService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
}
```

---

## Data Models

### Team

```csharp
public class Team
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string HomeGround { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
    public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
}
```

### MatchStatus (Enum)

```csharp
public enum MatchStatus
{
    Scheduled,
    Completed,
    Cancelled
}
```

### Match

```csharp
public class Match
{
    public int Id { get; set; }

    [Required]
    public int HomeTeamId { get; set; }
    public Team HomeTeam { get; set; } = null!;

    [Required]
    public int AwayTeamId { get; set; }
    public Team AwayTeam { get; set; } = null!;

    [Required]
    public DateTime MatchDate { get; set; }

    [MaxLength(200)]
    public string? Venue { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
```

### Attendance

```csharp
public class Attendance
{
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsPresent { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.Now;
}
```

### Comment

```csharp
public class Comment
{
    public int Id { get; set; }

    [Required]
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required, MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

---

## ViewModels

### Team ViewModels

```csharp
// Hiển thị trong danh sách
public class TeamListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HomeGround { get; set; } = string.Empty;
    public int MatchCount { get; set; }
}

// Form tạo mới
public class TeamCreateViewModel
{
    [Required(ErrorMessage = "Tên đội không được để trống")]
    [MaxLength(100, ErrorMessage = "Tên đội không được vượt quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sân nhà không được để trống")]
    [MaxLength(200)]
    public string HomeGround { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

// Form chỉnh sửa
public class TeamEditViewModel : TeamCreateViewModel
{
    public int Id { get; set; }
}

// Trang chi tiết
public class TeamDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HomeGround { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<MatchListItemViewModel> Matches { get; set; } = new();
}
```

### Match ViewModels

```csharp
// Hiển thị trong danh sách
public class MatchListItemViewModel
{
    public int Id { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public string AwayTeamName { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public string? Venue { get; set; }
    public MatchStatus Status { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
}

// Form tạo trận đấu
public class MatchCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn đội nhà")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đội nhà")]
    public int HomeTeamId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn đội khách")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đội khách")]
    public int AwayTeamId { get; set; }

    [Required(ErrorMessage = "Ngày thi đấu không được để trống")]
    public DateTime MatchDate { get; set; }

    [MaxLength(200)]
    public string? Venue { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Dùng để populate dropdown
    public List<SelectListItem> Teams { get; set; } = new();
}

// Form cập nhật kết quả
public class MatchResultViewModel
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Số bàn thắng không được âm")]
    public int HomeScore { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Số bàn thắng không được âm")]
    public int AwayScore { get; set; }
}

// Trang chi tiết
public class MatchDetailsViewModel
{
    public int Id { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;
    public string AwayTeamName { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public string? Venue { get; set; }
    public string? Notes { get; set; }
    public MatchStatus Status { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
}
```

### Dashboard ViewModel

```csharp
public class DashboardViewModel
{
    public int TotalTeams { get; set; }
    public int TotalMatches { get; set; }
    public int CompletedMatches { get; set; }
    public int UpcomingMatchCount { get; set; }
    public List<MatchListItemViewModel> UpcomingMatches { get; set; } = new();
}
```

---

## Database Schema và EF Relationships

### ApplicationDbContext (cập nhật)

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Match → HomeTeam (restrict delete để tránh cascade conflict)
        modelBuilder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Match → AwayTeam
        modelBuilder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Attendance → Match (cascade)
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Match)
            .WithMany(m => m.Attendances)
            .HasForeignKey(a => a.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment → Match (cascade)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Match)
            .WithMany(m => m.Comments)
            .HasForeignKey(c => c.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index unique cho Team.Name
        modelBuilder.Entity<Team>()
            .HasIndex(t => t.Name)
            .IsUnique();

        // MatchStatus lưu dưới dạng string
        modelBuilder.Entity<Match>()
            .Property(m => m.Status)
            .HasConversion<string>();
    }
}
```

### Sơ đồ quan hệ

```
Users ──────────────────────────────────────────────────────────────────┐
  │                                                                      │
  │                                                                      ▼
  │                                                               Attendances
  │                                                                (UserId FK)
  │
Teams ──────────────────────────────────────────────────────────────────┐
  │ (HomeTeamId FK)                                                      │
  │ (AwayTeamId FK)                                                      │
  ▼                                                                      │
Matches ◄───────────────────────────────────────────────────────────────┘
  │
  ├──► Attendances (MatchId FK, cascade delete)
  └──► Comments    (MatchId FK, cascade delete)
```

---

## Controller Actions

### TeamController

| Action | Method | Route | Authorize | Mô tả |
|--------|--------|-------|-----------|-------|
| Index | GET | /Team | Authenticated | Danh sách đội |
| Details | GET | /Team/Details/{id} | Authenticated | Chi tiết đội |
| Create | GET | /Team/Create | Admin | Form tạo mới |
| Create | POST | /Team/Create | Admin | Xử lý tạo mới |
| Edit | GET | /Team/Edit/{id} | Admin | Form chỉnh sửa |
| Edit | POST | /Team/Edit/{id} | Admin | Xử lý chỉnh sửa |
| Delete | POST | /Team/Delete/{id} | Admin | Xóa đội |

### MatchController

| Action | Method | Route | Authorize | Mô tả |
|--------|--------|-------|-----------|-------|
| Index | GET | /Match | Authenticated | Danh sách trận |
| Details | GET | /Match/Details/{id} | Authenticated | Chi tiết trận |
| Create | GET | /Match/Create | Admin, Coach | Form tạo trận |
| Create | POST | /Match/Create | Admin, Coach | Xử lý tạo trận |
| UpdateResult | GET | /Match/UpdateResult/{id} | Admin, Coach | Form cập nhật kết quả |
| UpdateResult | POST | /Match/UpdateResult/{id} | Admin, Coach | Xử lý cập nhật kết quả |
| Cancel | POST | /Match/Cancel/{id} | Admin | Hủy trận đấu |

### DashboardController

| Action | Method | Route | Authorize | Mô tả |
|--------|--------|-------|-----------|-------|
| Index | GET | /Dashboard | Authenticated | Trang tổng quan |

---

## Cấu Trúc Views

```
Views/
├── Team/
│   ├── Index.cshtml          — Bảng danh sách đội, nút Thêm mới (Admin)
│   ├── Details.cshtml        — Thông tin đội + danh sách trận liên quan
│   ├── Create.cshtml         — Form tạo đội mới
│   └── Edit.cshtml           — Form chỉnh sửa đội
├── Match/
│   ├── Index.cshtml          — Bảng danh sách trận, badge trạng thái, tỷ số
│   ├── Details.cshtml        — Thông tin chi tiết trận
│   ├── Create.cshtml         — Form lên lịch trận (dropdown chọn đội)
│   └── UpdateResult.cshtml   — Form nhập tỷ số
└── Dashboard/
    └── Index.cshtml          — Cards thống kê + bảng 5 trận sắp diễn ra
```

Mỗi view sử dụng strongly-typed ViewModel, Bootstrap 5 cho layout và Bootstrap Icons cho icon.  
Partial view `_ValidationScriptsPartial` được include trong các form để hiển thị lỗi client-side.

---

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system — essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Danh sách đội trả về đúng số lượng và đủ thông tin

*For any* tập hợp N đội bóng được lưu trong DB, `GetAllTeamsAsync()` SHALL trả về đúng N phần tử, mỗi phần tử có `Name`, `HomeGround` và `MatchCount` hợp lệ.

**Validates: Requirements 1.1, 1.2**

---

### Property 2: Tạo đội hợp lệ luôn được lưu và có thể truy xuất lại

*For any* `TeamCreateViewModel` với `Name` không rỗng, không trùng lặp và không vượt quá 100 ký tự, sau khi `CreateTeamAsync()` thành công, `GetTeamByIdAsync()` với ID trả về SHALL trả về đội với đúng `Name` và `HomeGround` đã nhập.

**Validates: Requirements 2.2, 3.1**

---

### Property 3: Tên đội không hợp lệ bị từ chối

*For any* chuỗi `Name` rỗng, chỉ chứa whitespace, hoặc có độ dài > 100 ký tự, `ModelState` validation SHALL báo lỗi và không có đội nào được tạo mới trong DB.

**Validates: Requirements 2.3, 3.3**

---

### Property 4: Tên đội trùng lặp bị từ chối

*For any* tên đội X đã tồn tại trong DB, gọi `CreateTeamAsync()` với `Name = X` SHALL trả về `(Success: false, ErrorMessage: non-null)` và số lượng đội trong DB không thay đổi.

**Validates: Requirements 2.4**

---

### Property 5: Xóa đội không có trận đấu luôn thành công

*For any* đội bóng không có trận đấu liên quan, `DeleteTeamAsync()` SHALL trả về `(Success: true)` và đội đó không còn tồn tại trong DB sau khi xóa.

**Validates: Requirements 4.1**

---

### Property 6: Xóa đội có trận đấu bị từ chối

*For any* đội bóng có ít nhất một trận đấu liên quan (HomeTeam hoặc AwayTeam), `DeleteTeamAsync()` SHALL trả về `(Success: false, ErrorMessage: non-null)` và đội đó vẫn còn trong DB.

**Validates: Requirements 4.2**

---

### Property 7: Danh sách trận đấu được sắp xếp giảm dần theo ngày

*For any* tập hợp N trận đấu với ngày thi đấu ngẫu nhiên, `GetAllMatchesAsync()` SHALL trả về danh sách mà với mọi cặp phần tử liền kề `(i, i+1)`, `Matches[i].MatchDate >= Matches[i+1].MatchDate`.

**Validates: Requirements 6.1**

---

### Property 8: Trận đấu mới luôn được tạo với trạng thái Scheduled

*For any* `MatchCreateViewModel` hợp lệ (HomeTeamId ≠ AwayTeamId, MatchDate không null), `CreateMatchAsync()` SHALL lưu trận đấu với `Status = MatchStatus.Scheduled`.

**Validates: Requirements 7.2**

---

### Property 9: Không thể tạo trận đấu với hai đội giống nhau

*For any* đội bóng T, gọi `CreateMatchAsync()` với `HomeTeamId = T.Id` và `AwayTeamId = T.Id` SHALL trả về `(Success: false, ErrorMessage: non-null)`.

**Validates: Requirements 7.3**

---

### Property 10: Cập nhật kết quả hợp lệ chuyển trạng thái thành Completed

*For any* trận đấu có `Status = Scheduled` và bất kỳ cặp score `(homeScore, awayScore)` với cả hai >= 0, `UpdateResultAsync()` SHALL lưu đúng score và chuyển `Status = MatchStatus.Completed`.

**Validates: Requirements 8.2**

---

### Property 11: Không thể cập nhật kết quả hoặc hủy trận đã kết thúc/đã hủy

*For any* trận đấu có `Status = Completed` hoặc `Status = Cancelled`, cả `UpdateResultAsync()` và `CancelMatchAsync()` SHALL trả về `(Success: false, ErrorMessage: non-null)` và `Status` của trận không thay đổi.

**Validates: Requirements 8.4, 10.2**

---

### Property 12: Hủy trận Scheduled luôn thành công

*For any* trận đấu có `Status = Scheduled`, `CancelMatchAsync()` SHALL trả về `(Success: true)` và `Status` của trận chuyển thành `MatchStatus.Cancelled`.

**Validates: Requirements 10.1**

---

### Property 13: StatisticsService đếm đúng số đội và số trận theo trạng thái

*For any* tập hợp đội bóng và trận đấu trong DB, `GetDashboardDataAsync()` SHALL trả về:
- `TotalTeams` = tổng số bản ghi trong bảng Teams
- `CompletedMatches` = số trận có `Status = Completed`
- `UpcomingMatchCount` = số trận có `Status = Scheduled` và `MatchDate >= DateTime.Today`

**Validates: Requirements 11.2, 11.3, 11.4**

---

### Property 14: Dashboard trả về tối đa 5 trận sắp diễn ra, sắp xếp tăng dần

*For any* tập hợp trận đấu trong DB, `GetDashboardDataAsync()` SHALL trả về `UpcomingMatches` với:
- `Count <= 5`
- Chỉ chứa trận có `Status = Scheduled` và `MatchDate >= DateTime.Today`
- Được sắp xếp tăng dần theo `MatchDate`

**Validates: Requirements 11.5**

---

## Xử Lý Lỗi

### Validation Errors (ModelState)
- Xảy ra tại tầng Controller trước khi gọi Service.
- Controller kiểm tra `ModelState.IsValid`, nếu false thì trả về View với model hiện tại để hiển thị lỗi inline.
- Data Annotations trên ViewModel xử lý: required, maxlength, range.

### Business Logic Errors (Service Layer)
- Service trả về tuple `(bool Success, string? ErrorMessage)` thay vì throw exception.
- Controller nhận kết quả, nếu `Success = false` thì thêm lỗi vào `ModelState[""]` và trả về View.
- Các lỗi business: tên đội trùng, xóa đội có trận đấu, cập nhật trận đã kết thúc, hai đội trùng nhau.

### Not Found (404)
- Service trả về `null` khi không tìm thấy entity theo ID.
- Controller kiểm tra null và trả về `NotFound()`.

### Authorization Errors (403)
- Xử lý bởi `[Authorize(Roles = "...")]` attribute tại Controller/Action.
- Cookie Authentication middleware tự động redirect về `/Auth/Login` cho unauthenticated users.
- `AccessDeniedPath` được cấu hình trong `Program.cs` để xử lý 403.

### Database Errors
- EF Core exceptions được để bubble up tự nhiên trong môi trường Development.
- Trong Production, middleware `UseExceptionHandler("/Home/Error")` đã được cấu hình.

---

## Testing Strategy

### Phân loại test

Dựa trên phân tích acceptance criteria, feature này phù hợp với property-based testing cho tầng Service (pure business logic), kết hợp với example-based tests cho Controller actions và UI rendering.

### Property-Based Testing (Service Layer)

Thư viện: **FsCheck** (cho .NET, tích hợp tốt với xUnit).

Mỗi property test chạy tối thiểu 100 iterations.  
Tag format: `// Feature: football-manager-core-features, Property {N}: {property_text}`

Các property cần implement (tương ứng với Correctness Properties ở trên):

- **Property 1**: `GetAllTeamsAsync()` trả về đúng N đội với đủ thông tin
- **Property 2**: Create → GetById round-trip bảo toàn dữ liệu
- **Property 3**: Input không hợp lệ bị từ chối bởi validation
- **Property 4**: Tên trùng lặp bị từ chối
- **Property 5**: Xóa đội không có trận đấu luôn thành công
- **Property 6**: Xóa đội có trận đấu bị từ chối
- **Property 7**: Danh sách trận được sắp xếp giảm dần
- **Property 8**: Trận mới luôn có status Scheduled
- **Property 9**: Hai đội giống nhau bị từ chối
- **Property 10**: UpdateResult với score >= 0 → status Completed
- **Property 11**: UpdateResult/Cancel trên trận terminal bị từ chối
- **Property 12**: Cancel trận Scheduled luôn thành công
- **Property 13**: Statistics đếm đúng theo trạng thái
- **Property 14**: Dashboard trả về <= 5 trận, sắp xếp đúng

### Example-Based Unit Tests (xUnit)

Tập trung vào:
- Controller actions: kiểm tra redirect, ViewData, HTTP status codes
- Edge cases: empty list, ID không tồn tại, DB rỗng
- UI rendering: form có đủ fields, dropdown có dữ liệu

### Integration Tests

- Kiểm tra EF Core relationships (cascade delete, restrict delete)
- Kiểm tra unique index trên `Team.Name`
- Kiểm tra authentication/authorization attributes được áp dụng đúng

### Smoke Tests

- `[Authorize]` attribute có mặt trên tất cả controllers
- `[Authorize(Roles = "Admin")]` trên Create/Edit/Delete của Team
- `[Authorize(Roles = "Admin,Coach")]` trên Create/UpdateResult của Match
- `[Authorize(Roles = "Admin")]` trên Cancel của Match
