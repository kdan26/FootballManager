# Tài Liệu Yêu Cầu

## Giới Thiệu

Tài liệu này mô tả các yêu cầu cho ba tính năng cốt lõi của hệ thống **Football Manager** — một ứng dụng web ASP.NET Core MVC dùng để quản lý đội bóng nội bộ. Ba tính năng bao gồm:

1. **Quản lý Đội bóng (Team Management)** — Thêm, sửa, xóa và xem danh sách các đội bóng.
2. **Quản lý Trận đấu (Match Management)** — Lên lịch trận đấu, cập nhật kết quả và xem chi tiết trận.
3. **Trang Tổng quan (Dashboard)** — Hiển thị thống kê tổng hợp về đội bóng và trận đấu.

Hệ thống đã có sẵn xác thực Cookie Authentication với ba vai trò: **Admin**, **Coach** và **Member**.

---

## Bảng Thuật Ngữ

- **Hệ thống**: Ứng dụng web Football Manager (ASP.NET Core MVC).
- **TeamService**: Service xử lý nghiệp vụ liên quan đến đội bóng.
- **MatchService**: Service xử lý nghiệp vụ liên quan đến trận đấu.
- **StatisticsService**: Service tổng hợp và tính toán số liệu thống kê.
- **TeamController**: Controller điều phối các yêu cầu HTTP liên quan đến đội bóng.
- **MatchController**: Controller điều phối các yêu cầu HTTP liên quan đến trận đấu.
- **DashboardController**: Controller điều phối yêu cầu HTTP cho trang tổng quan.
- **Người dùng đã xác thực**: Người dùng đã đăng nhập thành công vào hệ thống.
- **Admin**: Vai trò quản trị viên, có toàn quyền thao tác.
- **Coach**: Vai trò huấn luyện viên, có quyền quản lý trận đấu và xem thông tin đội.
- **Member**: Vai trò thành viên, chỉ có quyền xem thông tin.
- **Đội bóng (Team)**: Một đội tham gia thi đấu, có tên, sân nhà và thông tin mô tả.
- **Trận đấu (Match)**: Một trận thi đấu giữa hai đội, có lịch thi đấu, địa điểm và kết quả.
- **Kết quả trận đấu**: Số bàn thắng của đội nhà và đội khách sau khi trận đấu kết thúc.
- **Trạng thái trận đấu**: Một trong ba giá trị: `Scheduled` (đã lên lịch), `Completed` (đã kết thúc), `Cancelled` (đã hủy).
- **Dashboard**: Trang tổng quan hiển thị các số liệu thống kê tổng hợp.

---

## Yêu Cầu

### Yêu Cầu 1: Quản lý Đội bóng — Xem Danh Sách

**User Story:** Là một người dùng đã xác thực, tôi muốn xem danh sách tất cả các đội bóng, để tôi có thể nắm bắt tổng quan về các đội trong hệ thống.

#### Tiêu Chí Chấp Nhận

1. WHEN người dùng đã xác thực truy cập `/Team`, THE TeamController SHALL trả về danh sách tất cả các đội bóng được lưu trong cơ sở dữ liệu.
2. THE Hệ thống SHALL hiển thị tên đội, sân nhà và số trận đã thi đấu cho mỗi đội trong danh sách.
3. WHEN danh sách đội bóng rỗng, THE TeamController SHALL hiển thị thông báo "Chưa có đội bóng nào" thay vì bảng dữ liệu trống.
4. WHEN người dùng chưa xác thực truy cập `/Team`, THE Hệ thống SHALL chuyển hướng người dùng đến trang đăng nhập `/Auth/Login`.

---

### Yêu Cầu 2: Quản lý Đội bóng — Thêm Đội Mới

**User Story:** Là một Admin, tôi muốn thêm đội bóng mới vào hệ thống, để tôi có thể mở rộng danh sách các đội tham gia.

#### Tiêu Chí Chấp Nhận

