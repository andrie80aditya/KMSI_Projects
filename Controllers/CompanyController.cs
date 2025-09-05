using KMSI_Projects.Data;
using KMSI_Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KMSI_Projects.Controllers
{
    [Authorize]
    public class CompanyController : BaseController
    {
        public CompanyController(ApplicationDbContext context, ILogger<CompanyController> logger)
            : base(context, logger)
        {
        }

        // GET: Company
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string? search = null)
        {
            try
            {
                if (!HasPermission("ViewAll") && !HasPermission("ViewCompany"))
                {
                    return UnauthorizedAccess();
                }

                var query = _context.Companies
                    .Include(c => c.ParentCompany)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(c => c.CompanyName.Contains(search) || c.CompanyCode.Contains(search));
                }

                // For non-super admin users, filter by their company hierarchy
                if (CurrentUserRole != "SuperAdmin")
                {
                    query = query.Where(c => c.CompanyId == CurrentCompanyId || c.ParentCompanyId == CurrentCompanyId);
                }

                var (skip, take) = GetPaginationParameters(page, pageSize);
                var totalCount = await query.CountAsync();

                var companies = await query
                    .OrderBy(c => c.CompanyName)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = totalCount;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                ViewBag.Search = search;

                return View(companies);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "loading companies");
            }
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                var company = await _context.Companies
                    .Include(c => c.ParentCompany)
                    .Include(c => c.ChildCompanies)
                    .Include(c => c.Sites)
                    .Include(c => c.Users)
                    .FirstOrDefaultAsync(c => c.CompanyId == id);

                if (company == null)
                {
                    return NotFound();
                }

                // Check permissions
                if (CurrentUserRole != "SuperAdmin" && company.CompanyId != CurrentCompanyId)
                {
                    return UnauthorizedAccess();
                }

                return View(company);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "loading company details");
            }
        }

        // GET: Company/Create
        public async Task<IActionResult> Create()
        {
            if (!HasPermission("CreateAll") && !HasPermission("CreateCompany"))
            {
                return UnauthorizedAccess();
            }

            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (!HasPermission("CreateAll") && !HasPermission("CreateCompany"))
            {
                return UnauthorizedAccess();
            }

            if (!IsValidModel())
            {
                await PopulateDropdownsAsync();
                return View(company);
            }

            try
            {
                // Check if company code already exists
                if (await _context.Companies.AnyAsync(c => c.CompanyCode == company.CompanyCode))
                {
                    ModelState.AddModelError("CompanyCode", "Company code already exists.");
                    await PopulateDropdownsAsync();
                    return View(company);
                }

                company.CreatedBy = CurrentUserId;
                company.CreatedDate = DateTime.Now;

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                // Log audit trail
                await LogAuditAsync("Companies", company.CompanyId, "Insert", null, company);

                SetSuccessMessage($"Company '{company.CompanyName}' created successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                SetErrorMessage("An error occurred while creating the company.");
                await PopulateDropdownsAsync();
                return View(company);
            }
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            if (!HasPermission("UpdateAll") && !HasPermission("UpdateCompany"))
            {
                return UnauthorizedAccess();
            }

            try
            {
                var company = await _context.Companies.FindAsync(id);

                if (company == null)
                {
                    return NotFound();
                }

                // Check permissions - users can only edit their own company
                if (CurrentUserRole != "SuperAdmin" && company.CompanyId != CurrentCompanyId)
                {
                    return UnauthorizedAccess();
                }

                await PopulateDropdownsAsync(company.ParentCompanyId);
                return View(company);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "loading company for edit");
            }
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (!HasPermission("UpdateAll") && !HasPermission("UpdateCompany"))
            {
                return UnauthorizedAccess();
            }

            if (!IsValidModel())
            {
                await PopulateDropdownsAsync(company.ParentCompanyId);
                return View(company);
            }

            try
            {
                var existingCompany = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.CompanyId == id);

                if (existingCompany == null)
                {
                    return NotFound();
                }

                // Check permissions
                if (CurrentUserRole != "SuperAdmin" && existingCompany.CompanyId != CurrentCompanyId)
                {
                    return UnauthorizedAccess();
                }

                // Check if company code already exists (excluding current company)
                if (await _context.Companies.AnyAsync(c => c.CompanyCode == company.CompanyCode && c.CompanyId != id))
                {
                    ModelState.AddModelError("CompanyCode", "Company code already exists.");
                    await PopulateDropdownsAsync(company.ParentCompanyId);
                    return View(company);
                }

                company.UpdatedBy = CurrentUserId;
                company.UpdatedDate = DateTime.Now;
                company.CreatedBy = existingCompany.CreatedBy;
                company.CreatedDate = existingCompany.CreatedDate;

                _context.Update(company);
                await _context.SaveChangesAsync();

                // Log audit trail
                await LogAuditAsync("Companies", company.CompanyId, "Update", existingCompany, company);

                SetSuccessMessage($"Company '{company.CompanyName}' updated successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CompanyExistsAsync(company.CompanyId))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating company");
                SetErrorMessage("An error occurred while updating the company.");
                await PopulateDropdownsAsync(company.ParentCompanyId);
                return View(company);
            }
        }

        // GET: Company/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            if (!HasPermission("DeleteAll") && !HasPermission("DeleteCompany"))
            {
                return UnauthorizedAccess();
            }

            try
            {
                var company = await _context.Companies
                    .Include(c => c.ParentCompany)
                    .FirstOrDefaultAsync(c => c.CompanyId == id);

                if (company == null)
                {
                    return NotFound();
                }

                // Check permissions
                if (CurrentUserRole != "SuperAdmin")
                {
                    return UnauthorizedAccess();
                }

                // Check if company has dependencies
                var hasChildCompanies = await _context.Companies.AnyAsync(c => c.ParentCompanyId == id);
                var hasSites = await _context.Sites.AnyAsync(s => s.CompanyId == id);
                var hasUsers = await _context.Users.AnyAsync(u => u.CompanyId == id);

                ViewBag.HasDependencies = hasChildCompanies || hasSites || hasUsers;
                ViewBag.DependencyMessage = GetDependencyMessage(hasChildCompanies, hasSites, hasUsers);

                return View(company);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "loading company for deletion");
            }
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasPermission("DeleteAll") && !HasPermission("DeleteCompany"))
            {
                return UnauthorizedAccess();
            }

            try
            {
                var company = await _context.Companies.FindAsync(id);

                if (company == null)
                {
                    return NotFound();
                }

                // Check permissions
                if (CurrentUserRole != "SuperAdmin")
                {
                    return UnauthorizedAccess();
                }

                // Check for dependencies
                var hasChildCompanies = await _context.Companies.AnyAsync(c => c.ParentCompanyId == id);
                var hasSites = await _context.Sites.AnyAsync(s => s.CompanyId == id);
                var hasUsers = await _context.Users.AnyAsync(u => u.CompanyId == id);

                if (hasChildCompanies || hasSites || hasUsers)
                {
                    SetErrorMessage("Cannot delete company with existing dependencies.");
                    return RedirectToAction(nameof(Delete), new { id });
                }

                var companyName = company.CompanyName;

                // Log audit trail before deletion
                await LogAuditAsync("Companies", company.CompanyId, "Delete", company, null);

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                SetSuccessMessage($"Company '{companyName}' deleted successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company");
                SetErrorMessage("An error occurred while deleting the company.");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // Helper methods
        private async Task PopulateDropdownsAsync(int? selectedParentCompanyId = null)
        {
            var companies = await _context.Companies
                .Where(c => c.IsActive)
                .OrderBy(c => c.CompanyName)
                .Select(c => new { c.CompanyId, c.CompanyName })
                .ToListAsync();

            ViewBag.ParentCompanyId = companies
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.CompanyId.ToString(),
                    Text = c.CompanyName,
                    Selected = c.CompanyId == selectedParentCompanyId
                })
                .ToList();
        }

        private async Task<bool> CompanyExistsAsync(int id)
        {
            return await _context.Companies.AnyAsync(c => c.CompanyId == id);
        }

        private string GetDependencyMessage(bool hasChildCompanies, bool hasSites, bool hasUsers)
        {
            var messages = new List<string>();

            if (hasChildCompanies) messages.Add("child companies");
            if (hasSites) messages.Add("sites");
            if (hasUsers) messages.Add("users");

            return $"This company has {string.Join(", ", messages)}. Please remove these dependencies before deleting.";
        }
    }
}
