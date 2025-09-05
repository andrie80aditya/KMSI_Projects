using KMSI_Projects.Data;
using KMSI_Projects.Models.ViewModels;
using KMSI_Projects.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KMSI_Projects.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, IUserService userService, ILogger<AccountController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.ReturnUrl = returnUrl;
                return View(new LoginViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading login page");
                ViewBag.ReturnUrl = returnUrl;
                return View(new LoginViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            try
            {
                // Debug logging
                _logger.LogInformation($"Login attempt - Username: '{model?.Username}', Password length: {model?.Password?.Length ?? 0}, RememberMe: {model?.RememberMe}");

                // Check if model is null
                if (model == null)
                {
                    _logger.LogError("LoginViewModel is null");
                    ModelState.AddModelError(string.Empty, "Invalid form data.");
                    return View(new LoginViewModel());
                }

                // Manual model validation
                if (string.IsNullOrWhiteSpace(model.Username))
                {
                    ModelState.AddModelError("Username", "Username is required");
                }

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Password is required");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"Model validation failed. Errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                    return View(model);
                }

                var user = await _userService.AuthenticateAsync(model.Username, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    _logger.LogWarning($"Failed login attempt for username: {model.Username}");
                    return View(model);
                }

                var userLevel = await _context.UserLevels.FirstOrDefaultAsync(x => x.UserLevelId == user.UserLevelId);
                string levelUser = userLevel != null ? userLevel.LevelName : string.Empty;

                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, levelUser),
                    new Claim("CompanyId", user.CompanyId.ToString()),
                    new Claim("CompanyName", user.Company?.CompanyName ?? ""),
                    new Claim("Username", user.Username)
                };

                if (user.SiteId.HasValue)
                {
                    claims.Add(new Claim("SiteId", user.SiteId.Value.ToString()));
                    claims.Add(new Claim("SiteName", user.Site?.SiteName ?? ""));
                }

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(30)
                        : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync("CookieAuth", claimsPrincipal, authProperties);

                _logger.LogInformation($"User {user.Username} logged in successfully");

                // Update last login date
                user.UpdateLastLogin();
                await _context.SaveChangesAsync();

                // Redirect to return URL or dashboard
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process for user: {Username}", model?.Username);
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model ?? new LoginViewModel());
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var username = User.FindFirst("Username")?.Value;

                await HttpContext.SignOutAsync("CookieAuth");

                _logger.LogInformation($"User {username} logged out");

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout process");
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _userService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                ModelState.AddModelError(string.Empty, "An error occurred while changing password.");
                return View(model);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _userService.GetByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                var model = new ProfileViewModel
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? "",
                    Phone = user.Phone ?? "",
                    CompanyName = user.Company?.CompanyName ?? "",
                    SiteName = user.Site?.SiteName ?? "",
                    UserLevelId = user.UserLevelId,
                    LastLoginDate = user.LastLoginDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user profile");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _userService.GetByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Phone = model.Phone;

                await _userService.UpdateUserAsync(user);

                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                ModelState.AddModelError(string.Empty, "An error occurred while updating profile.");
                return View(model);
            }
        }
    }
}