1. WHEN Admin truy cập form tạo đội mới, THE TeamController SHALL hiển thị form nhập liệu với các trường: tên đội, sân nhà, mô tả.
2. WHEN Admin gửi form hợp lệ với tên đội và sân nhà, THE TeamService SHALL lưu đội bóng mới vào cơ sở dữ liệu và THE TeamController SHALL chuyển hướng về trang danh sách đội.
3. IF tên đội bị bỏ trống hoặc vượt quá 100 ký tự, THEN THE Hệ thống SHALL hiển thị thông báo lỗi validation tương ứng và giữ nguyên dữ liệu đã nhập.
4. IF tên đội đã tồn tại trong cơ sở dữ liệu, THEN THE TeamService SHALL trả về lỗi và THE TeamController SHALL hiển thị thông báo "Tên đội đã tồn tại".
5. WHEN người dùng có vai trò Coach hoặc Member truy cập chức năng tạo đội, THE Hệ thống SHALL trả về HTTP 403 Forbidden.

---

### Yêu Cầu 3: Quản lý Đội bóng — Sửa Thông Tin Đội

**User Story:** Là một Admin, tôi muốn chỉnh sửa thông tin đội bóng, để tôi có thể cập nhật dữ liệu khi có thay đổi.

#### Tiêu Chí Chấp Nhận

1. WHEN Admin truy cập form chỉnh sửa đội với ID hợp lệ, THE TeamController SHALL hiển thị form với dữ liệu hiện tại của đội được điền sẵn.
2. WHEN Admin gửi form chỉnh sửa hợp lệ, THE TeamService SHALL cập nhật thông tin đội trong cơ sở dữ liệu và THE TeamController SHALL chuyển hướng về trang danh sách đội.
3. IF Admin gửi form chỉnh sửa với tên đội bị bỏ trống, THEN THE Hệ thống SHALL hiển thị thông báo lỗi validation và giữ nguyên dữ liệu đã nhập.
4. IF ID đội bóng không tồn tại trong cơ sở dữ liệu, THEN THE TeamController SHALL trả về HTTP 404 Not Found.

---

### Yêu Cầu 4: Quản lý Đội bóng — Xóa Đội

**User Story:** Là một Admin, tôi muốn xóa đội bóng khỏi hệ thống, để tôi có thể loại bỏ các đội không còn hoạt động.

#### Tiêu Chí Chấp Nhận

1. WHEN Admin xác nhận xóa một đội bóng, THE TeamService SHALL xóa đội đó khỏi cơ sở dữ liệu và THE TeamController SHALL chuyển hướng về trang danh sách đội.
2. IF đội bóng có trận đấu liên quan trong cơ sở dữ liệu, THEN THE TeamService SHALL từ chối xóa và THE TeamController SHALL hiển thị thông báo "Không thể xóa đội đang có trận đấu liên quan".
3. IF ID đội bóng không tồn tại, THEN THE TeamController SHALL trả về HTTP 404 Not Found.
4. WHEN người dùng có vai trò Coach hoặc Member thực hiện yêu cầu xóa, THE Hệ thống SHALL trả về HTTP 403 Forbidden.

---

### Yêu Cầu 5: Quản lý Đội bóng — Xem Chi Tiết Đội

**User Story:** Là một người dùng đã xác thực, tôi muốn xem chi tiết một đội bóng, để tôi có thể biết thông tin đầy đủ và lịch sử trận đấu của đội đó.

#### Tiêu Chí Chấp Nhận

1. WHEN người dùng đã xác thực truy cập `/Team/Details/{id}` với ID hợp lệ, THE TeamController SHALL hiển thị tên đội, sân nhà, mô tả và danh sách các trận đấu liên quan.
2. IF ID đội bóng không tồn tại, THEN THE TeamController SHALL trả về HTTP 404 Not Found.

---

### Yêu Cầu 6: Quản lý Trận đấu — Xem Danh Sách

