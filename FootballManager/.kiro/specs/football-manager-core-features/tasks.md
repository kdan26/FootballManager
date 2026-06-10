# Kế Hoạch Triển Khai: Football Manager — Core Features

## Tổng Quan

Triển khai ba tính năng cốt lõi trên nền ASP.NET Core MVC (.NET 10) đã có sẵn Cookie Authentication:
1. Quản lý Đội bóng (Team CRUD)
2. Quản lý Trận đấu (Match scheduling + results)
3. Trang Tổng quan (Dashboard statistics)

Stack: Controllers → Services → EF Core 9 → SQL Server LocalDB

---

## Tasks

- [x] 1. Hoàn thiện Data Models
  - [x] 1.1 Cập nhật `Models/Team.cs` với đầy đủ properties, Data Annotations và navigation properties
    - Thêm: `Id`, `Name` (Required, MaxLength 100), `HomeGround` (Required, MaxLength 200), `Description` (MaxLength 500), `CreatedAt`, `HomeMatches`, `AwayMatches`
    - _Yêu cầu: 1.2, 2.1, 3.1_

  - [x] 1.2 Cập nhật `Models/Match.cs` với enum `MatchStatus` và đầy đủ properties
    - Tạo enum `MatchStatus { Scheduled, Completed, Cancelled }` trong cùng file hoặc file riêng `Models/MatchStatus.cs`
    - Thêm: `Id`, `HomeTeamId`, `HomeTeam`, `AwayTeamId`, `AwayTeam`, `MatchDate`, `Venue`, `Notes`, `Status`, `HomeScore`, `AwayScore`, `CreatedAt`, `Attendances`, `Comments`
    - _Yêu cầu: 6.2, 6.3, 7.2, 8.2_

  - [x] 1.3 Cập nhật `Models/Attendance.cs` và `Models/Comment.cs` với đầy đủ properties
    - `Attendance`: `Id`, `MatchId`, `Match`, `UserId`, `User`, `IsPresent`, `RecordedAt`
    - `Comment`: `Id`, `MatchId`, `Match`, `UserId`, `User`, `Content` (Required, MaxLength 1000), `CreatedAt`
    - _Yêu cầu: (navigation properties cho Match)_

- [x] 2. Cập nhật ApplicationDbContext và Migration
  - [x] 2.1 Cập nhật `Data/ApplicationDbContext.cs` — thêm DbSets và cấu hình relationships
    - Thêm `DbSet<Team>`, `DbSet<Match>`, `DbSet<Attendance>`, `DbSet<Comment>`
    - Cấu hình `OnModelCreating`: Match→HomeTeam (Restrict), Match→AwayTeam (Restrict), Attendance→Match (Cascade), Comment→Match (Cascade)
    - Thêm unique index cho `Team.Name`
    - Cấu hình `MatchStatus` lưu dưới dạng string
    - _Yêu cầu: 2.4, 4.2, 12.1_

  - [x] 2.2 Tạo migration mới và cập nhật database schema
    - Chạy `dotnet ef migrations add AddCoreFeatures` để tạo migration
    - Chạy `dotnet ef database update` để áp dụng schema mới
    - _Yêu cầu: (database schema)_

- [x] 3. Tạo ViewModels
  - [x] 3.1 Tạo `ViewModels/TeamViewModels.cs` với 4 ViewModels
    - `TeamListItemViewModel`: `Id`, `Name`, `HomeGround`, `MatchCount`
    - `TeamCreateViewModel`: `Name` (Required, MaxLength 100), `HomeGround` (Required, MaxLength 200), `Description`
    - `TeamEditViewModel : TeamCreateViewModel`: thêm `Id`
    - `TeamDetailsViewModel`: `Id`, `Name`, `HomeGround`, `Description`, `List<MatchListItemViewModel> Matches`
    - _Yêu cầu: 1.2, 2.1, 3.1, 5.1_

  - [x] 3.2 Tạo `ViewModels/MatchViewModels.cs` với 4 ViewModels
    - `MatchListItemViewModel`: `Id`, `HomeTeamName`, `AwayTeamName`, `MatchDate`, `Venue`, `Status`, `HomeScore`, `AwayScore`
    - `MatchCreateViewModel`: `HomeTeamId` (Required, Range 1+), `AwayTeamId` (Required, Range 1+), `MatchDate` (Required), `Venue`, `Notes`, `List<SelectListItem> Teams`
    - `MatchResultViewModel`: `HomeScore` (Range 0+), `AwayScore` (Range 0+)
    - `MatchDetailsViewModel`: `Id`, `HomeTeamName`, `AwayTeamName`, `MatchDate`, `Venue`, `Notes`, `Status`, `HomeScore`, `AwayScore`
    - _Yêu cầu: 6.2, 6.3, 7.1, 8.1, 9.1_

  - [x] 3.3 Tạo `ViewModels/DashboardViewModel.cs`
    - `DashboardViewModel`: `TotalTeams`, `TotalMatches`, `CompletedMatches`, `UpcomingMatchCount`, `List<MatchListItemViewModel> UpcomingMatches`
    - _Yêu cầu: 11.1, 11.5_

