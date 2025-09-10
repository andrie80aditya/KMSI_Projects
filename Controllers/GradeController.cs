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
    /// Controller For Master Grade
    /// Manages grade/level definitions within companies
    /// </summary>
    [Authorize]
    public class GradeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GradeController> _logger;

        public GradeController(ApplicationDbContext context, ILogger<GradeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Grade
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

                // Get grades based on user's access level
                var grades = await _context.Grades
                    .Include(g => g.Company)
                    .Where(g => g.CompanyId == currentUser.CompanyId || g.Company.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(g => g.SortOrder)
                    .ThenBy(g => g.GradeName)
                    .ToListAsync();

                ViewBag.CurrentCompanyId = currentUser.CompanyId;
                return View(grades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading grades");
                TempData["ErrorMessage"] = "An error occurred while loading grades.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Grade/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Grade ID is required." });
            }

            try
            {
                var grade = await _context.Grades
                    .Include(g => g.Company)
                    .FirstOrDefaultAsync(m => m.GradeId == id);

                if (grade == null)
                {
                    return Json(new { success = false, message = "Grade not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Check access permissions
                if (grade.CompanyId != currentUser.CompanyId && grade.Company.ParentCompanyId != currentUser.CompanyId)
                {
                    return Json(new { success = false, message = "Access denied." });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        gradeId = grade.GradeId,
                        companyName = grade.Company?.CompanyName,
                        gradeCode = grade.GradeCode,
                        gradeName = grade.GradeName,
                        description = grade.Description,
                        duration = grade.Duration,
                        sortOrder = grade.SortOrder,
                        isActive = grade.IsActive,
                        createdDate = grade.CreatedDate.ToString("dd/MM/yyyy HH:mm")
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting grade details for grade {GradeId}", id);
                return Json(new { success = false, message = "Error loading grade details." });
            }
        }

        // GET: Grade/GetCreateForm
        public async Task<IActionResult> GetCreateForm()
        {
            try
            {
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
                    companies = companies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting create form data");
                return Json(new { success = false, message = "Error loading form data." });
            }
        }

        // POST: Grade/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] GradeViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    // Check if grade code already exists within the same company
                    var existingGrade = await _context.Grades
                        .FirstOrDefaultAsync(g => g.GradeCode == model.GradeCode && g.CompanyId == model.CompanyId);

                    if (existingGrade != null)
                    {
                        return Json(new { success = false, message = "Grade code already exists in this company." });
                    }

                    var grade = new Grade
                    {
                        CompanyId = model.CompanyId,
                        GradeCode = model.GradeCode.ToUpper(),
                        GradeName = model.GradeName,
                        Description = model.Description,
                        Duration = model.Duration,
                        SortOrder = model.SortOrder,
                        IsActive = model.IsActive,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    _context.Grades.Add(grade);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Grade {grade.GradeName} created by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Grade created successfully.",
                        data = new
                        {
                            gradeId = grade.GradeId,
                            gradeCode = grade.GradeCode,
                            gradeName = grade.GradeName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating grade");
                return Json(new { success = false, message = "An error occurred while creating the grade." });
            }
        }

        // GET: Grade/Edit/5
        public async Task<IActionResult> GetEditForm(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Grade ID is required." });
            }

            try
            {
                var grade = await _context.Grades
                    .Include(g => g.Company)
                    .FirstOrDefaultAsync(m => m.GradeId == id);

                if (grade == null)
                {
                    return Json(new { success = false, message = "Grade not found." });
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
                        gradeId = grade.GradeId,
                        companyId = grade.CompanyId,
                        gradeCode = grade.GradeCode,
                        gradeName = grade.GradeName,
                        description = grade.Description,
                        duration = grade.Duration,
                        sortOrder = grade.SortOrder,
                        isActive = grade.IsActive
                    },
                    companies = companies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting edit form data for grade {GradeId}", id);
                return Json(new { success = false, message = "Error loading grade data." });
            }
        }

        // POST: Grade/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] GradeViewModel model)
        {
            if (id != model.GradeId)
            {
                return Json(new { success = false, message = "Grade ID mismatch." });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    var grade = await _context.Grades.FindAsync(id);
                    if (grade == null)
                    {
                        return Json(new { success = false, message = "Grade not found." });
                    }

                    // Check if grade code already exists (excluding current grade)
                    var existingGrade = await _context.Grades
                        .FirstOrDefaultAsync(g => g.GradeCode == model.GradeCode &&
                                                g.CompanyId == model.CompanyId &&
                                                g.GradeId != id);

                    if (existingGrade != null)
                    {
                        return Json(new { success = false, message = "Grade code already exists in this company." });
                    }

                    grade.CompanyId = model.CompanyId;
                    grade.GradeCode = model.GradeCode.ToUpper();
                    grade.GradeName = model.GradeName;
                    grade.Description = model.Description;
                    grade.Duration = model.Duration;
                    grade.SortOrder = model.SortOrder;
                    grade.IsActive = model.IsActive;
                    grade.UpdatedBy = currentUserId;
                    grade.UpdatedDate = DateTime.Now;

                    _context.Update(grade);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Grade {grade.GradeName} updated by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Grade updated successfully.",
                        data = new
                        {
                            gradeId = grade.GradeId,
                            gradeCode = grade.GradeCode,
                            gradeName = grade.GradeName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating grade");
                return Json(new { success = false, message = "An error occurred while updating the grade." });
            }
        }

        // POST: Grade/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var grade = await _context.Grades.FindAsync(id);
                if (grade == null)
                {
                    return Json(new { success = false, message = "Grade not found." });
                }

                // Check if grade has related data
                var hasStudents = await _context.Students.AnyAsync(s => s.CurrentGradeId == id);
                var hasGradeBooks = await _context.GradeBooks.AnyAsync(gb => gb.GradeId == id);

                if (hasStudents || hasGradeBooks)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete grade because it has related students or books."
                    });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Grade {grade.GradeName} deleted by user {currentUserId}");

                return Json(new
                {
                    success = true,
                    message = "Grade deleted successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting grade with ID {GradeId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the grade." });
            }
        }

        private bool GradeExists(int id)
        {
            return _context.Grades.Any(e => e.GradeId == id);
        }
    }
}   