using KMSI_Projects.Data;
using KMSI_Projects.Models.ViewModels;
using KMSI_Projects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

                // Get teachers based on user's access level - PERSIS SEPERTI COMPANY DAN SITE
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

                var teacherDetails = new
                {
                    teacherId = teacher.TeacherId,
                    companyName = teacher.Company.CompanyName,
                    siteName = teacher.Site.SiteName,
                    teacherCode = teacher.TeacherCode,
                    userName = teacher.User.FullName,
                    userEmail = teacher.User.Email,
                    userPhone = teacher.User.Phone,
                    specialization = teacher.Specialization,
                    experienceYears = teacher.ExperienceYears,
                    hourlyRate = teacher.HourlyRate,
                    maxStudentsPerDay = teacher.MaxStudentsPerDay,
                    isAvailableForTrial = teacher.IsAvailableForTrial,
                    isActive = teacher.IsActive,
                    statusDisplay = teacher.IsActive ? "Active" : "Inactive",
                    hourlyRateDisplay = teacher.HourlyRate?.ToString("C") ?? "Not Set",
                    experienceLevelDisplay = teacher.ExperienceYears switch
                    {
                        null => "Not Specified",
                        0 => "Fresh Graduate",
                        >= 1 and <= 2 => "Junior Teacher",
                        >= 3 and <= 5 => "Experienced Teacher",
                        >= 6 and <= 10 => "Senior Teacher",
                        > 10 => "Expert Teacher",
                        _ => "Unknown"
                    },
                    trialAvailabilityDisplay = teacher.IsAvailableForTrial ? "Available for Trial" : "Not Available for Trial",
                    createdDate = teacher.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                    updatedDate = teacher.UpdatedDate?.ToString("dd/MM/yyyy HH:mm")
                };

                return Json(new { success = true, data = teacherDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teacher details for ID {TeacherId}", id);
                return Json(new { success = false, message = "Error loading teacher details." });
            }
        }

        // GET: Teacher/GetCreateForm
        public async Task<IActionResult> GetCreateForm()
        {
            try
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Get available users (not already assigned as teachers)
                var assignedUserIds = await _context.Teachers
                    .Where(t => t.IsActive)
                    .Select(t => t.UserId)
                    .ToListAsync();

                var availableUsers = await _context.Users
                    .Where(u => u.IsActive &&
                               (u.CompanyId == currentUser.CompanyId || u.Company.ParentCompanyId == currentUser.CompanyId) &&
                               !assignedUserIds.Contains(u.UserId))
                    .Select(u => new { u.UserId, u.FullName, u.Email })
                    .OrderBy(u => u.FullName)
                    .ToListAsync();

                // Get available companies
                var companies = await _context.Companies
                    .Where(c => c.IsActive &&
                               (c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId))
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
                _logger.LogError(ex, "Error getting create form data");
                return Json(new { success = false, message = "Error loading form data." });
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

                    // Check if teacher code already exists
                    var existingTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.TeacherCode == model.TeacherCode && t.IsActive);

                    if (existingTeacher != null)
                    {
                        return Json(new { success = false, message = "Teacher code already exists." });
                    }

                    // Check if user is already assigned as a teacher
                    var userAlreadyTeacher = await _context.Teachers
                        .FirstOrDefaultAsync(t => t.UserId == model.UserId && t.IsActive);

                    if (userAlreadyTeacher != null)
                    {
                        return Json(new { success = false, message = "This user is already assigned as a teacher." });
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

                    _logger.LogInformation("Teacher created successfully: {TeacherCode} by user {UserId}", teacher.TeacherCode, currentUserId);

                    return Json(new { success = true, message = "Teacher created successfully." });
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

        // GET: Teacher/GetEditForm/5
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
                    .FirstOrDefaultAsync(m => m.TeacherId == id);

                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Get available users (including current teacher's user)
                var assignedUserIds = await _context.Teachers
                    .Where(t => t.IsActive && t.TeacherId != id)
                    .Select(t => t.UserId)
                    .ToListAsync();

                var availableUsers = await _context.Users
                    .Where(u => u.IsActive &&
                               (u.CompanyId == currentUser.CompanyId || u.Company.ParentCompanyId == currentUser.CompanyId) &&
                               (!assignedUserIds.Contains(u.UserId) || u.UserId == teacher.UserId))
                    .Select(u => new { u.UserId, u.FullName, u.Email })
                    .OrderBy(u => u.FullName)
                    .ToListAsync();

                // Get available companies
                var companies = await _context.Companies
                    .Where(c => c.IsActive &&
                               (c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId))
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                // Get sites for selected company
                var sites = await _context.Sites
                    .Where(s => s.IsActive && s.CompanyId == teacher.CompanyId)
                    .Select(s => new { s.SiteId, s.SiteName })
                    .OrderBy(s => s.SiteName)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        teacherId = teacher.TeacherId,
                        userId = teacher.UserId,
                        companyId = teacher.CompanyId,
                        siteId = teacher.SiteId,
                        teacherCode = teacher.TeacherCode,
                        specialization = teacher.Specialization,
                        experienceYears = teacher.ExperienceYears,
                        hourlyRate = teacher.HourlyRate,
                        maxStudentsPerDay = teacher.MaxStudentsPerDay,
                        isAvailableForTrial = teacher.IsAvailableForTrial,
                        isActive = teacher.IsActive
                    },
                    users = availableUsers,
                    companies = companies,
                    sites = sites
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting edit form data for teacher {TeacherId}", id);
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
                        .FirstOrDefaultAsync(t => t.TeacherCode == model.TeacherCode && t.TeacherId != id && t.IsActive);

                    if (existingTeacher != null)
                    {
                        return Json(new { success = false, message = "Teacher code already exists." });
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
                _logger.LogError(ex, "Error updating teacher: {TeacherId}", id);
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
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return Json(new { success = false, message = "Teacher not found." });
                }

                // Check if teacher has active students
                var hasActiveStudents = await _context.Students
                    .AnyAsync(s => s.AssignedTeacherId == id && s.IsActive);

                // Check if teacher has active schedules
                var hasActiveSchedules = await _context.ClassSchedules
                    .AnyAsync(cs => cs.TeacherId == id && cs.ScheduleDate >= DateTime.Today);

                if (hasActiveStudents || hasActiveSchedules)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Cannot delete teacher with active students or schedules. Please reassign or deactivate them first."
                    });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                // Soft delete
                teacher.IsActive = false;
                teacher.UpdatedBy = currentUserId;
                teacher.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Teacher deleted successfully: {TeacherCode} by user {UserId}", teacher.TeacherCode, currentUserId);

                return Json(new { success = true, message = "Teacher deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting teacher: {TeacherId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the teacher." });
            }
        }

        // GET: Teacher/GetSitesByCompany/5
        public async Task<IActionResult> GetSitesByCompany(int companyId)
        {
            try
            {
                var sites = await _context.Sites
                    .Where(s => s.IsActive && s.CompanyId == companyId)
                    .Select(s => new { s.SiteId, s.SiteName })
                    .OrderBy(s => s.SiteName)
                    .ToListAsync();

                return Json(new { success = true, sites = sites });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sites for company: {CompanyId}", companyId);
                return Json(new { success = false, message = "An error occurred while loading sites." });
            }
        }
    }
}
