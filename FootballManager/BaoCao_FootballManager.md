# BÁO CÁO DỰ ÁN: FOOTBALL MANAGER

## 1. TỔNG QUAN

**Football Manager** là ứng dụng web quản lý đội bóng được xây dựng bằng **ASP.NET Core MVC (.NET 10)**, sử dụng **Entity Framework Core** kết nối **SQL Server LocalDB**. Ứng dụng hỗ trợ các vai trò từ quản trị viên đến cầu thủ, cung cấp công cụ quản lý toàn diện cho một đội bóng.

### Công nghệ sử dụng

| Thành phần      | Công nghệ                          |
|-----------------|------------------------------------|
| Backend         | ASP.NET Core MVC .NET 10           |
| ORM             | Entity Framework Core 9            |
| Database        | SQL Server LocalDB                 |
| Authentication  | Cookie Authentication (CookieAuth) |
| Frontend        | Bootstrap 5.3, Bootstrap Icons     |
| Charts          | Chart.js 4.4                       |
| Calendar        | FullCalendar.js 6.1                |
| Password Hash   | SHA-256                            |

---

## 2. KIẾN TRÚC HỆ THỐNG

```
FootballManager/
├── Controllers/        # Xử lý HTTP request, điều phối luồng
├── Services/           # Business logic (interface + implementation)
├── Models/             # Entity Framework models (ánh xạ DB)
├── ViewModels/         # DTO truyền data từ Controller → View
├── Views/              # Razor views (.cshtml)
├── Data/               # DbContext + SeedData
└── Migrations/         # EF Core migration history
```

**Luồng xử lý:** Request → Controller → Service → DbContext → SQL Server → ViewModel → View

---

## 3. CƠ SỞ DỮ LIỆU

### 3.1 Lịch sử Migration

| Migration                         | Nội dung                                      |
|-----------------------------------|-----------------------------------------------|
| InitialCreate                     | Bảng Users, Teams, Matches cơ bản             |
| AddCoreFeatures                   | TeamTactics, Comments, Attendances            |
| AddPlayers                        | Bảng Players, vị trí, quốc tịch              |
| AddPlayerAttendance               | Điểm danh cầu thủ theo trận                  |
| AddMatchEvents                    | Sự kiện trận (bàn thắng, thẻ, thay người)    |
| AddPlayerHealthAndStringEnums     | Trạng thái sức khỏe cầu thủ                  |
| AddPerformanceRating              | Đánh giá phong độ HLV                        |
| AddDrillLibrary                   | Thư viện bài tập, buổi tập, TrainingSession  |
| AddTrainingAttendance             | Điểm danh buổi tập                           |
| AddTacticalBoard                  | PositionsJson, ArrowsJson cho chiến thuật    |
| AddUserRoles                      | IsActive, PlayerId FK cho phân quyền         |
| FixIsActiveDefault                | Kích hoạt lại tất cả tài khoản cũ           |

### 3.2 Sơ đồ quan hệ (ERD rút gọn)

```
Team ──< Player ──< PlayerAttendance >── Match
  │                │                      │
  │                ├──< PlayerMatchStats   ├──< MatchEvent
  │                ├──< PlayerTrainingStats│
  │                └──< TrainingAttendance─┘
  │
  ├──< TrainingSession ──< TrainingSessionDrill >── Drill
  └──  TeamTactics

User ──── Player (optional FK, role=Player)
PerformanceRating >── Player
```

---

## 4. PHÂN QUYỀN HỆ THỐNG

| Role        | Quyền truy cập                                                          |
|-------------|-------------------------------------------------------------------------|
| **Admin**   | Toàn quyền + quản lý tài khoản, xóa đội/cầu thủ                       |
| **Coach**   | Nhập chỉ số tập, điểm danh, lịch tập, bảng chiến thuật, sức khỏe      |
| **Analyst** | Nhập chỉ số thi đấu, xem tất cả báo cáo và thống kê                   |
| **Player**  | Xem dữ liệu cá nhân, lịch tập, lịch sử điểm danh (Player Portal)      |
| **Member**  | Chỉ xem — không chỉnh sửa bất kỳ dữ liệu nào                         |

