using KMSI_Projects.Data;
using KMSI_Projects.Models;
using KMSI_Projects.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KMSI_Projects.Controllers
{
    /// <summary>
    /// Controller For Master Site
    /// Manages site/branch locations within companies
    /// </summary>
    [Authorize]
    public class SiteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SiteController> _logger;

        public SiteController(ApplicationDbContext context, ILogger<SiteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Site
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Get sites based on user's access level
                var sites = await _context.Sites
                    .Include(s => s.Company)
                    .Where(s => s.CompanyId == currentUser.CompanyId || s.Company.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(s => s.SiteName)
                    .ToListAsync();

                ViewBag.CurrentCompanyId = currentUser.CompanyId;
                return View(sites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sites");
                TempData["ErrorMessage"] = "An error occurred while loading sites.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Site/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Site ID is required." });
            }

            try
            {
                var site = await _context.Sites
                    .Include(s => s.Company)
                    .FirstOrDefaultAsync(m => m.SiteId == id);

                if (site == null)
                {
                    return Json(new { success = false, message = "Site not found." });
                }

                var siteDetails = new
                {
                    siteId = site.SiteId,
                    companyName = site.Company.CompanyName,
                    siteCode = site.SiteCode,
                    siteName = site.SiteName,
                    address = site.Address,
                    city = site.City,
                    province = site.Province,
                    phone = site.Phone,
                    email = site.Email,
                    managerName = site.ManagerName,
                    isActive = site.IsActive,
                    statusDisplay = site.StatusDisplay,
                    fullAddress = site.FullAddress,
                    createdDate = site.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                    updatedDate = site.UpdatedDate?.ToString("dd/MM/yyyy HH:mm")
                };

                return Json(new { success = true, data = siteDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting site details for ID {SiteId}", id);
                return Json(new { success = false, message = "Error loading site details." });
            }
        }

        // GET: Site/GetCreateForm
        public async Task<IActionResult> GetCreateForm()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Get available companies for dropdown
                var companies = await _context.Companies
                    .Where(c => c.IsActive &&
                               (c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId))
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    companies = companies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting create form data");
                return Json(new { success = false, message = "Error loading form data." });
            }
        }

        // POST: Site/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] SiteViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    // Check if site code already exists within the same company
                    var existingSite = await _context.Sites
                        .FirstOrDefaultAsync(s => s.SiteCode == model.SiteCode && s.CompanyId == model.CompanyId);

                    if (existingSite != null)
                    {
                        return Json(new { success = false, message = "Site code already exists in this company." });
                    }

                    var site = new Site
                    {
                        CompanyId = model.CompanyId,
                        SiteCode = model.SiteCode.ToUpper(),
                        SiteName = model.SiteName,
                        Address = model.Address,
                        City = model.City,
                        Province = model.Province,
                        Phone = model.Phone,
                        Email = model.Email,
                        ManagerName = model.ManagerName,
                        IsActive = model.IsActive,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    _context.Sites.Add(site);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Site {site.SiteName} created by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Site created successfully.",
                        data = new
                        {
                            siteId = site.SiteId,
                            siteCode = site.SiteCode,
                            siteName = site.SiteName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating site");
                return Json(new { success = false, message = "An error occurred while creating the site." });
            }
        }

        // GET: Site/Edit/5
        public async Task<IActionResult> GetEditForm(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Site ID is required." });
            }

            try
            {
                var site = await _context.Sites
                    .Include(s => s.Company)
                    .FirstOrDefaultAsync(m => m.SiteId == id);

                if (site == null)
                {
                    return Json(new { success = false, message = "Site not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

                var companies = await _context.Companies
                    .Where(c => c.IsActive &&
                               (c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId))
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        siteId = site.SiteId,
                        companyId = site.CompanyId,
                        siteCode = site.SiteCode,
                        siteName = site.SiteName,
                        address = site.Address,
                        city = site.City,
                        province = site.Province,
                        phone = site.Phone,
                        email = site.Email,
                        managerName = site.ManagerName,
                        isActive = site.IsActive
                    },
                    companies = companies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting edit form data for site {SiteId}", id);
                return Json(new { success = false, message = "Error loading site data." });
            }
        }

        // POST: Site/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] SiteViewModel model)
        {
            if (id != model.SiteId)
            {
                return Json(new { success = false, message = "Site ID mismatch." });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    var site = await _context.Sites.FindAsync(id);
                    if (site == null)
                    {
                        return Json(new { success = false, message = "Site not found." });
                    }

                    // Check if site code already exists (excluding current site)
                    var existingSite = await _context.Sites
                        .FirstOrDefaultAsync(s => s.SiteCode == model.SiteCode &&
                                                s.CompanyId == model.CompanyId &&
                                                s.SiteId != id);

                    if (existingSite != null)
                    {
                        return Json(new { success = false, message = "Site code already exists in this company." });
                    }

                    site.CompanyId = model.CompanyId;
                    site.SiteCode = model.SiteCode.ToUpper();
                    site.SiteName = model.SiteName;
                    site.Address = model.Address;
                    site.City = model.City;
                    site.Province = model.Province;
                    site.Phone = model.Phone;
                    site.Email = model.Email;
                    site.ManagerName = model.ManagerName;
                    site.IsActive = model.IsActive;
                    site.UpdatedBy = currentUserId;
                    site.UpdatedDate = DateTime.Now;

                    _context.Update(site);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Site {site.SiteName} updated by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Site updated successfully.",
                        data = new
                        {
                            siteId = site.SiteId,
                            siteCode = site.SiteCode,
                            siteName = site.SiteName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating site");
                return Json(new { success = false, message = "An error occurred while updating the site." });
            }
        }

        // POST: Site/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var site = await _context.Sites.FindAsync(id);
                if (site == null)
                {
                    return Json(new { success = false, message = "Site not found." });
                }

                // Check if site has related data
                var hasUsers = await _context.Users.AnyAsync(u => u.SiteId == id);
                var hasStudents = await _context.Students.AnyAsync(s => s.SiteId == id);
                var hasTeachers = await _context.Teachers.AnyAsync(t => t.SiteId == id);

                if (hasUsers || hasStudents || hasTeachers)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete site because it has related users, students, or teachers."
                    });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                _context.Sites.Remove(site);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Site {site.SiteName} deleted by user {currentUserId}");

                return Json(new
                {
                    success = true,
                    message = "Site deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting site with ID {SiteId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the site." });
            }
        }

        private bool SiteExists(int id)
        {
            return _context.Sites.Any(e => e.SiteId == id);
        }
    }
}
