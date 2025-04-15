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
            var user = _context.NguoiDung
                        .FirstOrDefault(u =>
                            u.username != null &&
                            u.pwd != null &&
                            u.username.Trim().ToLower() == username.Trim().ToLower() &&
                            u.pwd.Trim() == password.Trim() &&
                            u.status_account == true);


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