**Đăng nhập mặc định:** `admin` / `admin123`

**Xác thực:** Cookie-based, hash SHA-256 cho mật khẩu, thời hạn 8 giờ (hoặc 7 ngày nếu Remember Me). Tài khoản bị vô hiệu hóa (`IsActive = false`) bị chặn đăng nhập.

---

## 5. CÁC MODULE CHỨC NĂNG

### 5.1 Quản lý Đội bóng (Team)

**Controller:** `TeamController`  
**Service:** `TeamService`  
**Views:** Index, Create, Edit, Details

**Chức năng:**
- Danh sách đội bóng, xem chi tiết, tạo mới, chỉnh sửa
- Xóa đội: cascade xóa toàn bộ cầu thủ, trận đấu, buổi tập, điểm danh trong 1 transaction. Hiển thị cảnh báo số lượng dữ liệu bị xóa trước khi xác nhận.

**Code đáng chú ý (`TeamService.DeleteTeamAsync`):**
```csharp
await using var tx = await _db.Database.BeginTransactionAsync();
// Xóa theo thứ tự: MatchEvent → PlayerAttendance → PlayerMatchStats
// → PerformanceRating → Match → PlayerTrainingStats
// → TrainingAttendance → Player → TrainingSession → TeamTactics → Team
await tx.CommitAsync();
```

---

### 5.2 Quản lý Cầu thủ (Player)

**Controller:** `PlayerController`  
**Service:** `PlayerService`  
**Views:** Index, Create, Edit, Details

**Chức năng:**
- CRUD cầu thủ với số áo unique trong đội
- Quản lý trạng thái sức khỏe: Fit / Injured / Sick / Suspended / Recovering
- Ngày dự kiến trở lại khi chấn thương
- Xóa cầu thủ: cascade xóa 6 bảng liên quan (PlayerAttendance, PlayerMatchStats, PlayerTrainingStats, TrainingAttendance, MatchEvent, PerformanceRating) trong transaction

**Enum vị trí:**
```
GoalKeeper | CenterBack | FullBack | DefensiveMid | CentralMid | AttackingMid | Winger | Striker
```

---

### 5.3 Quản lý Trận đấu (Match)

**Controller:** `MatchController`  
**Service:** `MatchService`  
**Views:** Index, Create, Details, UpdateResult

**Chức năng:**
- Lên lịch trận đấu giữa 2 đội
- Cập nhật kết quả sau trận (HomeScore, AwayScore)
- Hủy trận đấu
- Trạng thái: Scheduled → Completed / Cancelled

---

### 5.4 Sự kiện Trận đấu (MatchEvent)

**Controller:** `MatchEventController`  
**Service:** `MatchEventService`  
**Views:** Index, Add

**Chức năng:**
- Ghi nhận sự kiện: Bàn thắng, OwnGoal, Penalty, YellowCard, RedCard, Substitution
- Thời điểm xảy ra (phút thi đấu)
- Cầu thủ vào sân khi thay người (SubstitutePlayerId)

---

### 5.5 Điểm danh Trận đấu (Attendance)

**Controller:** `AttendanceController`  
**Service:** `AttendanceService`  
**View:** Match

**Chức năng:**
- Điểm danh từng cầu thủ trước/sau trận đấu
- Trạng thái: Present / Absent / Injured / Unknown
- Ghi chú lý do

---

### 5.6 Chỉ số Cầu thủ (PlayerStats)

**Controller:** `PlayerStatsController`  
**Service:** `PlayerStatsDetailService`  
**Views:** Index, MatchStats, Training, TacticalBoard

#### Chỉ số Thi đấu (`PlayerMatchStats`)
Nhập thủ công sau trận, phân theo vị trí:
- **Thủ môn:** PSxG+/-, % Cứu thua, % Chuyền dài
- **Trung vệ:** % Không chiến, Giải nguy, Cắt bóng
- **Hậu vệ biên:** Tạt chính xác, % Tắc bóng, Quãng đường
- **Tiền vệ:** Thu hồi bóng, % Chuyền khi pressing, Chuyền quyết định
- **Tiền đạo:** Sút/90 phút, % Sút trúng đích, Tỷ lệ ghi bàn

