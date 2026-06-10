# Performance Rating — Đánh giá & Chấm điểm phong độ

## Tổng quan

Tính năng **Performance Rating** cho phép HLV và ban huấn luyện đánh giá phong độ cầu thủ theo nhiều tiêu chí, theo dõi biểu đồ phong độ theo thời gian, và quản lý công bố thông tin đánh giá cho cầu thủ.

## Tính năng chính

### 1. Đa dạng loại đánh giá

- **Sau trận đấu** (AfterMatch): Gắn với trận cụ thể, đánh giá sau khi kết thúc trận
- **Sau buổi tập** (AfterTraining): Đánh giá sau buổi tập luyện
- **Đánh giá tuần** (Weekly): Tổng kết phong độ tuần
- **Đánh giá tháng** (Monthly): Tổng kết phong độ tháng

### 2. Hệ thống chấm điểm đa chiều (1-10)

#### Điểm bắt buộc:
- **Tổng thể** (OverallRating): Đánh giá chung về màn trình diễn

#### Điểm tùy chọn (chi tiết):
- **Thái độ / Nỗ lực** (AttitudeRating): Tinh thần thi đấu, sự chuyên nghiệp
- **Thể lực** (FitnessRating): Sức bền, tốc độ, thể trạng
- **Kỹ thuật** (TechnicalRating): Kỹ thuật cá nhân (chuyền, sút, rê...)
- **Chiến thuật** (TacticalRating): Hiểu chiến thuật, định vị, phối hợp

### 3. Biểu đồ phong độ trực quan

- Hiển thị **20 đánh giá gần nhất** dạng line chart (Chart.js)
- Đường chính: **Điểm tổng thể** (màu xanh, nền fill)
- Đường phụ: 4 tiêu chí (nét đứt, màu riêng cho từng tiêu chí)
- Tương tác: hover xem chi tiết từng điểm
- Scale: 0-10, dễ nhìn xu hướng tăng/giảm

### 4. Thống kê nhanh

- **Trung bình tổng thể**: Điểm TB của tất cả đánh giá
- **Trung bình tiêu chí**: TB riêng cho Thái độ, Thể lực, Kỹ thuật, Chiến thuật
- **Xu hướng** (Trend): So sánh 3 đánh giá gần nhất vs 3 trước đó
  - ✅ **Tăng** (màu xanh, icon mũi tên lên)
  - ❌ **Giảm** (màu đỏ, icon mũi tên xuống)
  - ➖ **Ổn định** (màu xám, icon ngang)

### 5. Quản lý công bố

- **Nội bộ** (IsPublishedToPlayer = false): Chỉ HLV/Admin thấy
- **Công bố** (IsPublishedToPlayer = true): Cầu thủ có thể xem
- Toggle nhanh bằng icon con mắt trong danh sách
- Badge trực quan phân biệt trạng thái

### 6. Nhận xét chi tiết

- Field **Notes** (tối đa 1000 ký tự): Ghi chú tự do của HLV
- Hiển thị popover khi click icon chat trong danh sách
- Giúp HLV ghi lại điểm mạnh, điểm yếu cụ thể

## Cấu trúc Database

### Bảng `PerformanceRatings`

| Cột | Kiểu | Mô tả |
|-----|------|-------|
| `Id` | int | Primary Key |
| `PlayerId` | int | FK → Players (Restrict) |
| `RatedByUserId` | int | FK → Users (HLV/chuyên viên) |
| `RatingType` | string | Loại đánh giá (enum) |
| `MatchId` | int? | FK → Matches (tùy chọn) |
| `TrainingStatsId` | int? | FK → PlayerTrainingStats (dự trữ) |
| `RatingDate` | datetime2 | Ngày đánh giá |
| `OverallRating` | decimal(18,2) | Điểm tổng thể (1-10) |
| `AttitudeRating` | decimal(18,2)? | Điểm thái độ |
| `FitnessRating` | decimal(18,2)? | Điểm thể lực |
| `TechnicalRating` | decimal(18,2)? | Điểm kỹ thuật |
| `TacticalRating` | decimal(18,2)? | Điểm chiến thuật |
| `Notes` | nvarchar(1000)? | Nhận xét |
| `IsPublishedToPlayer` | bit | Công bố hay không |
| `CreatedAt` | datetime2 | Timestamp tạo |

