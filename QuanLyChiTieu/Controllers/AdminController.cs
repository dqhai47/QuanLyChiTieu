using Microsoft.AspNetCore.Mvc;

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
    }
}