#### Chỉ số Tập luyện (`PlayerTrainingStats`)
- 3 loại: Physical (Thể lực) / Technical (Kỹ thuật) / Tactical (Chiến thuật)
- Tích hợp chọn bài tập từ thư viện Drill qua AJAX
- HLV chấm điểm 1–10 và nhận xét

#### Biểu đồ Phong độ (Chart.js)
- Line chart điểm HLV theo thời gian (30 buổi gần nhất)
- Line chart chỉ số thi đấu (10 trận gần nhất) với 2 metric theo vị trí
- 4 card thống kê nhanh: Điểm TB, % Chuyền TB, tổng buổi tập, tổng trận
- Chỉ số xu hướng: so sánh 3 buổi gần nhất vs 3 buổi trước

---

### 5.7 Bảng Chiến thuật (TacticalBoard)

**Controller:** `PlayerStatsController.TacticalBoard`  
**View:** `PlayerStats/TacticalBoard.cshtml`  
**Lưu trữ:** Cột `PositionsJson` và `ArrowsJson` trong bảng `TeamTactics`

**Chức năng:**
- Canvas HTML5 vẽ sân cỏ với đường kẻ chuẩn (vòng cấm, vòng tròn giữa, chấm penalty)
- **Bấm chip cầu thủ** → tự động đặt vào slot trống theo formation, hoặc xóa khỏi sân nếu đang có
- **Kéo thả** chip cầu thủ lên vị trí chính xác trên canvas
- **Double-click** cầu thủ trên sân → xóa khỏi sân
- Chip highlight xanh + dấu ✓ khi cầu thủ đang trên sân
- **Vẽ mũi tên** (5 màu, liền hoặc nét đứt) mô tả đường di chuyển
- 7 sơ đồ formation: 4-3-3, 4-4-2, 3-5-2, 4-2-3-1, 5-3-2, 3-4-3, 4-1-4-1
- **Undo** nhiều bước, Reset vị trí, Xóa mũi tên
- **Lưu AJAX** không reload trang, lưu JSON tọa độ lên server

**Lưu trữ JSON format:**
```json
positions: [{"id":1,"name":"Nguyên","jerseyNumber":"9","x":350,"y":130}]
arrows:    [{"x1":200,"y1":300,"x2":350,"y2":200,"color":"#ffffff","dash":"solid"}]
```

---

### 5.8 Thư viện Bài tập (Drill Library)

**Controller:** `DrillController`  
**Service:** `DrillService`  
**Views:** Index, Create, Edit, Details

**Chức năng:**
- Kho bài tập chia theo 3 loại: Thể lực / Kỹ thuật / Chiến thuật
- Mỗi bài có: Tên, Mô tả, Hướng dẫn chi tiết, Thời lượng, Độ khó (1-5), Link video YouTube
- Chia sẻ (`IsShared`): HLV có thể cho phép tất cả HLV khác thấy bài tập
- Ẩn/hiện bài tập (`IsActive`) mà không cần xóa
- Đếm số lần bài tập đã được sử dụng
- **AJAX endpoint** `/Drill/SelectList?category=Physical` → dropdown trong form nhập chỉ số tập

---

### 5.9 Lịch tập & Buổi tập (TrainingSession + Calendar)

**Controllers:** `TrainingSessionController`, `CalendarController`  
**Services:** `TrainingSessionService`, `CalendarService`  
**Views:** TrainingSession/Index|Details|Form, Calendar/Index|Attendance

#### Buổi tập (TrainingSession)
- Lên lịch buổi tập cho đội với tiêu đề, thời gian, địa điểm
- Gán danh sách bài tập (Drill) theo thứ tự
- Trạng thái: Scheduled → Completed / Cancelled
- Ghi chú HLV

