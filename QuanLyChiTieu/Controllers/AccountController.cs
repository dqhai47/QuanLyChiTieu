using Microsoft.AspNetCore.Mvc;
using QuanLyChiTieu.Models;

namespace QuanLyChiTieu.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyChiTieuContext _context;

        public AccountController(QuanLyChiTieuContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public IActionResult Register(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                var existUser = _context.NguoiDung.FirstOrDefault(x => x.username == model.username);
                if (existUser != null)
                {
                    ModelState.AddModelError("username", "Tên đăng nhập đã tồn tại!");
                    return View(model);
                }

                // Gán role mặc định là người dùng thường (vd: ID = 2)
                model.id_loainguoidung = 2;
                model.create_at = DateTime.Now;

                _context.NguoiDung.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Login");
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
            var user = _context.NguoiDung
                .FirstOrDefault(u => u.username == username && u.pwd == password && u.status_account == true);

            if (user != null)
            {
                // Lưu thông tin đăng nhập
                HttpContext.Session.SetString("username", user.username);
                HttpContext.Session.SetString("role", user.id_loainguoidung.ToString());
                HttpContext.Session.SetString("fullname", user.fname + " " + user.lname);

                // Điều hướng tùy loại người dùng
                if (user.id_loainguoidung == 1) // giả sử ID = 1 là Admin
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
