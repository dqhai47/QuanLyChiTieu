using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieu.Models;

namespace QuanLyChiTieu.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra nếu không phải admin thì chuyển về Login
            if (HttpContext.Session.GetString("role") != "1")
            {
                return RedirectToAction("Login", "Account");
            }

            return View("Index");
        }

        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
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

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT ct.id, ct.titles, ct.id_loaichitieu, ct.id_danhmuc, ct.id_loaitiente, ct.total, ct.create_at, ct.notes, " +
                    "lct.sname AS ten_loaichitieu, dm.sname AS ten_danhmuc, ltt.sname AS ten_loaitiente " +
                    "FROM ChiTieu ct " +
                    "JOIN LoaiChiTieu lct ON ct.id_loaichitieu = lct.id " +
                    "JOIN DanhMuc dm ON ct.id_danhmuc = dm.id " +
                    "JOIN LoaiTienTe ltt ON ct.id_loaitiente = ltt.id;";

                SqlCommand cmd = new SqlCommand(sql, conn);
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

        public IActionResult Report()
        {

            return View("Money/Report");
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

        
    }
}