- [x] 4. Triển khai Service Interfaces và Implementations
  - [x] 4.1 Tạo `Services/ITeamService.cs` — định nghĩa interface
    - Khai báo: `GetAllTeamsAsync()`, `GetTeamDetailsAsync(int id)`, `GetTeamByIdAsync(int id)`, `CreateTeamAsync(model)`, `UpdateTeamAsync(int id, model)`, `DeleteTeamAsync(int id)`
    - _Yêu cầu: 1.1, 2.2, 3.2, 4.1, 5.1_

  - [x] 4.2 Triển khai `Services/TeamService.cs`
    - `GetAllTeamsAsync()`: query Teams, include HomeMatches + AwayMatches, map sang `TeamListItemViewModel` với `MatchCount = HomeMatches.Count + AwayMatches.Count`
    - `GetTeamDetailsAsync(int id)`: query Team by id, include matches, map sang `TeamDetailsViewModel`
    - `GetTeamByIdAsync(int id)`: trả về `Team?` entity
    - `CreateTeamAsync()`: kiểm tra tên trùng (AnyAsync), nếu trùng trả về `(false, "Tên đội đã tồn tại")`, ngược lại Add + SaveChanges
    - `UpdateTeamAsync()`: tìm team, cập nhật fields, SaveChanges; trả về 404 nếu không tìm thấy
    - `DeleteTeamAsync()`: kiểm tra có trận đấu liên quan không (HomeMatches hoặc AwayMatches), nếu có trả về `(false, "Không thể xóa đội đang có trận đấu liên quan")`, ngược lại Remove + SaveChanges
    - _Yêu cầu: 1.1, 1.3, 2.2, 2.4, 3.2, 4.1, 4.2, 5.1_

  - [ ]* 4.3 Viết property test cho TeamService — Property 1: GetAllTeamsAsync trả về đúng số lượng
    - **Property 1: Danh sách đội trả về đúng số lượng và đủ thông tin**
    - **Validates: Yêu cầu 1.1, 1.2**

  - [ ]* 4.4 Viết property test cho TeamService — Property 2: Create → GetById round-trip
    - **Property 2: Tạo đội hợp lệ luôn được lưu và có thể truy xuất lại**
    - **Validates: Yêu cầu 2.2, 3.1**

  - [ ]* 4.5 Viết property test cho TeamService — Property 4: Tên trùng lặp bị từ chối
    - **Property 4: Tên đội trùng lặp bị từ chối**
    - **Validates: Yêu cầu 2.4**

  - [ ]* 4.6 Viết property test cho TeamService — Property 5 & 6: Xóa đội
    - **Property 5: Xóa đội không có trận đấu luôn thành công**
    - **Property 6: Xóa đội có trận đấu bị từ chối**
    - **Validates: Yêu cầu 4.1, 4.2**

  - [x] 4.7 Tạo `Services/IMatchService.cs` — định nghĩa interface
    - Khai báo: `GetAllMatchesAsync()`, `GetMatchDetailsAsync(int id)`, `CreateMatchAsync(model)`, `UpdateResultAsync(int id, model)`, `CancelMatchAsync(int id)`, `GetAllTeamsForSelectAsync()`
    - _Yêu cầu: 6.1, 7.2, 8.2, 9.1, 10.1_

  - [x] 4.8 Triển khai `Services/MatchService.cs`
    - `GetAllMatchesAsync()`: query Matches include HomeTeam + AwayTeam, sắp xếp giảm dần theo `MatchDate`, map sang `MatchListItemViewModel`
    - `GetMatchDetailsAsync(int id)`: query Match by id include teams, map sang `MatchDetailsViewModel`
    - `CreateMatchAsync()`: kiểm tra `HomeTeamId == AwayTeamId` → trả về `(false, "Đội nhà và đội khách không được trùng nhau")`; ngược lại Add + SaveChanges với `Status = Scheduled`
    - `UpdateResultAsync()`: tìm match, kiểm tra status không phải Scheduled → trả về `(false, "Trận đấu này đã kết thúc hoặc đã hủy")`; cập nhật scores + Status = Completed + SaveChanges
    - `CancelMatchAsync()`: tìm match, kiểm tra status không phải Scheduled → trả về `(false, "Không thể hủy trận đấu đã kết thúc hoặc đã hủy trước đó")`; cập nhật Status = Cancelled + SaveChanges
    - `GetAllTeamsForSelectAsync()`: trả về `List<Team>` để populate dropdown
    - _Yêu cầu: 6.1, 7.2, 7.3, 8.2, 8.4, 9.1, 10.1, 10.2_

  - [ ]* 4.9 Viết property test cho MatchService — Property 7: Sắp xếp giảm dần
    - **Property 7: Danh sách trận đấu được sắp xếp giảm dần theo ngày**
    - **Validates: Yêu cầu 6.1**

  - [ ]* 4.10 Viết property test cho MatchService — Property 8 & 9: Tạo trận
    - **Property 8: Trận đấu mới luôn được tạo với trạng thái Scheduled**
    - **Property 9: Không thể tạo trận đấu với hai đội giống nhau**
    - **Validates: Yêu cầu 7.2, 7.3**

  - [ ]* 4.11 Viết property test cho MatchService — Property 10, 11, 12: Cập nhật và hủy trận
    - **Property 10: Cập nhật kết quả hợp lệ chuyển trạng thái thành Completed**
    - **Property 11: Không thể cập nhật kết quả hoặc hủy trận đã kết thúc/đã hủy**
    - **Property 12: Hủy trận Scheduled luôn thành công**
    - **Validates: Yêu cầu 8.2, 8.4, 10.1, 10.2**

  - [x] 4.12 Tạo `Services/IStatisticsService.cs` và triển khai `Services/StatisticsService.cs`
    - Interface: `GetDashboardDataAsync()` trả về `DashboardViewModel`
    - Implementation: đếm TotalTeams, TotalMatches, CompletedMatches (Status = Completed), UpcomingMatchCount (Status = Scheduled AND MatchDate >= DateTime.Today)
    - Lấy tối đa 5 trận Scheduled sắp diễn ra, sắp xếp tăng dần theo MatchDate
    - _Yêu cầu: 11.1, 11.2, 11.3, 11.4, 11.5_

  - [ ]* 4.13 Viết property test cho StatisticsService — Property 13 & 14: Dashboard
    - **Property 13: StatisticsService đếm đúng số đội và số trận theo trạng thái**
    - **Property 14: Dashboard trả về tối đa 5 trận sắp diễn ra, sắp xếp tăng dần**
    - **Validates: Yêu cầu 11.2, 11.3, 11.4, 11.5**

