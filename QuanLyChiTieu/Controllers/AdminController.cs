using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult DanhSachNguoiDung()
        {
            List<NguoiDung> list = new List<NguoiDung>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT nd.id, nd.fname, nd.lname, nd.email, nd.username, nd.phone, nd.gender, nd.id_loainguoidung, nd.descriptions, lnd.sname AS ten_loai FROM NguoiDung nd JOIN LoaiNguoiDung lnd ON nd.id_loainguoidung = lnd.id"; // câu lệnh SQL đầy đủ của bạn
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
                        ten_loai = reader["ten_loai"].ToString(), // Fix ambiguity by explicitly specifying the property
                        descriptions = reader["descriptions"].ToString()
                    };

                    list.Add(nd);
                }
            }

            return View(list);
        }

        public IActionResult LoaiNguoiDung()
        {
            List<LoaiNguoiDung> list = new List<LoaiNguoiDung>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

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
                        sname = reader["sname"].ToString(),
                        scode = reader["scode"].ToString(),
                        descriptions = reader["descriptions"].ToString()
                    };

                    list.Add(lnd);
                }
            }

            return View(list);
        }

        public IActionResult DanhMuc()
        {
            List<DanhMuc> list = new List<DanhMuc>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

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
                        sname = reader["sname"].ToString(),
                        scode = reader["scode"].ToString(),
                        descriptions = reader["descriptions"].ToString()
                    };

                    list.Add(danhmuc);
                }
            }

            return View(list);
        }

        public IActionResult PhanLoai()
        {
            List<PhanLoai> list = new List<PhanLoai>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM PhanLoai";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PhanLoai phanloai = new PhanLoai
                    {
                        id = Convert.ToInt32(reader["id"]),
                        sname = reader["sname"].ToString(),
                        scode = reader["scode"].ToString(),
                        descriptions = reader["descriptions"].ToString()
                    };

                    list.Add(phanloai);
                }
            }

            return View(list);
        }

        public IActionResult LoaiTienTe()
        {
            List<LoaiTienTe> list = new List<LoaiTienTe>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

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
                        sname = reader["sname"].ToString(),
                        scode = reader["scode"].ToString(),
                        converts = reader["converts"].ToString(),
                        descriptions = reader["descriptions"].ToString()
                    };

                    list.Add(loaitiente);
                }
            }

            return View(list);
        }


    }
}
