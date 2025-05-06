# Quản Lý Chi Tiêu - ASP.NET MVC & SQL Server

Đây là hệ thống Website quản lý chi tiêu cá nhân được xây dựng bằng ASP.NET MVC, sử dụng ADO.NET để truy xuất cơ sở dữ liệu SQL Server. 
Dự án này hỗ trợ phân quyền người dùng (Quản trị viên và người dùng thông thường)

## 📁 Clone Dự Án
=> git clone https://github.com/dqhai47/dqhai47-ASPNET-DK23TTC11-dinhquochai-QuanLyChiTieu.git
   
## 📁 Cài Đặt Cơ Sở Dữ Liệu

Bạn có thể chọn 1 trong 2 cách dưới đây. Cả hai đều giúp bạn có đầy đủ **bảng và dữ liệu mẫu** để chạy hệ thống.

### ✅ Cách 1: Restore từ file `.bak` (Nhanh chóng & đầy đủ)
1. Mở **SQL Server Management Studio (SSMS)**.
2. Chuột phải vào `Databases` → chọn `Restore Database...`.
3. Chọn **Device** → nhấn `...` → chọn file `QuanLyChiTieu.bak`.
4. Đặt tên cơ sở dữ liệu là `QuanLyChiTieu`.
5. Nhấn OK để thực hiện restore.

> ⚠️ Lưu ý: Yêu cầu SQL Server cùng phiên bản hoặc cao hơn phiên bản khi backup.

---

### 💻 Cách 2: Chạy script SQL (Khuyên dùng nếu muốn tùy biến hoặc học tập)
1. Mở SSMS.
2. Tạo một cơ sở dữ liệu mới tên là `QuanLyChiTieu`.
3. Mở file `QuanLyChiTieu.sql`.
4. Chạy toàn bộ script để tạo bảng và dữ liệu mẫu.

> ✅ Cách này đảm bảo bạn hiểu rõ cấu trúc và thuận tiện tùy chỉnh nếu cần.

⚙️ Cấu Hình Dự Án
Mở file QuanLyChiTieu.sln bằng Visual Studio.

Trong Web.config, chỉnh chuỗi kết nối đến SQL Server phù hợp với máy của bạn:

<connectionStrings>
  <add name="MyConnection" 
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=QuanLyChiTieu;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
    
🔁 Lưu ý: Nếu bạn dùng SQL Server Authentication thì cần sửa Integrated Security=False và thêm User ID=...;Password=....

▶️ Chạy Dự Án
Nhấn F5 trong Visual Studio để chạy.
Trang đăng nhập sẽ hiện ra. Dùng tài khoản mẫu sau:
🧑 Tài Khoản Mẫu
1. Quản trị viên:
- Tên đăng nhập: admin
- Mật khẩu: 12345678Aa@

2. Người dùng thông thường
- Tên đăng nhập: haiplc47
- Mật khẩu: 12345678Aa@

📌 Tính Năng Chính
1. Người dùng thông thường
- Đăng ký, đăng nhập và phân quyền.
- Quản lý thu/chi theo danh mục, loại thu chi và loại tiền tệ của từng người dùng.
- Thống kê và hiển thị báo cáo thu chi theo dạng thông tin, biểu đồ chi tiêu.
- Quản lý ảnh đính kèm hóa đơn.

2. Quản trị viên
- Tất cả chức năng của người dùng thông thường
- Quản lý danh sách người dùng
- Cấu hình tài nguyên phân loại/danh mục/...
- Báo cáo thống kê tổng quan
  
📬 Liên hệ
Sinh viên: Đinh Quốc Hải
Mã sinh viên: 170123494
Lớp: DK23TTC11
SĐT: 0978 363 700
