using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuanLyChiTieu.Models;

namespace QuanLyChiTieu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
        }

        public IActionResult Index()
        {
            /*Kiểm tra đăng nhập trước, nếu chưa thì điều trang về login*/
            // Lấy id_user từ đăng nhập
            int? id_user = HttpContext.Session.GetInt32("id_user"); // cần lưu khi đăng nhập
            if (id_user == null) return RedirectToAction("Login", "Account"); // nếu chưa login

            List<ChiTieu> listChiTieu = new List<ChiTieu>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

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

            return View("Index", listChiTieu);
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
            return RedirectToAction("Index");
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
            return RedirectToAction("Index");
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

            return View("Report", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