### Indexes

- Composite: `(PlayerId, RatingDate)` — tìm kiếm nhanh theo cầu thủ + thời gian
- Foreign keys: `RatedByUserId`, `MatchId`, `TrainingStatsId`

## Cấu trúc Code

### Models
- `PerformanceRating.cs` — Entity chính
- `RatingType` enum — 4 loại đánh giá

### ViewModels
- `PerformanceRatingFormViewModel` — Form thêm/sửa
- `PerformanceRatingListItem` — Item trong danh sách
- `RatingChartPoint` — Điểm dữ liệu chart
- `PerformanceRatingIndexViewModel` — Trang index (danh sách + chart + stats)

### Services
- `IPerformanceRatingService` / `PerformanceRatingService`
  - `GetIndexAsync()` — Load toàn bộ cho trang index
  - `GetFormAsync()` — Load form (thêm mới / sửa)
  - `SaveAsync()` — Lưu (insert / update)
  - `DeleteAsync()` — Xóa
  - `TogglePublishAsync()` — Bật/tắt công bố

### Controllers
- `PerformanceRatingController`
  - `Index` — Trang danh sách + chart
  - `Create` — Form thêm mới (Admin, Coach)
  - `Edit` — Form sửa (Admin, Coach)
  - `Save` — POST lưu
  - `Delete` — POST xóa
  - `TogglePublish` — POST toggle công bố

### Views
- `Index.cshtml` — Trang chính (Chart.js + Bootstrap table + badges)
- `Form.cshtml` — Form thêm/sửa (range slider sync với number input)

## Luồng sử dụng

### Cho HLV / Admin

1. **Xem trang chỉ số cầu thủ**:  
   `PlayerStats/Index?playerId=X` → Click nút **"⭐ Phong độ"**

2. **Xem danh sách đánh giá + biểu đồ**:  
   `PerformanceRating/Index?playerId=X`
   - Thấy biểu đồ phong độ 20 điểm gần nhất
   - Thấy danh sách tất cả đánh giá (bảng, sắp xếp mới nhất trước)
   - Thống kê: TB tổng thể, TB tiêu chí, xu hướng

3. **Thêm đánh giá mới**:  
   Click **"Thêm đánh giá"** → Form:
   - Chọn loại (Sau trận / Sau tập / Tuần / Tháng)
   - Nếu loại = Sau trận: chọn trận từ dropdown
   - Kéo slider điểm tổng thể (bắt buộc)
   - Kéo slider tiêu chí phụ (tùy chọn)
   - Nhập nhận xét
   - Bật/tắt công bố
   - **Lưu đánh giá**

4. **Sửa đánh giá**:  
   Click icon bút chì → Form tương tự, dữ liệu đã được load

5. **Xóa đánh giá**:  
   Click icon thùng rác → Confirm → Xóa vĩnh viễn

6. **Công bố / ẩn**:  
   Click icon con mắt → Toggle trạng thái ngay lập tức

7. **Xem nhận xét**:  
   Click icon chat → Popover hiển thị Notes

### Cho Member (cầu thủ)

- Vào trang `PerformanceRating/Index?playerId=X` (nếu có quyền xem)
- Chỉ thấy các đánh giá có `IsPublishedToPlayer = true`
- Không có nút thêm/sửa/xóa/toggle

## UI/UX Highlights

### Trang Index
- **Header**: Nút quay lại, tên cầu thủ, nút "Thêm đánh giá" (HLV)
- **Card thông tin**: Số áo, vị trí, đội, xu hướng (icon + màu), TB tổng thể lớn, 4 mini stats
- **Card chart**: Canvas Chart.js, legend dưới, tooltip chi tiết, trục Y cố định 0-10
- **Card danh sách**: Bảng responsive (ẩn tiêu chí phụ trên mobile), badge màu theo loại đánh giá, điểm tổng thể nổi bật (màu xanh ≥8, xanh dương 6-8, đỏ <6), badge trạng thái (Công bố / Nội bộ), nút action (popover, toggle, edit, delete)

