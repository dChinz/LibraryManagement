using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password, bool rememberMe)
        {
            if (username == "admin" && password == "123456")
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(
                "",
                "Tên đăng nhập hoặc mật khẩu không chính xác.");

            return View();
        }
    }
}