**User Story:** Là một người dùng đã xác thực, tôi muốn xem danh sách tất cả các trận đấu, để tôi có thể theo dõi lịch thi đấu và kết quả.

#### Tiêu Chí Chấp Nhận

1. WHEN người dùng đã xác thực truy cập `/Match`, THE MatchController SHALL trả về danh sách tất cả các trận đấu được sắp xếp theo ngày thi đấu giảm dần.
2. THE Hệ thống SHALL hiển thị tên đội nhà, tên đội khách, ngày thi đấu, địa điểm và trạng thái cho mỗi trận trong danh sách.
3. WHEN trận đấu có trạng thái `Completed`, THE Hệ thống SHALL hiển thị kết quả (tỷ số) bên cạnh tên hai đội.
4. WHEN danh sách trận đấu rỗng, THE MatchController SHALL hiển thị thông báo "Chưa có trận đấu nào được lên lịch".
5. WHEN người dùng chưa xác thực truy cập `/Match`, THE Hệ thống SHALL chuyển hướng đến trang đăng nhập.

---

### Yêu Cầu 7: Quản lý Trận đấu — Lên Lịch Trận Đấu

**User Story:** Là một Admin hoặc Coach, tôi muốn lên lịch trận đấu mới, để tôi có thể tổ chức và thông báo lịch thi đấu cho các thành viên.

#### Tiêu Chí Chấp Nhận

1. WHEN Admin hoặc Coach truy cập form tạo trận đấu, THE MatchController SHALL hiển thị form với các trường: đội nhà, đội khách, ngày giờ thi đấu, địa điểm, ghi chú; và danh sách đội bóng để chọn.
2. WHEN Admin hoặc Coach gửi form hợp lệ, THE MatchService SHALL lưu trận đấu mới với trạng thái `Scheduled` và THE MatchController SHALL chuyển hướng về trang danh sách trận đấu.
3. IF đội nhà và đội khách được chọn là cùng một đội, THEN THE MatchService SHALL trả về lỗi và THE MatchController SHALL hiển thị thông báo "Đội nhà và đội khách không được trùng nhau".
4. IF ngày thi đấu bị bỏ trống hoặc đội nhà hoặc đội khách không được chọn, THEN THE Hệ thống SHALL hiển thị thông báo lỗi validation tương ứng.
5. WHEN người dùng có vai trò Member truy cập chức năng tạo trận đấu, THE Hệ thống SHALL trả về HTTP 403 Forbidden.

---

### Yêu Cầu 8: Quản lý Trận đấu — Cập Nhật Kết Quả

**User Story:** Là một Admin hoặc Coach, tôi muốn cập nhật kết quả trận đấu sau khi trận kết thúc, để tôi có thể lưu lại tỷ số chính xác cho thống kê.

#### Tiêu Chí Chấp Nhận

1. WHEN Admin hoặc Coach truy cập form cập nhật kết quả của trận có trạng thái `Scheduled`, THE MatchController SHALL hiển thị form nhập số bàn thắng của đội nhà và đội khách.
2. WHEN Admin hoặc Coach gửi kết quả hợp lệ (số bàn thắng >= 0), THE MatchService SHALL cập nhật số bàn thắng và chuyển trạng thái trận đấu thành `Completed`.
3. IF số bàn thắng được nhập là số âm, THEN THE Hệ thống SHALL hiển thị thông báo lỗi validation "Số bàn thắng không được âm".
4. IF trận đấu đã có trạng thái `Completed` hoặc `Cancelled`, THEN THE MatchService SHALL từ chối cập nhật và THE MatchController SHALL hiển thị thông báo "Trận đấu này đã kết thúc hoặc đã hủy".

---

### Yêu Cầu 9: Quản lý Trận đấu — Xem Chi Tiết Trận

**User Story:** Là một người dùng đã xác thực, tôi muốn xem chi tiết một trận đấu, để tôi có thể biết đầy đủ thông tin về trận đó.

#### Tiêu Chí Chấp Nhận

