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
    /// Controller For Master Teacher
    /// Manages teacher information within companies and sites
    /// </summary>
    [Authorize]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ApplicationDbContext context, ILogger<TeacherController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Teacher
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

                // Get teachers based on user's access level - mengikuti pola Site
                var teachers = await _context.Teachers
                    .Include(t => t.User)
                    .Include(t => t.Company)
                    .Include(t => t.Site)
                    .Where(t => t.CompanyId == currentUser.CompanyId || t.Company.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(t => t.TeacherCode)
                    .ToListAsync();

                ViewBag.CurrentCompanyId = currentUser.CompanyId;
                return View(teachers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teachers");
                TempData["ErrorMessage"] = "An error occurred while loading teachers.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Teacher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Teacher ID is required." });
            }

            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.User)
                    .Include(t => t.Company)
                    .Include(t => t.Site)
                    .FirstOrDefaultAsync(m => m.TeacherId == id);

                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                var viewModel = new TeacherDetailsViewModel
                {
                    TeacherId = teacher.TeacherId,
                    CompanyName = teacher.Company.CompanyName,
                    SiteName = teacher.Site.SiteName,
                    TeacherCode = teacher.TeacherCode,
                    UserName = teacher.User.FullName,
                    UserEmail = teacher.User.Email,
                    UserPhone = teacher.User.Phone,
                    Specialization = teacher.Specialization,
                    ExperienceYears = teacher.ExperienceYears,
                    HourlyRate = teacher.HourlyRate,
                    MaxStudentsPerDay = teacher.MaxStudentsPerDay,
                    IsAvailableForTrial = teacher.IsAvailableForTrial,
                    IsActive = teacher.IsActive,
                    CreatedDate = teacher.CreatedDate,
                    UpdatedDate = teacher.UpdatedDate
                };

                return Json(new { success = true, data = viewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher details");
                return Json(new { success = false, message = "An error occurred while retrieving teacher details." });
            }
        }

        // GET: Teacher/GetCreateForm
        public async Task<IActionResult> GetCreateForm()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                // Get available users (not already assigned as teachers)
                var assignedUserIds = await _context.Teachers
                    .Where(t => t.IsActive)
                    .Select(t => t.UserId)
                    .ToListAsync();

                var availableUsers = await _context.Users
                    .Where(u => u.IsActive && u.CompanyId == currentUser.CompanyId && !assignedUserIds.Contains(u.UserId))
                    .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName, u.Email })
                    .OrderBy(u => u.FullName)
                    .ToListAsync();

                // Get companies for dropdown
                var companies = await _context.Companies
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    users = availableUsers,
                    companies = companies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form data");
                return Json(new { success = false, message = "Error loading form data." });
            }
        }

        // GET: Teacher/GetSitesByCompany/5
        public async Task<IActionResult> GetSitesByCompany(int companyId)
        {
            try
            {
                var sites = await _context.Sites
                    .Where(s => s.CompanyId == companyId && s.IsActive)
                    .Select(s => new { s.SiteId, s.SiteName })
                    .OrderBy(s => s.SiteName)
                    .ToListAsync();

                return Json(new { success = true, sites = sites });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sites for company {CompanyId}", companyId);
                return Json(new { success = false, message = "Error loading sites." });
            }
        }

        // POST: Teacher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] TeacherViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    // Check if teacher code already exists within the same company
                    var existingTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.TeacherCode == model.TeacherCode && t.CompanyId == model.CompanyId);

                    if (existingTeacher != null)
                    {
                        return Json(new { success = false, message = "Teacher code already exists in this company." });
                    }

                    // Check if user is already assigned to another teacher
                    var userAlreadyTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.UserId == model.UserId && t.IsActive);

                    if (userAlreadyTeacher != null)
                    {
                        return Json(new { success = false, message = "This user is already assigned to another teacher." });
                    }

                    var teacher = new Teacher
                    {
                        UserId = model.UserId,
                        CompanyId = model.CompanyId,
                        SiteId = model.SiteId,
                        TeacherCode = model.TeacherCode.ToUpper(),
                        Specialization = model.Specialization,
                        ExperienceYears = model.ExperienceYears,
                        HourlyRate = model.HourlyRate,
                        MaxStudentsPerDay = model.MaxStudentsPerDay,
                        IsAvailableForTrial = model.IsAvailableForTrial,
                        IsActive = model.IsActive,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Teacher {teacher.TeacherCode} created by user {currentUserId}");

                    return Json(new
                    {
                        success = true,
                        message = "Teacher created successfully.",
                        data = new
                        {
                            teacherId = teacher.TeacherId,
                            teacherCode = teacher.TeacherCode,
                            userName = model.UserName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher");
                return Json(new { success = false, message = "An error occurred while creating the teacher." });
            }
        }

        // GET: Teacher/Edit/5
        public async Task<IActionResult> GetEditForm(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Teacher ID is required." });
            }

            try
            {
                var teacher = await _context.Teachers
                    .Include(t => t.User)
                    .Include(t => t.Company)
                    .Include(t => t.Site)
                    .FirstOrDefaultAsync(t => t.TeacherId == id);

                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Get available users plus current teacher's user
                var assignedUserIds = await _context.Teachers
                    .Where(t => t.IsActive && t.TeacherId != id)
                    .Select(t => t.UserId)
                    .ToListAsync();

                var availableUsers = await _context.Users
                    .Where(u => u.IsActive && u.CompanyId == currentUser.CompanyId && (!assignedUserIds.Contains(u.UserId) || u.UserId == teacher.UserId))
                    .Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName, u.Email })
                    .OrderBy(u => u.FullName)
                    .ToListAsync();

                // Get companies for dropdown
                var companies = await _context.Companies
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                // Get sites for selected company
                var sites = await _context.Sites
                    .Where(s => s.CompanyId == teacher.CompanyId && s.IsActive)
                    .Select(s => new { s.SiteId, s.SiteName })
                    .OrderBy(s => s.SiteName)
                    .ToListAsync();

                var viewModel = new TeacherViewModel
                {
                    TeacherId = teacher.TeacherId,
                    UserId = teacher.UserId,
                    CompanyId = teacher.CompanyId,
                    SiteId = teacher.SiteId,
                    TeacherCode = teacher.TeacherCode,
                    Specialization = teacher.Specialization,
                    ExperienceYears = teacher.ExperienceYears,
                    HourlyRate = teacher.HourlyRate,
                    MaxStudentsPerDay = teacher.MaxStudentsPerDay,
                    IsAvailableForTrial = teacher.IsAvailableForTrial,
                    IsActive = teacher.IsActive
                };

                return Json(new
                {
                    success = true,
                    teacher = viewModel,
                    users = availableUsers,
                    companies = companies,
                    sites = sites
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for teacher {TeacherId}", id);
                return Json(new { success = false, message = "Error loading teacher data." });
            }
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] TeacherViewModel model)
        {
            if (id != model.TeacherId)
            {
                return Json(new { success = false, message = "Teacher ID mismatch." });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    var teacher = await _context.Teachers.FindAsync(id);
                    if (teacher == null)
                    {
                        return Json(new { success = false, message = "Teacher not found." });
                    }

                    // Check if teacher code already exists (excluding current teacher)
                    var existingTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.TeacherCode == model.TeacherCode && t.TeacherId != id && t.CompanyId == model.CompanyId);

                    if (existingTeacher != null)
                    {
                        return Json(new { success = false, message = "Teacher code already exists in this company." });
                    }

                    // Check if user is already assigned to another teacher
                    var userAlreadyTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.UserId == model.UserId && t.TeacherId != id && t.IsActive);

                    if (userAlreadyTeacher != null)
                    {
                        return Json(new { success = false, message = "This user is already assigned to another teacher." });
                    }

                    // Update teacher properties
                    teacher.UserId = model.UserId;
                    teacher.CompanyId = model.CompanyId;
                    teacher.SiteId = model.SiteId;
                    teacher.TeacherCode = model.TeacherCode.ToUpper();
                    teacher.Specialization = model.Specialization;
                    teacher.ExperienceYears = model.ExperienceYears;
                    teacher.HourlyRate = model.HourlyRate;
                    teacher.MaxStudentsPerDay = model.MaxStudentsPerDay;
                    teacher.IsAvailableForTrial = model.IsAvailableForTrial;
                    teacher.IsActive = model.IsActive;
                    teacher.UpdatedBy = currentUserId;
                    teacher.UpdatedDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Teacher updated successfully: {TeacherCode} by user {UserId}", teacher.TeacherCode, currentUserId);

                    return Json(new { success = true, message = "Teacher updated successfully." });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating teacher {TeacherId}", id);
                return Json(new { success = false, message = "An error occurred while updating the teacher." });
            }
        }

        // POST: Teacher/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.TeacherId == id);

                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                // Check if teacher has active schedules (without navigation property include)
                var hasActiveSchedules = await _context.ClassSchedules
                    .AnyAsync(cs => cs.TeacherId == id && cs.ScheduleDate >= DateTime.Today);

                // Check if teacher has active students (without navigation property include)
                var hasActiveStudents = await _context.Students
                    .AnyAsync(s => s.AssignedTeacherId == id && s.IsActive);

                if (hasActiveSchedules || hasActiveStudents)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete teacher with active schedules or assigned students. Please reassign or remove them first."
                    });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Soft delete - set IsActive to false
                teacher.IsActive = false;
                teacher.UpdatedBy = currentUserId;
                teacher.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Teacher {TeacherCode} soft deleted by user {UserId}", teacher.TeacherCode, currentUserId);

                return Json(new { success = true, message = "Teacher deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher {TeacherId}: {Message}", id, ex.Message);
                return Json(new { success = false, message = "An error occurred while deleting the teacher." });
            }
        }
    }
}