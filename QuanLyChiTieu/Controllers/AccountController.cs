using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuanLyChiTieu.Models;

namespace QuanLyChiTieu.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyChiTieuContext _context;
        private readonly IConfiguration _configuration;


        public AccountController(QuanLyChiTieuContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsUsernameInUse(string username)
        {
            var exists = _context.NguoiDung.Any(u => u.username == username);
            return Json(!exists);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsEmailInUse(string email)
        {
            var exists = _context.NguoiDung.Any(u => u.email == email);
            return Json(!exists);
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsPhoneInUse(string phone)
        {
            var exists = _context.NguoiDung.Any(u => u.phone == phone);
            return Json(!exists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                var existsUsername = _context.NguoiDung.Any(u => u.username == model.username);
                var existsEmail = _context.NguoiDung.Any(u => u.email == model.email);
                var existsPhone = _context.NguoiDung.Any(u => u.phone == model.phone);

                if (existsUsername || existsEmail || existsPhone)
                {
                    if (existsUsername)
                        ModelState.AddModelError("username", "Tên đăng nhập đã tồn tại.");
                    if (existsEmail)
                        ModelState.AddModelError("email", "Email đã tồn tại.");
                    if (existsPhone)
                        ModelState.AddModelError("phone", "Số điện thoại đã tồn tại.");

                    return View(model);
                }

                _context.NguoiDung.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Account", "Login");
            }

            return View(model);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT TOP 1 
                nd.id, nd.username, nd.pwd, nd.fname, nd.lname, nd.id_loainguoidung, lnd.scode
            FROM NguoiDung nd
            JOIN LoaiNguoiDung lnd ON nd.id_loainguoidung = lnd.id
            WHERE nd.username IS NOT NULL AND nd.pwd IS NOT NULL
                AND LTRIM(RTRIM(LOWER(nd.username))) = @username
                AND LTRIM(RTRIM(nd.pwd)) = @password
                AND nd.status_account = 1";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@username", username.Trim().ToLower());
                cmd.Parameters.AddWithValue("@password", password.Trim());

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int id_user = Convert.ToInt32(reader["id"]);
                    string? uname = reader["username"]?.ToString();
                    string? fname = reader["fname"]?.ToString();
                    string? lname = reader["lname"]?.ToString();
                    int id_loainguoidung = Convert.ToInt32(reader["id_loainguoidung"]);
                    string? scode = reader["scode"]?.ToString();

                    HttpContext.Session.SetInt32("id_user", id_user);

                    if (!string.IsNullOrEmpty(uname))
                        HttpContext.Session.SetString("username", uname);

                    HttpContext.Session.SetString("role", id_loainguoidung.ToString());

                    if (!string.IsNullOrEmpty(fname) && !string.IsNullOrEmpty(lname))
                        HttpContext.Session.SetString("fullname", fname + " " + lname);

                    // So sánh scode để phân quyền
                    HttpContext.Session.SetString("ma_quyenhan", scode);
                    if (!string.IsNullOrEmpty(scode) && scode.ToUpper() == "ADMIN")
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                    return View();
                }
            }
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}