1. WHEN người dùng đã xác thực truy cập `/Match/Details/{id}` với ID hợp lệ, THE MatchController SHALL hiển thị tên đội nhà, tên đội khách, ngày giờ thi đấu, địa điểm, trạng thái và ghi chú.
2. WHEN trận đấu có trạng thái `Completed`, THE MatchController SHALL hiển thị thêm kết quả tỷ số của trận đấu.
3. IF ID trận đấu không tồn tại, THEN THE MatchController SHALL trả về HTTP 404 Not Found.

---

### Yêu Cầu 10: Quản lý Trận đấu — Hủy Trận Đấu

**User Story:** Là một Admin, tôi muốn hủy một trận đấu đã lên lịch, để tôi có thể xử lý các tình huống bất khả kháng.

#### Tiêu Chí Chấp Nhận

1. WHEN Admin xác nhận hủy trận đấu có trạng thái `Scheduled`, THE MatchService SHALL cập nhật trạng thái trận đấu thành `Cancelled`.
2. IF trận đấu đã có trạng thái `Completed` hoặc `Cancelled`, THEN THE MatchService SHALL từ chối hủy và THE MatchController SHALL hiển thị thông báo "Không thể hủy trận đấu đã kết thúc hoặc đã hủy trước đó".
3. WHEN người dùng có vai trò Coach hoặc Member thực hiện yêu cầu hủy trận, THE Hệ thống SHALL trả về HTTP 403 Forbidden.

---

### Yêu Cầu 11: Trang Tổng Quan (Dashboard)

**User Story:** Là một người dùng đã xác thực, tôi muốn xem trang tổng quan với các số liệu thống kê, để tôi có thể nắm bắt nhanh tình hình hoạt động của hệ thống.

#### Tiêu Chí Chấp Nhận

1. WHEN người dùng đã xác thực truy cập `/Dashboard`, THE DashboardController SHALL hiển thị các số liệu: tổng số đội bóng, tổng số trận đấu, số trận đã hoàn thành và số trận sắp diễn ra.
2. THE StatisticsService SHALL tính tổng số đội bóng bằng cách đếm tất cả bản ghi trong bảng Teams.
3. THE StatisticsService SHALL tính số trận sắp diễn ra bằng cách đếm các trận có trạng thái `Scheduled` và ngày thi đấu lớn hơn hoặc bằng ngày hiện tại.
4. THE StatisticsService SHALL tính số trận đã hoàn thành bằng cách đếm các trận có trạng thái `Completed`.
5. WHEN người dùng đã xác thực truy cập `/Dashboard`, THE DashboardController SHALL hiển thị danh sách tối đa 5 trận đấu sắp diễn ra gần nhất, sắp xếp theo ngày thi đấu tăng dần.
6. WHEN người dùng chưa xác thực truy cập `/Dashboard`, THE Hệ thống SHALL chuyển hướng đến trang đăng nhập.

---

### Yêu Cầu 12: Phân Quyền Truy Cập

**User Story:** Là một Admin, tôi muốn hệ thống kiểm soát quyền truy cập theo vai trò, để tôi có thể đảm bảo chỉ người có thẩm quyền mới thực hiện được các thao tác nhạy cảm.

#### Tiêu Chí Chấp Nhận

1. THE Hệ thống SHALL cho phép Admin thực hiện tất cả các thao tác: xem, thêm, sửa, xóa đội bóng và trận đấu.
2. THE Hệ thống SHALL cho phép Coach thực hiện các thao tác: xem danh sách, xem chi tiết đội bóng; lên lịch trận đấu và cập nhật kết quả trận đấu.
3. THE Hệ thống SHALL cho phép Member thực hiện các thao tác: xem danh sách và xem chi tiết đội bóng và trận đấu.
4. WHEN người dùng đã xác thực thực hiện thao tác vượt quá quyền của vai trò, THE Hệ thống SHALL trả về HTTP 403 Forbidden.