#### Lịch (FullCalendar)
- Hiển thị trận đấu (đỏ) và buổi tập (xanh) trên cùng 1 lịch
- Lọc theo đội
- View tháng / tuần / danh sách
- Click event → redirect đến trang chi tiết

#### Điểm danh Buổi tập
- Danh sách cầu thủ của đội với trạng thái: Present / Late / Absent / Excused / Injured
- Nhập số phút đến muộn khi Late
- 5 card thống kê: số lượng từng trạng thái

---

### 5.10 Đánh giá Phong độ (PerformanceRating)

**Controller:** `PerformanceRatingController`  
**Service:** `PerformanceRatingService`  
**Views:** Index, Form

**Chức năng:**
- HLV/Analyst chấm điểm 1–10 cho từng tiêu chí: Tổng thể, Thái độ, Thể lực, Kỹ thuật, Chiến thuật
- 4 loại đánh giá: Sau trận / Sau tập / Tuần / Tháng
- Liên kết đánh giá với trận đấu cụ thể
- Công bố/ẩn đánh giá (`IsPublishedToPlayer`) — kiểm soát xem cầu thủ có thấy không
- Biểu đồ line chart phong độ theo thời gian (20 điểm gần nhất)
- Tính xu hướng: so sánh 3 đánh giá gần nhất vs 3 trước

---

### 5.11 Thống kê (Stats)

**Controller:** `StatsController`  
**Service:** `PlayerStatsService, StatisticsService`  
**Views:** Index, Player

**Chức năng:**
- Top ghi bàn, nhiều thẻ vàng nhất, thẻ đỏ nhiều nhất
- Lọc theo đội
- Thống kê chi tiết từng cầu thủ (bàn thắng, penalty, thẻ, số trận)

---

### 5.12 Dashboard

**Controller:** `DashboardController`  
**Service:** `StatisticsService`  
**View:** Index

**Chức năng:**
- Tổng quan: số đội, cầu thủ, trận đấu
- Trận sắp diễn ra
- Buổi tập sắp tới

---

### 5.13 Quản lý Tài khoản (User)

**Controller:** `UserController`  
**Service:** `UserService`  
**Views:** Index, Create, Edit, ChangePassword, Portal

**Chức năng:**
- Admin: CRUD tài khoản, gán role, kích hoạt/vô hiệu hóa
- Liên kết tài khoản role Player với Player record (để truy cập dữ liệu cá nhân)
- Đổi mật khẩu
- Bảo vệ: không xóa Admin cuối cùng

#### Player Portal
- Sau đăng nhập, role Player được redirect thẳng đến Portal cá nhân
- Xem: thông tin cá nhân, số áo, vị trí, trạng thái sức khỏe
- Lịch buổi tập 7 ngày tới (kèm trạng thái điểm danh của mình)
- Lịch sử điểm danh 10 buổi gần nhất
- Điểm HLV gần nhất và điểm TB
- Link xem biểu đồ phong độ cá nhân

---

## 6. HELPER VÀ TIỆN ÍCH

### ViewModelHelpers (`ViewModels/ViewModelHelpers.cs`)
Static class tránh duplicate switch expression:
- `GetPositionLabel(PlayerPosition)` → "Thủ môn", "Trung vệ"...
- `GetPositionShort(PlayerPosition)` → "TM", "TV"...
- `GetCategoryLabel(TrainingCategory)` → "Thể lực", "Kỹ thuật"...
- `GetDifficultyLabel(int)` → "★★★☆☆ Trung bình"...
- `GetSessionStatusLabel/Badge(TrainingSessionStatus)`

---

## 7. BẢO MẬT

| Biện pháp | Mô tả |
|---|---|
| Password hashing | SHA-256, không lưu plaintext |
| Anti-Forgery Token | Tất cả form POST đều có `[ValidateAntiForgeryToken]` |
| Cookie Auth | HttpOnly cookie, expire 8h |
| Role-based Auth | `[Authorize(Roles = "Admin,Coach")]` trên từng action |
| IsActive check | Tài khoản vô hiệu bị chặn ở bước login |
| XSS prevention | Dùng `@Json.Serialize()` thay vì `Html.Raw()` cho data từ DB vào JS |
| Transaction | Các thao tác xóa cascade dùng DB transaction, rollback nếu lỗi |
| Input validation | DataAnnotations + ModelState validation |