- [x] 5. Checkpoint — Đảm bảo tất cả services build thành công
  - Đảm bảo tất cả tests pass, hỏi người dùng nếu có thắc mắc.

- [x] 6. Đăng ký DI và cấu hình Program.cs
  - [x] 6.1 Cập nhật `Program.cs` — đăng ký services vào DI container
    - Thêm: `builder.Services.AddScoped<ITeamService, TeamService>()`
    - Thêm: `builder.Services.AddScoped<IMatchService, MatchService>()`
    - Thêm: `builder.Services.AddScoped<IStatisticsService, StatisticsService>()`
    - _Yêu cầu: 12.1_

- [x] 7. Triển khai TeamController
  - [x] 7.1 Cập nhật `Controllers/TeamController.cs` — kế thừa Controller, inject ITeamService
    - Thêm `[Authorize]` ở class level
    - Implement `Index()`: gọi `GetAllTeamsAsync()`, trả về View
    - Implement `Details(int id)`: gọi `GetTeamDetailsAsync()`, trả về NotFound nếu null
    - _Yêu cầu: 1.1, 1.4, 5.1, 5.2, 12.3_

  - [x] 7.2 Thêm Create, Edit, Delete actions vào TeamController
    - `Create GET`: `[Authorize(Roles = "Admin")]`, trả về View với `TeamCreateViewModel`
    - `Create POST`: `[Authorize(Roles = "Admin")]`, validate ModelState, gọi `CreateTeamAsync()`, xử lý lỗi business
    - `Edit GET`: `[Authorize(Roles = "Admin")]`, gọi `GetTeamByIdAsync()`, map sang `TeamEditViewModel`
    - `Edit POST`: `[Authorize(Roles = "Admin")]`, validate ModelState, gọi `UpdateTeamAsync()`
    - `Delete POST`: `[Authorize(Roles = "Admin")]`, gọi `DeleteTeamAsync()`, xử lý lỗi business
    - _Yêu cầu: 2.1, 2.2, 2.3, 2.4, 2.5, 3.1, 3.2, 3.3, 3.4, 4.1, 4.2, 4.3, 4.4, 12.1, 12.4_

