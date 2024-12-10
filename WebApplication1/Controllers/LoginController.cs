using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using NETCore.Encrypt.Extensions;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _dbContext;

        public LoginController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Show the login form.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handle login submission.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre.");
                return View(model);

            }
            var hashedPass = (model.Password).MD5();
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == hashedPass);

            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid email or password.");
                return View(model);
            }

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            };

            // Create authentication properties
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
            if (user.Role == "Admin") {
                return RedirectToAction("Index", "Admin"); // Redirect to Admin dashboard
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Logout the user.
        /// </summary>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
