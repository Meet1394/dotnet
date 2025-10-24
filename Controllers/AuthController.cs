using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PersonalCloudDrive.Models;
using PersonalCloudDrive.Services;
using BCrypt.Net;

namespace PersonalCloudDrive.Controllers
{
    public class AuthController : Controller
    {
        private readonly SupabaseService _supabase;

        public AuthController(SupabaseService supabase)
        {
            _supabase = supabase;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match");
                return View(model);
            }

            try
            {
                var client = _supabase.GetClient();

                // Check if user exists
                var existingUser = await client.From<User>()
                    .Where(u => u.Email == model.Email)
                    .Get();

                if (existingUser.Models.Any())
                {
                    ModelState.AddModelError("", "Email already registered");
                    return View(model);
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Create user
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    FullName = model.FullName,
                    StorageUsedMB = 0,
                    StorageLimitMB = 1024,
                    CreatedAt = DateTime.UtcNow
                };

                await client.From<User>().Insert(user);

                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration failed: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _supabase.GetClient();

                // Get user by email
                var response = await client.From<User>()
                    .Where(u => u.Email == model.Email)
                    .Single();

                if (response == null)
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(model.Password, response.PasswordHash))
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, response.Id!),
                    new Claim(ClaimTypes.Email, response.Email),
                    new Claim(ClaimTypes.Name, response.FullName),
                    new Claim(ClaimTypes.Role, response.IsAdmin ? "Admin" : "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login failed: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}