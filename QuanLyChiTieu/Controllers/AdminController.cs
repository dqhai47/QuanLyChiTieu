using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLyChiTieu.Models;

namespace QuanLyChiTieu.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra nếu không phải admin thì chuyển về Login
            if (HttpContext.Session.GetString("ma_quyenhan") != "ADMIN")
            {
                return RedirectToAction("Login", "Account");
            }

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            int totalUsers = 0, maleCount = 0, femaleCount = 0;
            List<CategoryStats> categoryStats = new();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Tổng số người dùng
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NguoiDung", conn))
                {
                    totalUsers = (int)cmd.ExecuteScalar();
                }

                // Giới tính nam
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE gender = N'Nam'", conn))
                {
                    maleCount = (int)cmd.ExecuteScalar();
                }

                // Giới tính nữ
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM NguoiDung WHERE gender = N'Nữ'", conn))
                {
                    femaleCount = (int)cmd.ExecuteScalar();
                }

                // Top 5 danh mục dùng nhiều nhất
                string topCategorySql = @"
                SELECT TOP 5 DM.sname, COUNT(*) AS SoLan
                FROM ChiTieu CT
                JOIN DanhMuc DM ON CT.id_danhMuc = DM.id
                GROUP BY DM.sname
                ORDER BY SoLan DESC";

                using (SqlCommand cmd = new SqlCommand(topCategorySql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categoryStats.Add(new CategoryStats
                        {
                            Name = reader["sname"].ToString(),
                            Count = Convert.ToInt32(reader["SoLan"])
                        });
                    }
                }
            }

            ViewBag.TotalUsers = totalUsers;
            ViewBag.MaleCount = maleCount;
            ViewBag.FemaleCount = femaleCount;
            ViewBag.CategoryStats = categoryStats;

            return View("Index");
        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AdminController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public IActionResult DanhSachNguoiDung()
        {
            List<NguoiDung> list = new List<NguoiDung>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT nd.id, nd.fname, nd.lname, nd.email, nd.username, nd.phone, nd.gender, nd.status_account, nd.id_loainguoidung, nd.descriptions, lnd.sname AS ten_loai FROM NguoiDung nd JOIN LoaiNguoiDung lnd ON nd.id_loainguoidung = lnd.id"; // câu lệnh SQL đầy đủ của bạn
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    NguoiDung nd = new NguoiDung
                    {
                        id = Convert.ToInt32(reader["id"]),
                        fname = reader["fname"].ToString(),
                        lname = reader["lname"].ToString(),
                        email = reader["email"].ToString(),
                        username = reader["username"].ToString(),
                        phone = reader["phone"].ToString(),
                        gender = reader["gender"].ToString(),
                        id_loainguoidung = Convert.ToInt32(reader["id_loainguoidung"]),
                        status_account = Convert.ToBoolean(reader["status_account"]),
                        ten_loai = reader["ten_loai"].ToString(), // Fix ambiguity by explicitly specifying the property
                        descriptions = reader["descriptions"].ToString()
                    };

                    list.Add(nd);
                }
            }

            // ✅ Gán danh sách loại người dùng vào ViewBag
            List<LoaiNguoiDung> dsLoai = new List<LoaiNguoiDung>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string sql = "SELECT id, sname FROM LoaiNguoiDung";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dsLoai.Add(new LoaiNguoiDung
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty
                    });
                }
            }
            ViewBag.listLoaiNguoiDung = dsLoai;

            return View("DanhSachNguoiDung", list);
        }

        public IActionResult LoaiNguoiDung()
        {
            List<LoaiNguoiDung> listLoaiNguoiDung = new List<LoaiNguoiDung>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM LoaiNguoiDung";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LoaiNguoiDung lnd = new LoaiNguoiDung
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty,
                        scode = reader["scode"]?.ToString() ?? string.Empty,
                        descriptions = reader["descriptions"]?.ToString() ?? string.Empty
                    };

                    listLoaiNguoiDung.Add(lnd);
                }
            }

            return View("Setting/LoaiNguoiDung", listLoaiNguoiDung);
        }


        public IActionResult DanhMuc()
        {
            List<DanhMuc> listDanhMuc = new List<DanhMuc>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM DanhMuc";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DanhMuc danhmuc = new DanhMuc
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty,
                        scode = reader["scode"]?.ToString() ?? string.Empty,
                        sdefault = Convert.ToBoolean(reader["sdefault"]), // Fix: Convert string to bool
                        descriptions = reader["descriptions"]?.ToString() ?? string.Empty
                    };

                    listDanhMuc.Add(danhmuc);
                }
            }

            return View("Setting/DanhMuc", listDanhMuc);
        }

        public IActionResult LoaiChiTieu()
        {
            List<LoaiChiTieu> listLoaiChiTieu = new List<LoaiChiTieu>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM LoaiChiTieu";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LoaiChiTieu loaichitieu = new LoaiChiTieu
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty,
                        scode = reader["scode"]?.ToString() ?? string.Empty,
                        descriptions = reader["descriptions"]?.ToString() ?? string.Empty
                    };

                    listLoaiChiTieu.Add(loaichitieu);
                }
            }

            return View("Setting/LoaiChiTieu", listLoaiChiTieu);
        }

        public IActionResult LoaiTienTe()
        {
            List<LoaiTienTe> listLoaiTienTe = new List<LoaiTienTe>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM LoaiTienTe";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LoaiTienTe loaitiente = new LoaiTienTe
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty,
                        scode = reader["scode"]?.ToString() ?? string.Empty,
                        sdefault = Convert.ToBoolean(reader["sdefault"]), // Fix: Convert string to bool
                        converts = reader["converts"]?.ToString() ?? string.Empty,
                        descriptions = reader["descriptions"]?.ToString() ?? string.Empty
                    };

                    listLoaiTienTe.Add(loaitiente);
                }
            }

            return View("Setting/LoaiTienTe", listLoaiTienTe);
        }

        public IActionResult IncomeAndExpense()
        {
            List<ChiTieu> listChiTieu = new List<ChiTieu>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            // Lấy id_user từ đăng nhập
            int? id_user = HttpContext.Session.GetInt32("id_user"); // cần lưu khi đăng nhập
            if (id_user == null) return RedirectToAction("Login", "Account"); // nếu chưa login

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT ct.id, ct.titles, ct.id_loaichitieu, ct.id_danhmuc, ct.id_loaitiente, ct.total, ct.create_at, ct.notes, 
                   lct.sname AS ten_loaichitieu, dm.sname AS ten_danhmuc, ltt.sname AS ten_loaitiente , ltt.scode AS ma_loaitiente 
            FROM ChiTieu ct 
            JOIN LoaiChiTieu lct ON ct.id_loaichitieu = lct.id 
            JOIN DanhMuc dm ON ct.id_danhmuc = dm.id 
            JOIN LoaiTienTe ltt ON ct.id_loaitiente = ltt.id
            WHERE ct.id_user = @id_user";  // Thêm điều kiện WHERE để lọc theo id_user

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id_user", id_user); // Thêm tham số cho id_user
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ChiTieu chitieu = new ChiTieu
                    {
                        id = Convert.ToInt32(reader["id"]),
                        titles = reader["titles"].ToString(),
                        id_loaichitieu = Convert.ToInt32(reader["id_loaichitieu"]),
                        id_danhmuc = Convert.ToInt32(reader["id_danhmuc"]),
                        id_loaitiente = Convert.ToInt32(reader["id_loaitiente"]),
                        ten_loaichitieu = reader["ten_loaichitieu"].ToString(),
                        ten_danhmuc = reader["ten_danhmuc"].ToString(),
                        ten_loaitiente = reader["ten_loaitiente"].ToString(),
                        ma_loaitiente = reader["ma_loaitiente"].ToString(),
                        total = Convert.ToDecimal(reader["total"]),
                        create_at = Convert.ToDateTime(reader["create_at"]),
                        notes = reader["notes"].ToString()
                    };

                    listChiTieu.Add(chitieu);
                }
            }

            // ✅ Gán danh sách loại chi tiêu vào ViewBag
            List<LoaiChiTieu> dsLoaiChiTieu = new List<LoaiChiTieu>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string sql = "SELECT id, sname FROM LoaiChiTieu";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dsLoaiChiTieu.Add(new LoaiChiTieu
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty
                    });
                }
            }
            ViewBag.listLoaiChiTieu = dsLoaiChiTieu;

            // ✅ Gán danh sách danh mục vào ViewBag
            List<DanhMuc> dsDanhMuc = new List<DanhMuc>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string sql = "SELECT id, sname, sdefault FROM DanhMuc";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dsDanhMuc.Add(new DanhMuc
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty,
                        sdefault = Convert.ToBoolean(reader["sdefault"]),
                    });
                }
            }
            ViewBag.listDanhMuc = dsDanhMuc;

            // ✅ Gán danh sách loại tiền tệ vào ViewBag
            List<LoaiTienTe> dsLoaiTienTe = new List<LoaiTienTe>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string sql = "SELECT id, sname, sdefault FROM LoaiTienTe";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dsLoaiTienTe.Add(new LoaiTienTe
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"]?.ToString() ?? string.Empty,
                        sdefault = Convert.ToBoolean(reader["sdefault"]),
                    });
                }
            }
            ViewBag.listLoaiTienTe = dsLoaiTienTe;

            return View("Money/IncomeAndExpense", listChiTieu);
        }

        public IActionResult Report(int? thang, int? nam)
        {
            var model = new ThongKeViewModel();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            // Lấy id_user từ đăng nhập
            int? id_user = HttpContext.Session.GetInt32("id_user");
            if (id_user == null) return RedirectToAction("Login", "Account");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Điều kiện lọc tháng/năm
                string monthCondition = thang.HasValue ? "AND MONTH(ct.create_at) = @thang" : "";
                string yearCondition = nam.HasValue ? "AND YEAR(ct.create_at) = @nam" : "";

                // Tổng thu
                SqlCommand cmdThu = new SqlCommand($@"
            SELECT ISNULL(SUM(total), 0) 
            FROM ChiTieu ct 
            JOIN LoaiChiTieu lct ON ct.id_loaichitieu = lct.id 
            WHERE lct.scode = 'TIEN-THU' AND ct.id_user = @id_user {monthCondition} {yearCondition}", conn);
                cmdThu.Parameters.AddWithValue("@id_user", id_user);
                if (thang.HasValue) cmdThu.Parameters.AddWithValue("@thang", thang.Value);
                if (nam.HasValue) cmdThu.Parameters.AddWithValue("@nam", nam.Value);
                model.TongThu = (decimal)cmdThu.ExecuteScalar();

                // Tổng chi
                SqlCommand cmdChi = new SqlCommand($@"
            SELECT ISNULL(SUM(total), 0) 
            FROM ChiTieu ct 
            JOIN LoaiChiTieu lct ON ct.id_loaichitieu = lct.id 
            WHERE lct.scode = 'TIEN-CHI' AND ct.id_user = @id_user {monthCondition} {yearCondition}", conn);
                cmdChi.Parameters.AddWithValue("@id_user", id_user);
                if (thang.HasValue) cmdChi.Parameters.AddWithValue("@thang", thang.Value);
                if (nam.HasValue) cmdChi.Parameters.AddWithValue("@nam", nam.Value);
                model.TongChi = (decimal)cmdChi.ExecuteScalar();

                // Tổng loại chi tiêu đã dùng
                SqlCommand cmdLoai = new SqlCommand($@"
            SELECT COUNT(DISTINCT id_loaichitieu) 
            FROM ChiTieu ct 
            WHERE ct.id_user = @id_user {monthCondition} {yearCondition}", conn);
                cmdLoai.Parameters.AddWithValue("@id_user", id_user);
                if (thang.HasValue) cmdLoai.Parameters.AddWithValue("@thang", thang.Value);
                if (nam.HasValue) cmdLoai.Parameters.AddWithValue("@nam", nam.Value);
                model.TongLoaiChiTieu = (int)cmdLoai.ExecuteScalar();

                // Loại chi tiêu phổ biến nhất
                SqlCommand cmdPhoBien = new SqlCommand($@"
            SELECT TOP 1 lct.sname 
            FROM ChiTieu ct
            JOIN LoaiChiTieu lct ON ct.id_loaichitieu = lct.id
            WHERE ct.id_user = @id_user {monthCondition} {yearCondition}
            GROUP BY lct.sname
            ORDER BY COUNT(*) DESC", conn);
                cmdPhoBien.Parameters.AddWithValue("@id_user", id_user);
                if (thang.HasValue) cmdPhoBien.Parameters.AddWithValue("@thang", thang.Value);
                if (nam.HasValue) cmdPhoBien.Parameters.AddWithValue("@nam", nam.Value);
                object result = cmdPhoBien.ExecuteScalar();
                model.LoaiChiTieuPhoBien = result != null ? result.ToString() : "Không có dữ liệu";

                // Thống kê theo danh mục
                SqlCommand cmdDanhMuc = new SqlCommand($@"
            SELECT dm.sname, SUM(ct.total) AS tongTien
            FROM ChiTieu ct
            JOIN DanhMuc dm ON ct.id_danhmuc = dm.id
            WHERE ct.id_user = @id_user {monthCondition} {yearCondition}
            GROUP BY dm.sname", conn);
                cmdDanhMuc.Parameters.AddWithValue("@id_user", id_user);
                if (thang.HasValue) cmdDanhMuc.Parameters.AddWithValue("@thang", thang.Value);
                if (nam.HasValue) cmdDanhMuc.Parameters.AddWithValue("@nam", nam.Value);
                SqlDataReader reader = cmdDanhMuc.ExecuteReader();
                while (reader.Read())
                {
                    model.DanhMucThongKes.Add(new DanhMucThongKe
                    {
                        TenDanhMuc = reader["sname"].ToString(),
                        TongTien = Convert.ToDecimal(reader["tongTien"])
                    });
                }
                // Thống kê theo danh mục
                SqlCommand cmdLoaiChiTieu = new SqlCommand($@"
            SELECT lct.sname, SUM(ct.total) AS tongTien
            FROM ChiTieu ct
            JOIN LoaiChiTieu lct ON ct.id_loaichitieu = lct.id
            WHERE ct.id_user = @id_user {monthCondition} {yearCondition}
            GROUP BY lct.sname", conn);
                cmdLoaiChiTieu.Parameters.AddWithValue("@id_user", id_user);
                if (thang.HasValue) cmdLoaiChiTieu.Parameters.AddWithValue("@thang", thang.Value);
                if (nam.HasValue) cmdLoaiChiTieu.Parameters.AddWithValue("@nam", nam.Value);
                SqlDataReader readerLoai = cmdLoaiChiTieu.ExecuteReader();
                while (readerLoai.Read())
                {
                    model.ChiTieuTheoLoaiList.Add(new ChiTieuTheoLoai
                    {
                        TenLoai = readerLoai["sname"].ToString(),
                        TongTien = Convert.ToDecimal(readerLoai["tongTien"])
                    });
                }

                reader.Close();
            }

            return View("Money/Report", model);
        }




        [HttpPost]
        public IActionResult LuuLoaiNguoiDung(IFormCollection form)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            int.TryParse(form["Id"], out int id);
            string? maLoai = form["MaLoai"];
            string? tenLoai = form["TenLoai"];
            string? ghiChu = form["GhiChu"];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (id > 0)
                {
                    // Cập nhật
                    string sql = @"UPDATE LoaiNguoiDung 
                           SET scode = @scode, sname = @sname, descriptions = @descriptions 
                           WHERE id = @id";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    // Thêm mới
                    string sql = @"INSERT INTO LoaiNguoiDung (scode, sname, descriptions) 
                           VALUES (@scode, @sname, @descriptions)";
                    cmd = new SqlCommand(sql, conn);
                }

                cmd.Parameters.AddWithValue("@scode", maLoai);
                cmd.Parameters.AddWithValue("@sname", tenLoai);
                cmd.Parameters.AddWithValue("@descriptions", string.IsNullOrEmpty(ghiChu) ? "" : ghiChu);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = id > 0
                ? "Cập nhật loại người dùng thành công!"
                : "Thêm loại người dùng mới thành công!";

            return RedirectToAction("LoaiNguoiDung");
        }

        [HttpPost]
        public IActionResult XoaLoaiNguoiDung(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM LoaiNguoiDung WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Xóa loại người dùng thành công!";
            return RedirectToAction("LoaiNguoiDung");
        }

        [HttpPost]
        public IActionResult LuuDanhMuc(IFormCollection form)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            int.TryParse(form["Id"], out int id);
            string? maLoai = form["MaLoai"];
            string? tenLoai = form["TenLoai"];
            string? ghiChu = form["GhiChu"];

            bool isMacDinh = form["MacDinh"] == "on";

            int macdinh = isMacDinh ? 1 : 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Nếu đặt là mặc định thì phải reset tất cả về 0 trước
                if (isMacDinh)
                {
                    string resetSql = "UPDATE DanhMuc SET sdefault = 0 WHERE sdefault = 1";
                    SqlCommand resetCmd = new SqlCommand(resetSql, conn);
                    resetCmd.ExecuteNonQuery();
                }

                SqlCommand cmd;

                if (id > 0)
                {
                    // Cập nhật
                    string sql = @"UPDATE DanhMuc 
                           SET scode = @scode, sname = @sname, sdefault = @sdefault, descriptions = @descriptions 
                           WHERE id = @id";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    // Thêm mới
                    string sql = @"INSERT INTO DanhMuc (scode, sname, sdefault, descriptions) 
                           VALUES (@scode, @sname, @sdefault, @descriptions)";
                    cmd = new SqlCommand(sql, conn);
                }

                cmd.Parameters.AddWithValue("@scode", maLoai);
                cmd.Parameters.AddWithValue("@sname", tenLoai);
                cmd.Parameters.AddWithValue("@sdefault", macdinh);
                cmd.Parameters.AddWithValue("@descriptions", string.IsNullOrEmpty(ghiChu) ? "" : ghiChu);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = id > 0
                ? "Cập nhật danh mục thành công!"
                : "Thêm danh mục mới thành công!";

            return RedirectToAction("DanhMuc");
        }

        [HttpPost]
        public IActionResult XoaDanhMuc(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM DanhMuc WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction("DanhMuc");
        }

        [HttpPost]
        public IActionResult LuuLoaiChiTieu(IFormCollection form)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            int.TryParse(form["Id"], out int id);
            string? maLoai = form["MaLoai"];
            string? tenLoai = form["TenLoai"];
            string? ghiChu = form["GhiChu"];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;

                if (id > 0)
                {
                    // Cập nhật
                    string sql = @"UPDATE LoaiChiTieu 
                           SET scode = @scode, sname = @sname, descriptions = @descriptions 
                           WHERE id = @id";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    // Thêm mới
                    string sql = @"INSERT INTO LoaiChiTieu (scode, sname, descriptions) 
                           VALUES (@scode, @sname, @descriptions)";
                    cmd = new SqlCommand(sql, conn);
                }

                cmd.Parameters.AddWithValue("@scode", maLoai);
                cmd.Parameters.AddWithValue("@sname", tenLoai);
                cmd.Parameters.AddWithValue("@descriptions", string.IsNullOrEmpty(ghiChu) ? "" : ghiChu);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = id > 0
                ? "Cập nhật loại chi tiêu thành công!"
                : "Thêm loại chi tiêu mới thành công!";

            return RedirectToAction("LoaiChiTieu");
        }

        [HttpPost]
        public IActionResult XoaLoaiChiTieu(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM LoaiChiTieu WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Xóa loại chi tiêu thành công!";
            return RedirectToAction("LoaiChiTieu");
        }

        [HttpPost]
        public IActionResult LuuLoaiTienTe(IFormCollection form)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            int.TryParse(form["Id"], out int id);
            string? maLoai = form["MaLoai"];
            string? tenLoai = form["TenLoai"];
            string? quyDoi = form["QuyDoi"];
            string? ghiChu = form["GhiChu"];

            bool isMacDinh = form["MacDinh"] == "on";

            int macdinh = isMacDinh ? 1 : 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Nếu đặt là mặc định thì phải reset tất cả về 0 trước
                if (isMacDinh)
                {
                    string resetSql = "UPDATE LoaiTienTe SET sdefault = 0 WHERE sdefault = 1";
                    SqlCommand resetCmd = new SqlCommand(resetSql, conn);
                    resetCmd.ExecuteNonQuery();
                }

                SqlCommand cmd;

                if (id > 0)
                {
                    // Cập nhật
                    string sql = @"UPDATE LoaiTienTe 
                           SET scode = @scode, sname = @sname, converts = @converts, sdefault = @sdefault, descriptions = @descriptions 
                           WHERE id = @id";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    // Thêm mới
                    string sql = @"INSERT INTO LoaiTienTe (scode, sname, converts, sdefault, descriptions) 
                           VALUES (@scode, @sname, @converts, @sdefault, @descriptions)";
                    cmd = new SqlCommand(sql, conn);
                }

                cmd.Parameters.AddWithValue("@scode", maLoai);
                cmd.Parameters.AddWithValue("@sname", tenLoai);
                cmd.Parameters.AddWithValue("@sdefault", macdinh);
                cmd.Parameters.AddWithValue("@converts", quyDoi);
                cmd.Parameters.AddWithValue("@descriptions", string.IsNullOrEmpty(ghiChu) ? "" : ghiChu);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = id > 0
                ? "Cập nhật loại tiền tệ thành công!"
                : "Thêm loại tiền tệ mới thành công!";

            return RedirectToAction("LoaiTienTe");
        }

        [HttpPost]
        public IActionResult XoaLoaiTienTe(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM LoaiTienTe WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Xóa loại tiền tệ thành công!";
            return RedirectToAction("LoaiTienTe");
        }

        [HttpPost]
        public IActionResult LuuNguoiDung(IFormCollection form)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            int.TryParse(form["Id"], out int id);
            string? fname = form["fname"];
            string? lname = form["lname"];
            string? gender = form["gender"];
            string? phone = form["phone"];
            string? email = form["email"];
            string? username = form["username"];
            string? ghiChu = form["GhiChu"];

            string? loainguoidung = form["id_loainguoidung"];
            if (string.IsNullOrEmpty(loainguoidung))
            {
                throw new InvalidOperationException("The 'id_loainguoidung' field is required.");
            }

            bool isStatus = form["status_account"] == "on";

            int status_account = isStatus ? 1 : 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;
                string pwd_default = "12345678Aa@";

                if (id > 0)
                {
                    // Cập nhật
                    string sql = @"UPDATE NguoiDung 
                           SET fname = @fname, lname = @lname, gender = @gender, phone = @phone , email = @email, username = @username, 
                                descriptions = @descriptions, status_account = @status_account, id_loainguoidung = @loainguoidung 
                           WHERE id = @id";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    // Thêm mới

                    string sql = @"INSERT INTO NguoiDung (fname, lname, gender, phone, email, username, descriptions, id_loainguoidung, status_account, pwd) 
                           VALUES (@fname, @lname, @gender, @phone, @email, @username, @descriptions, @loainguoidung, @status_account, @pwd_default)";
                    cmd = new SqlCommand(sql, conn);
                }

                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@lname", lname);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@status_account", status_account);
                cmd.Parameters.AddWithValue("@loainguoidung", loainguoidung);
                cmd.Parameters.AddWithValue("@pwd_default", pwd_default);
                cmd.Parameters.AddWithValue("@descriptions", string.IsNullOrEmpty(ghiChu) ? "" : ghiChu);

                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = id > 0
                ? "Cập nhật người dùng thành công!"
                : "Thêm người dùng mới thành công!";

            return RedirectToAction("DanhSachNguoiDung");
        }

        [HttpPost]
        public IActionResult XoaNguoiDung(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM NguoiDung WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Xóa người dùng thành công!";
            return RedirectToAction("DanhSachNguoiDung");
        }


        [HttpPost]
        public IActionResult LuuChiTieu(IFormCollection form, List<IFormFile> images)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Missing connection string.");
            
            int.TryParse(form["id"], out int id);
            string? titles = form["titles"];
            string? create_at = form["create_at"];
            int.TryParse(form["id_loaichitieu"], out int id_loaichitieu);
            int.TryParse(form["id_danhmuc"], out int id_danhmuc);
            int.TryParse(form["id_loaitiente"], out int id_loaitiente);
            decimal.TryParse(form["total"], out decimal total);
            string notes = form["notes"].ToString() ?? string.Empty;

            // Lấy id_user từ đăng nhập
            int? id_user = HttpContext.Session.GetInt32("id_user"); // cần lưu khi đăng nhập
            if (id_user == null) return RedirectToAction("Login", "Account"); // nếu chưa login

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd;
                int id_chitieu = id;

                if (id > 0)
                {
                    // Update ChiTieu
                    string sqlUpdate = @"
                UPDATE ChiTieu
                SET titles = @titles,
                    id_loaichitieu = @id_loaichitieu,
                    id_danhmuc = @id_danhmuc,
                    id_loaitiente = @id_loaitiente,
                    total = @total,
                    create_at = @create_at,
                    notes = @notes

                WHERE id = @id";
                    cmd = new SqlCommand(sqlUpdate, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                }
                else
                {
                    // Insert ChiTieu
                    string sqlInsert = @"
                INSERT INTO ChiTieu (titles, id_loaichitieu, id_danhmuc, id_user, id_loaitiente, total, create_at, notes)
                OUTPUT INSERTED.id
                VALUES (@titles, @id_loaichitieu, @id_danhmuc, @id_user, @id_loaitiente, @total, @create_at, @notes)";
                    cmd = new SqlCommand(sqlInsert, conn);
                    cmd.Parameters.AddWithValue("@id_user", id_user);
                }

                cmd.Parameters.AddWithValue("@titles", titles ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@id_loaichitieu", id_loaichitieu);
                cmd.Parameters.AddWithValue("@id_danhmuc", id_danhmuc);
                cmd.Parameters.AddWithValue("@id_loaitiente", id_loaitiente);
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@create_at", create_at);
                cmd.Parameters.AddWithValue("@notes", string.IsNullOrEmpty(notes) ? "" : notes);

                if (id > 0)
                {
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    // Lấy ID ChiTieu vừa tạo
                    id = (int)cmd.ExecuteScalar();
                }

                // Lưu ảnh nếu có
                if (images != null && images.Count > 0)
                {
                    foreach (var file in images)
                    {
                        if (file.Length > 0)
                        {
                            string uploads = Path.Combine(_environment.WebRootPath, "uploads");
                            if (!Directory.Exists(uploads))
                                Directory.CreateDirectory(uploads);

                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string filePath = Path.Combine(uploads, fileName);

                            // Sao chép ảnh vào thư mục uploads
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            // Lưu thông tin ảnh vào cơ sở dữ liệu
                            string sqlImage = @"INSERT INTO HinhAnh (id_chitieu, url_image, create_at, notes)
                                        VALUES (@id_chitieu, @url_image, GETDATE(), '')";
                            using (SqlCommand cmdImg = new SqlCommand(sqlImage, conn))
                            {
                                cmdImg.Parameters.AddWithValue("@id_chitieu", id);
                                cmdImg.Parameters.AddWithValue("@url_image", "/uploads/" + fileName); // Đường dẫn ảnh lưu trong DB
                                cmdImg.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            TempData["Success"] = id > 0 ? "Cập nhật thu - chi thành công!" : "Thêm thu - chi mới thành công!";
            return RedirectToAction("IncomeAndExpense");
        }

        [HttpPost]
        public IActionResult XoaChiTieu(int id)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM ChiTieu WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Xóa thông tin chi - tiêu thành công!";
            return RedirectToAction("IncomeAndExpense");
        }

        public JsonResult LayHinhAnhTheoChiTieu(int id)
        {
            List<string> imageUrls = new List<string>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM HinhAnh WHERE id_chitieu = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string? url = reader["url_image"]?.ToString(); // Use null-conditional operator
                    if (!string.IsNullOrEmpty(url)) // Check for null or empty before adding
                    {
                        imageUrls.Add(Url.Content("~" + url));
                    }
                }

                reader.Close();
            }

            return Json(imageUrls);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa session
            return RedirectToAction("Login", "Account"); // Chuyển hướng về trang login
        }

    }

    public class CategoryStats
    {
        public string? Name { get; set; }
        public int Count { get; set; }
    }
}