---

## 8. CẤU TRÚC FILE CHÍNH

```
Controllers/
  AuthController.cs          — Đăng nhập, đăng xuất, redirect theo role
  TeamController.cs          — CRUD đội bóng
  PlayerController.cs        — CRUD cầu thủ
  MatchController.cs         — Quản lý trận đấu
  MatchEventController.cs    — Sự kiện trận
  AttendanceController.cs    — Điểm danh trận đấu
  PlayerStatsController.cs   — Chỉ số, biểu đồ, bảng chiến thuật
  DrillController.cs         — Thư viện bài tập
  TrainingSessionController.cs — Buổi tập
  CalendarController.cs      — Lịch + điểm danh buổi tập
  PerformanceRatingController.cs — Đánh giá phong độ
  StatsController.cs         — Thống kê
  DashboardController.cs     — Tổng quan
  UserController.cs          — Quản lý tài khoản + Player Portal

Services/
  TeamService.cs             — Business logic đội bóng
  PlayerService.cs           — Business logic cầu thủ + chiến thuật
  MatchService.cs            — Business logic trận đấu
  AttendanceService.cs       — Điểm danh trận
  MatchEventService.cs       — Sự kiện trận
  PlayerStatsDetailService.cs — Chỉ số + biểu đồ phong độ
  PlayerStatsService.cs      — Thống kê tổng hợp
  DrillService.cs            — Thư viện bài tập
  TrainingSessionService.cs  — Quản lý buổi tập
  CalendarService.cs         — Calendar events + điểm danh buổi tập
  PerformanceRatingService.cs — Đánh giá phong độ
  StatisticsService.cs       — Dashboard data
  UserService.cs             — Tài khoản + Player Portal

Models/
  Team.cs, Player.cs, Match.cs, MatchEvent.cs
  PlayerAttendance.cs, PlayerMatchStats.cs, PlayerTrainingStats.cs
  TeamTactics.cs, Drill.cs, TrainingSession.cs
  TrainingSessionDrill.cs, TrainingAttendance.cs
  PerformanceRating.cs, User.cs

Data/
  ApplicationDbContext.cs    — DbContext, 15 DbSet, fluent API config
  SeedData.cs                — Tạo tài khoản admin mặc định

ViewModels/
  PlayerViewModels.cs        — List/Create/Edit/Details cho cầu thủ
  PlayerStatsViewModels.cs   — Chart data (TrainingChartPoint, MatchChartPoint...)
  DrillViewModels.cs         — Drill CRUD + Select
  TrainingSessionViewModels.cs — Buổi tập CRUD
  TrainingAttendanceViewModels.cs — Điểm danh + CalendarEventDto
  PerformanceRatingViewModels.cs — Đánh giá + biểu đồ
  UserViewModels.cs          — Tài khoản + PlayerPortal
  ViewModelHelpers.cs        — Static helper tránh duplicate code
```

---

## 9. ĐIỂM ĐẶC BIỆT

### Cascade Delete an toàn
Tất cả thao tác xóa đội/cầu thủ đều xóa theo đúng thứ tự FK constraint trong 1 transaction, rollback toàn bộ nếu có lỗi.

### Chart.js tích hợp sâu
Biểu đồ phong độ thay đổi metric theo vị trí cầu thủ — thủ môn xem % cứu thua, tiền đạo xem tỷ lệ ghi bàn, tự động.

### TacticalBoard không reload
Lưu vị trí cầu thủ và mũi tên bằng fetch AJAX, không mất trạng thái canvas sau khi lưu.

### Migration history rõ ràng
12 migration với tên mô tả rõ từng giai đoạn phát triển.

---

*Báo cáo được tạo ngày 21/06/2026*
