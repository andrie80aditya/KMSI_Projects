using KMSI_Projects.Data;
using KMSI_Projects.Models;
using KMSI_Projects.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KMSI_Projects.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ApplicationDbContext context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Company
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

                // Get companies based on user's access level
                var companies = await _context.Companies
                    .Include(c => c.ParentCompany)
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                ViewBag.CurrentCompanyId = currentUser.CompanyId;
                return View(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading companies");
                TempData["ErrorMessage"] = "An error occurred while loading companies.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.ParentCompany)
                .Include(c => c.Sites)
                .FirstOrDefaultAsync(m => m.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    companyId = company.CompanyId,
                    parentCompanyId = company.ParentCompanyId,
                    companyCode = company.CompanyCode,
                    companyName = company.CompanyName,
                    address = company.Address,
                    city = company.City,
                    province = company.Province,
                    phone = company.Phone,
                    email = company.Email,
                    isHeadOffice = company.IsHeadOffice,
                    isActive = company.IsActive,
                    parentCompanyName = company.ParentCompany?.CompanyName,
                    sitesCount = company.Sites?.Count ?? 0,
                    createdDate = company.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                    updatedDate = company.UpdatedDate?.ToString("dd/MM/yyyy HH:mm")
                }
            });
        }

        // GET: Company/Create
        public async Task<IActionResult> GetCreateForm()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

                var parentCompanies = await _context.Companies
                    .Where(c => c.IsActive && (c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId))
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    parentCompanies = parentCompanies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting create form data");
                return Json(new { success = false, message = "Error loading form data." });
            }
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CompanyViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    // Check if company code already exists
                    var existingCompany = await _context.Companies
                        .FirstOrDefaultAsync(c => c.CompanyCode == model.CompanyCode);

                    if (existingCompany != null)
                    {
                        return Json(new { success = false, message = "Company code already exists." });
                    }

                    var company = new Company
                    {
                        ParentCompanyId = model.ParentCompanyId,
                        CompanyCode = model.CompanyCode.ToUpper(),
                        CompanyName = model.CompanyName,
                        Address = model.Address,
                        City = model.City,
                        Province = model.Province,
                        Phone = model.Phone,
                        Email = model.Email,
                        IsHeadOffice = model.IsHeadOffice,
                        IsActive = model.IsActive,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Company {company.CompanyName} created by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Company created successfully.",
                        data = new
                        {
                            companyId = company.CompanyId,
                            companyCode = company.CompanyCode,
                            companyName = company.CompanyName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return Json(new { success = false, message = "An error occurred while creating the company." });
            }
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> GetEditForm(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Company ID is required." });
            }

            try
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(m => m.CompanyId == id);

                if (company == null)
                {
                    return Json(new { success = false, message = "Company not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

                var parentCompanies = await _context.Companies
                    .Where(c => c.IsActive && c.CompanyId != id &&
                               (c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId))
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        companyId = company.CompanyId,
                        parentCompanyId = company.ParentCompanyId,
                        companyCode = company.CompanyCode,
                        companyName = company.CompanyName,
                        address = company.Address,
                        city = company.City,
                        province = company.Province,
                        phone = company.Phone,
                        email = company.Email,
                        isHeadOffice = company.IsHeadOffice,
                        isActive = company.IsActive
                    },
                    parentCompanies = parentCompanies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting edit form data for company {CompanyId}", id);
                return Json(new { success = false, message = "Error loading company data." });
            }
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] CompanyViewModel model)
        {
            if (id != model.CompanyId)
            {
                return Json(new { success = false, message = "Company ID mismatch." });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    var company = await _context.Companies.FindAsync(id);
                    if (company == null)
                    {
                        return Json(new { success = false, message = "Company not found." });
                    }

                    // Check if company code already exists (excluding current company)
                    var existingCompany = await _context.Companies
                        .FirstOrDefaultAsync(c => c.CompanyCode == model.CompanyCode && c.CompanyId != id);

                    if (existingCompany != null)
                    {
                        return Json(new { success = false, message = "Company code already exists." });
                    }

                    company.ParentCompanyId = model.ParentCompanyId;
                    company.CompanyCode = model.CompanyCode.ToUpper();
                    company.CompanyName = model.CompanyName;
                    company.Address = model.Address;
                    company.City = model.City;
                    company.Province = model.Province;
                    company.Phone = model.Phone;
                    company.Email = model.Email;
                    company.IsHeadOffice = model.IsHeadOffice;
                    company.IsActive = model.IsActive;
                    company.UpdatedBy = currentUserId;
                    company.UpdatedDate = DateTime.Now;

                    _context.Update(company);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Company {company.CompanyName} updated by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Company updated successfully.",
                        data = new
                        {
                            companyId = company.CompanyId,
                            companyCode = company.CompanyCode,
                            companyName = company.CompanyName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating company {CompanyId}", id);
                return Json(new { success = false, message = "The company was modified by another user. Please refresh and try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company {CompanyId}", id);
                return Json(new { success = false, message = "An error occurred while updating the company." });
            }
        }

        // POST: Company/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.Sites)
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.CompanyId == id);

                if (company == null)
                {
                    return Json(new { success = false, message = "Company not found." });
                }

                // Check if company has active sites or users
                if (company.Sites.Any(s => s.IsActive) || company.Users.Any(u => u.IsActive))
                {
                    return Json(new { success = false, message = "Cannot delete company with active sites or users." });
                }

                // Soft delete - just deactivate
                company.IsActive = false;
                company.UpdatedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                company.UpdatedDate = DateTime.Now;

                _context.Update(company);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Company {company.CompanyName} deactivated by user {company.UpdatedBy}");

                return Json(new { success = true, message = "Company deactivated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company {CompanyId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the company." });
            }
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}