- [x] 8. Triển khai MatchController
  - [x] 8.1 Cập nhật `Controllers/MatchController.cs` — kế thừa Controller, inject IMatchService
    - Thêm `[Authorize]` ở class level
    - Implement `Index()`: gọi `GetAllMatchesAsync()`, trả về View
    - Implement `Details(int id)`: gọi `GetMatchDetailsAsync()`, trả về NotFound nếu null
    - _Yêu cầu: 6.1, 6.2, 6.3, 6.4, 6.5, 9.1, 9.2, 9.3_

  - [x] 8.2 Thêm Create, UpdateResult, Cancel actions vào MatchController
    - `Create GET`: `[Authorize(Roles = "Admin,Coach")]`, gọi `GetAllTeamsForSelectAsync()`, populate dropdown
    - `Create POST`: `[Authorize(Roles = "Admin,Coach")]`, validate ModelState, gọi `CreateMatchAsync()`
    - `UpdateResult GET`: `[Authorize(Roles = "Admin,Coach")]`, tìm match, trả về form
    - `UpdateResult POST`: `[Authorize(Roles = "Admin,Coach")]`, validate ModelState, gọi `UpdateResultAsync()`
    - `Cancel POST`: `[Authorize(Roles = "Admin")]`, gọi `CancelMatchAsync()`, xử lý lỗi business
    - _Yêu cầu: 7.1, 7.2, 7.3, 7.4, 7.5, 8.1, 8.2, 8.3, 8.4, 10.1, 10.2, 10.3, 12.2, 12.4_

- [x] 9. Triển khai DashboardController
  - [x] 9.1 Cập nhật `Controllers/DashboardController.cs` — inject IStatisticsService
    - Thêm `[Authorize]` ở class level
    - Implement `Index()`: gọi `GetDashboardDataAsync()`, trả về View với `DashboardViewModel`
    - _Yêu cầu: 11.1, 11.5, 11.6_

- [x] 10. Triển khai Views — Team
  - [x] 10.1 Cập nhật `Views/Team/Index.cshtml` — danh sách đội bóng
    - Model: `IEnumerable<TeamListItemViewModel>`
    - Hiển thị bảng: Tên đội, Sân nhà, Số trận; nút Details cho tất cả
    - Hiển thị nút "Thêm đội mới" chỉ khi `User.IsInRole("Admin")`
    - Hiển thị thông báo "Chưa có đội bóng nào" khi danh sách rỗng
    - _Yêu cầu: 1.1, 1.2, 1.3_

  - [x] 10.2 Cập nhật `Views/Team/Details.cshtml` — chi tiết đội
    - Model: `TeamDetailsViewModel`
    - Hiển thị: Tên đội, Sân nhà, Mô tả, danh sách trận liên quan
    - _Yêu cầu: 5.1_

  - [x] 10.3 Cập nhật `Views/Team/Create.cshtml` — form tạo đội mới
    - Model: `TeamCreateViewModel`
    - Form fields: Tên đội, Sân nhà, Mô tả; nút Submit và Cancel
    - Include `_ValidationScriptsPartial`
    - _Yêu cầu: 2.1, 2.3_

  - [x] 10.4 Cập nhật `Views/Team/Edit.cshtml` — form chỉnh sửa đội
    - Model: `TeamEditViewModel`
    - Form fields điền sẵn dữ liệu hiện tại; nút Submit và Cancel
    - Include `_ValidationScriptsPartial`
    - _Yêu cầu: 3.1, 3.3_