### Form
- **Range slider + Number input sync realtime**: Kéo slider thì số thay đổi, nhập số thì slider di chuyển
- **Conditional dropdown**: Chỉ hiện dropdown trận khi loại = "Sau trận"
- **Icon trực quan**: Mỗi tiêu chí có icon riêng (emoji, lightning, controller, diagram)
- **Validation**: Required cho Overall, Range(1-10) cho tất cả điểm
- **Switch công bố**: Form-check-switch Bootstrap, rõ ràng

## Integration

- **Link từ PlayerStats/Index**: Nút **"⭐ Phong độ"** màu vàng, nổi bật
- **Không phá vỡ data cũ**: Tất cả FK đều `Restrict` hoặc optional, xóa Player không xóa rating (Restrict)
- **Service pattern**: Tuân thủ pattern service layer hiện có
- **Authorization**: `[Authorize]` cho Index (tất cả user đăng nhập), `[Authorize(Roles = "Admin,Coach")]` cho CUD

## Dependencies

### NuGet (đã có sẵn)
- `Microsoft.EntityFrameworkCore.SqlServer` 9.0.0
- `Microsoft.EntityFrameworkCore.Design` 9.0.0

### Frontend (CDN)
- **Bootstrap 5.3.3** (đã có)
- **Bootstrap Icons 1.11.3** (đã có)
- **Chart.js 4.4.3** (thêm trong `@section Scripts`)

## Migration

```bash
# Đã chạy:
dotnet ef migrations add AddPerformanceRating --project FootballManager
dotnet ef database update --project FootballManager
```

Migration tạo:
- Bảng `PerformanceRatings` với 15 cột
- 4 indexes (PK + 3 FK + 1 composite)
- 4 FK constraints (Player, User, Match, TrainingStats)

## Test Scenarios

### Scenario 1: HLV chấm điểm sau trận
1. Vào PlayerStats/Index → Click "Phong độ"
2. Click "Thêm đánh giá"
3. Chọn "Sau trận đấu", chọn trận gần nhất
4. Kéo slider Tổng thể = 8.5
5. Kéo Thái độ = 9, Thể lực = 7, Kỹ thuật = 8.5, Chiến thuật = 8
6. Nhập nhận xét: "Phát huy tốt khả năng tạt cánh, cần cải thiện tốc độ di chuyển ngược"
7. Bật công bố
8. Lưu → Redirect về Index, thấy chart tăng, danh sách có bản ghi mới

### Scenario 2: Xem xu hướng
1. Thêm 6+ đánh giá cho 1 cầu thủ
2. Vào Index → Thấy:
   - Biểu đồ line chart có 6+ điểm
   - Xu hướng hiển thị (Tăng/Giảm/Ổn định) với icon và màu

### Scenario 3: Quản lý công bố
1. Thêm đánh giá, **không** bật công bố
2. Thấy badge "Nội bộ" màu xám
3. Click icon con mắt → Refresh, badge đổi thành "Công bố" màu xanh
4. (Giả sử) Cầu thủ đăng nhập → Giờ thấy được đánh giá này

## Roadmap mở rộng (tương lai)

- [ ] **So sánh cầu thủ**: Radar chart so sánh 5 tiêu chí giữa nhiều cầu thủ
- [ ] **Thông báo**: Gửi thông báo cho cầu thủ khi có đánh giá mới được công bố
- [ ] **Export PDF**: Xuất báo cáo phong độ cầu thủ theo tháng/quý
- [ ] **Target setting**: HLV đặt mục tiêu điểm số, theo dõi tiến độ
- [ ] **AI suggestions**: Gợi ý bài tập cải thiện dựa trên điểm yếu

## Notes

- Tất cả enum lưu dạng **string** trong DB (convention hiện tại của project)
- Decimal precision mặc định 18,2 (warning EF Core nhưng không ảnh hưởng)
- Chart.js load từ CDN (4.4.3), fallback nếu offline cần thêm bundle local
- Form validation: client-side (jQuery Validation) + server-side (ModelState)

---

**Tác giả**: Kiro AI Assistant  
**Ngày hoàn thành**: 03/06/2026  
**Version**: 1.0.0
