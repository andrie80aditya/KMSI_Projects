using KMSI_Projects.Data;
using KMSI_Projects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace KMSI_Projects.Controllers
{
    /// <summary>
    /// Base controller with common functionality for all controllers
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger _logger;

        protected BaseController(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        // Current user information
        protected int CurrentUserId
        {
            get
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userIdClaim, out int userId) ? userId : 0;
            }
        }

        protected int CurrentCompanyId
        {
            get
            {
                var companyIdClaim = User.FindFirst("CompanyId")?.Value;
                return int.TryParse(companyIdClaim, out int companyId) ? companyId : 0;
            }
        }

        protected int? CurrentSiteId
        {
            get
            {
                var siteIdClaim = User.FindFirst("SiteId")?.Value;
                return int.TryParse(siteIdClaim, out int siteId) ? siteId : null;
            }
        }

        protected string CurrentUserRole
        {
            get
            {
                return User.FindFirst(ClaimTypes.Role)?.Value ?? "Guest";
            }
        }

        protected string CurrentUserName
        {
            get
            {
                return User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            }
        }

        // Helper methods for common operations
        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }

        protected void SetWarningMessage(string message)
        {
            TempData["WarningMessage"] = message;
        }

        protected void SetInfoMessage(string message)
        {
            TempData["InfoMessage"] = message;
        }

        // Override methods for audit logging
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Set current user context for audit logging
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.CurrentCompanyId = CurrentCompanyId;
            ViewBag.CurrentSiteId = CurrentSiteId;
            ViewBag.CurrentUserRole = CurrentUserRole;
            ViewBag.CurrentUserName = CurrentUserName;

            base.OnActionExecuting(context);
        }

        // Audit logging helper
        protected async Task LogAuditAsync(string tableName, int recordId, string action,
            object? oldValues = null, object? newValues = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    CompanyId = CurrentCompanyId,
                    UserId = CurrentUserId,
                    TableName = tableName,
                    RecordId = recordId,
                    Action = action,
                    OldValues = oldValues != null ? System.Text.Json.JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? System.Text.Json.JsonSerializer.Serialize(newValues) : null,
                    IPAddress = GetClientIPAddress(),
                    UserAgent = Request.Headers["User-Agent"].ToString()
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit trail");
            }
        }

        // Get client IP address
        private string GetClientIPAddress()
        {
            var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
            {
                ipAddress = Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress ?? "Unknown";
        }

        // Authorization helpers
        protected bool HasPermission(string permission)
        {
            // Implement your permission logic here
            var role = CurrentUserRole;

            // Super Admin has all permissions
            if (role == "SuperAdmin") return true;

            // Define role-based permissions
            var rolePermissions = new Dictionary<string, List<string>>
            {
                ["Admin"] = new List<string> { "ViewAll", "CreateAll", "UpdateAll", "DeleteAll" },
                ["SiteAdmin"] = new List<string> { "ViewSite", "CreateSite", "UpdateSite", "DeleteSite" },
                ["Teacher"] = new List<string> { "ViewTeacher", "UpdateTeacher" },
                ["Staff"] = new List<string> { "ViewStaff" }
            };

            return rolePermissions.ContainsKey(role) && rolePermissions[role].Contains(permission);
        }

        protected IActionResult UnauthorizedAccess()
        {
            SetErrorMessage("You don't have permission to perform this action.");
            return RedirectToAction("Index", "Home");
        }

        // Common error handling
        protected IActionResult HandleException(Exception ex, string operation = "operation")
        {
            _logger.LogError(ex, $"Error during {operation}");
            SetErrorMessage($"An error occurred during {operation}. Please try again.");
            return View("Error");
        }

        // Data validation helpers
        protected bool IsValidModel()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) });

                SetErrorMessage("Please correct the validation errors and try again.");
                return false;
            }
            return true;
        }

        // Pagination helper
        protected (int skip, int take) GetPaginationParameters(int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, Math.Min(100, pageSize)); // Limit max page size to 100

            return ((page - 1) * pageSize, pageSize);
        }   
    }
}