- [x] 11. Triển khai Views — Match
  - [x] 11.1 Cập nhật `Views/Match/Index.cshtml` — danh sách trận đấu
    - Model: `IEnumerable<MatchListItemViewModel>`
    - Hiển thị: Đội nhà vs Đội khách, Ngày, Địa điểm, badge trạng thái (Bootstrap badge màu theo status)
    - Hiển thị tỷ số khi `Status == Completed`
    - Hiển thị thông báo "Chưa có trận đấu nào được lên lịch" khi rỗng
    - Nút "Lên lịch trận mới" chỉ khi Admin hoặc Coach
    - _Yêu cầu: 6.1, 6.2, 6.3, 6.4_

  - [x] 11.2 Cập nhật `Views/Match/Details.cshtml` — chi tiết trận đấu
    - Model: `MatchDetailsViewModel`
    - Hiển thị đầy đủ thông tin; hiển thị tỷ số khi `Status == Completed`
    - Nút "Cập nhật kết quả" (Admin/Coach, chỉ khi Scheduled), nút "Hủy trận" (Admin, chỉ khi Scheduled)
    - _Yêu cầu: 9.1, 9.2_

  - [x] 11.3 Cập nhật `Views/Match/Create.cshtml` — form lên lịch trận
    - Model: `MatchCreateViewModel`
    - Dropdown chọn Đội nhà, Đội khách (từ `model.Teams`); DateTime picker cho MatchDate; Venue, Notes
    - Include `_ValidationScriptsPartial`
    - _Yêu cầu: 7.1, 7.4_

  - [x] 11.4 Tạo `Views/Match/UpdateResult.cshtml` — form cập nhật kết quả
    - Model: `MatchResultViewModel`
    - Input số bàn thắng đội nhà và đội khách (min=0)
    - Include `_ValidationScriptsPartial`
    - _Yêu cầu: 8.1, 8.3_

- [x] 12. Triển khai Views — Dashboard
  - [x] 12.1 Tạo `Views/Dashboard/Index.cshtml` — trang tổng quan
    - Model: `DashboardViewModel`
    - 4 cards thống kê: Tổng đội, Tổng trận, Trận hoàn thành, Trận sắp diễn ra (Bootstrap cards với Bootstrap Icons)
    - Bảng tối đa 5 trận sắp diễn ra với tên đội, ngày, địa điểm
    - _Yêu cầu: 11.1, 11.5_

- [x] 13. Cập nhật Navigation và Layout
  - [x] 13.1 Cập nhật `Views/Shared/_Layout.cshtml` — thêm link Dashboard vào navbar
    - Thêm nav-item link đến `/Dashboard` với icon `bi-speedometer2`
    - _Yêu cầu: 11.6_

  - [x] 13.2 Cập nhật `Controllers/HomeController.cs` — redirect về Dashboard sau login
    - Sau khi xác thực thành công, redirect về `/Dashboard` thay vì `/Home`
    - _Yêu cầu: 11.6_

- [x] 14. Checkpoint cuối — Đảm bảo toàn bộ tính năng hoạt động
  - Đảm bảo tất cả tests pass, build thành công, hỏi người dùng nếu có thắc mắc.

---

## Ghi Chú

- Tasks đánh dấu `*` là optional, có thể bỏ qua để triển khai nhanh MVP
- Property tests sử dụng thư viện **FsCheck** + **xUnit** (cần tạo project test riêng)
- Mỗi task tham chiếu đến yêu cầu cụ thể để đảm bảo traceability
- Thứ tự thực hiện: Models → DbContext/Migration → ViewModels → Services → DI → Controllers → Views
