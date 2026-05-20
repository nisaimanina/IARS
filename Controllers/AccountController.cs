using IARS.Data;
using Microsoft.AspNetCore.Mvc;

namespace IARS.Controllers
{
    // NOTE: Login & Logout logic dipindah ke HomeController.
    // AccountController ini disimpan untuk kegunaan masa hadapan (e.g. profile, register).
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Redirect semua /Account/Login ke /Home/Login (canonical login page)
        public IActionResult Login()
        {
            return RedirectToAction("Login", "Home");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }
    }
}
