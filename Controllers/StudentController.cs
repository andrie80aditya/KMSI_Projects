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
    /// Controller For Master Student
    /// Manages student information within companies and sites
    /// </summary>
    [Authorize]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudentController> _logger;

        public StudentController(ApplicationDbContext context, ILogger<StudentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Student
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

                // Get students based on user's access level - mengikuti pola Teacher
                var students = await _context.Students
                    .Include(s => s.Company)
                    .Include(s => s.Site)
                    .Include(s => s.CurrentGrade)
                    .Include(s => s.AssignedTeacher)
                    .ThenInclude(t => t.User)
                    .Where(s => s.CompanyId == currentUser.CompanyId || s.Company.ParentCompanyId == currentUser.CompanyId)
                    .OrderBy(s => s.StudentCode)
                    .ToListAsync();

                ViewBag.CurrentCompanyId = currentUser.CompanyId;
                return View(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading students");
                TempData["ErrorMessage"] = "An error occurred while loading students.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Student ID is required." });
            }

            try
            {
                var student = await _context.Students
                    .Include(s => s.Company)
                    .Include(s => s.Site)
                    .Include(s => s.CurrentGrade)
                    .Include(s => s.AssignedTeacher)
                    .ThenInclude(t => t.User)
                    .FirstOrDefaultAsync(m => m.StudentId == id);

                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                var viewModel = new StudentDetailsViewModel
                {
                    StudentId = student.StudentId,
                    CompanyName = student.Company.CompanyName,
                    SiteName = student.Site.SiteName,
                    StudentCode = student.StudentCode,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    FullName = student.FullName,
                    DateOfBirth = student.DateOfBirth,
                    Gender = student.Gender,
                    Phone = student.Phone,
                    Email = student.Email,
                    Address = student.Address,
                    ParentName = student.ParentName,
                    ParentPhone = student.ParentPhone,
                    ParentEmail = student.ParentEmail,
                    CurrentGradeName = student.CurrentGrade?.GradeName,
                    AssignedTeacherName = student.AssignedTeacher?.User?.FullName,
                    Status = student.Status,
                    RegistrationDate = student.RegistrationDate,
                    Notes = student.Notes,
                    IsActive = student.IsActive,
                    CreatedDate = student.CreatedDate,
                    UpdatedDate = student.UpdatedDate
                };

                return Json(new { success = true, data = viewModel });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student details");
                return Json(new { success = false, message = "An error occurred while retrieving student details." });
            }
        }

        // GET: Student/GetCreateForm
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

                // Get companies for dropdown
                var companies = await _context.Companies
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                // Get active grades
                var grades = await _context.Grades
                    .Where(g => g.IsActive)
                    .Select(g => new { g.GradeId, g.GradeName })
                    .OrderBy(g => g.GradeName)
                    .ToListAsync();

                // Get active teachers
                var teachers = await _context.Teachers
                    .Include(t => t.User)
                    .Where(t => t.IsActive && t.CompanyId == currentUser.CompanyId)
                    .Select(t => new { t.TeacherId, TeacherName = t.User.FullName + " (" + t.TeacherCode + ")" })
                    .OrderBy(t => t.TeacherName)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    companies = companies,
                    grades = grades,
                    teachers = teachers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form data");
                return Json(new { success = false, message = "Error loading form data." });
            }
        }

        // GET: Student/GetEditForm/5
        public async Task<IActionResult> GetEditForm(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Student ID is required." });
            }

            try
            {
                var student = await _context.Students
                    .Include(s => s.Company)
                    .Include(s => s.Site)
                    .Include(s => s.CurrentGrade)
                    .Include(s => s.AssignedTeacher)
                    .FirstOrDefaultAsync(s => s.StudentId == id);

                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                // Get companies for dropdown
                var companies = await _context.Companies
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .Select(c => new { c.CompanyId, c.CompanyName })
                    .OrderBy(c => c.CompanyName)
                    .ToListAsync();

                // Get sites for selected company
                var sites = await _context.Sites
                    .Where(s => s.CompanyId == student.CompanyId && s.IsActive)
                    .Select(s => new { s.SiteId, s.SiteName })
                    .OrderBy(s => s.SiteName)
                    .ToListAsync();

                // Get active grades
                var grades = await _context.Grades
                    .Where(g => g.IsActive)
                    .Select(g => new { g.GradeId, g.GradeName })
                    .OrderBy(g => g.GradeName)
                    .ToListAsync();

                // Get active teachers
                var teachers = await _context.Teachers
                    .Include(t => t.User)
                    .Where(t => t.IsActive && t.CompanyId == student.CompanyId)
                    .Select(t => new { t.TeacherId, TeacherName = t.User.FullName + " (" + t.TeacherCode + ")" })
                    .OrderBy(t => t.TeacherName)
                    .ToListAsync();

                var viewModel = new StudentViewModel
                {
                    StudentId = student.StudentId,
                    CompanyId = student.CompanyId,
                    SiteId = student.SiteId,
                    StudentCode = student.StudentCode,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    DateOfBirth = student.DateOfBirth,
                    Gender = student.Gender,
                    Phone = student.Phone,
                    Email = student.Email,
                    Address = student.Address,
                    ParentName = student.ParentName,
                    ParentPhone = student.ParentPhone,
                    ParentEmail = student.ParentEmail,
                    CurrentGradeId = student.CurrentGradeId,
                    AssignedTeacherId = student.AssignedTeacherId,
                    Status = student.Status,
                    RegistrationDate = student.RegistrationDate,
                    Notes = student.Notes,
                    IsActive = student.IsActive
                };

                return Json(new
                {
                    success = true,
                    student = viewModel,
                    companies = companies,
                    sites = sites,
                    grades = grades,
                    teachers = teachers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form for student {StudentId}", id);
                return Json(new { success = false, message = "Error loading student data." });
            }
        }

        // GET: Student/GetSitesByCompany/5
        public async Task<IActionResult> GetSitesByCompany(int companyId)
        {
            try
            {
                var sites = await _context.Sites
                    .Where(s => s.CompanyId == companyId && s.IsActive)
                    .Select(s => new { s.SiteId, s.SiteName })
                    .OrderBy(s => s.SiteName)
                    .ToListAsync();

                return Json(new { success = true, data = sites });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sites for company {CompanyId}", companyId);
                return Json(new { success = false, message = "Error loading sites." });
            }
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] StudentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if student code already exists
                    var existingStudent = await _context.Students
                        .FirstOrDefaultAsync(s => s.StudentCode == model.StudentCode);

                    if (existingStudent != null)
                    {
                        return Json(new { success = false, message = "Student code already exists." });
                    }

                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    var student = new Student
                    {
                        CompanyId = model.CompanyId,
                        SiteId = model.SiteId,
                        StudentCode = model.StudentCode,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        Phone = model.Phone,
                        Email = model.Email,
                        Address = model.Address,
                        ParentName = model.ParentName,
                        ParentPhone = model.ParentPhone,
                        ParentEmail = model.ParentEmail,
                        CurrentGradeId = model.CurrentGradeId,
                        AssignedTeacherId = model.AssignedTeacherId,
                        Status = model.Status,
                        RegistrationDate = model.RegistrationDate,
                        Notes = model.Notes,
                        IsActive = model.IsActive,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Student {StudentCode} created successfully", model.StudentCode);
                    return Json(new { success = true, message = "Student created successfully!" });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Please correct the validation errors.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                return Json(new { success = false, message = "An error occurred while creating the student." });
            }
        }

        // POST: Student/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] StudentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var student = await _context.Students.FindAsync(model.StudentId);
                    if (student == null)
                    {
                        return Json(new { success = false, message = "Student not found." });
                    }

                    // Check if student code already exists (excluding current student)
                    var existingStudent = await _context.Students
                        .FirstOrDefaultAsync(s => s.StudentCode == model.StudentCode && s.StudentId != model.StudentId);

                    if (existingStudent != null)
                    {
                        return Json(new { success = false, message = "Student code already exists." });
                    }

                    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                    // Update student properties
                    student.CompanyId = model.CompanyId;
                    student.SiteId = model.SiteId;
                    student.StudentCode = model.StudentCode;
                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.DateOfBirth = model.DateOfBirth;
                    student.Gender = model.Gender;
                    student.Phone = model.Phone;
                    student.Email = model.Email;
                    student.Address = model.Address;
                    student.ParentName = model.ParentName;
                    student.ParentPhone = model.ParentPhone;
                    student.ParentEmail = model.ParentEmail;
                    student.CurrentGradeId = model.CurrentGradeId;
                    student.AssignedTeacherId = model.AssignedTeacherId;
                    student.Status = model.Status;
                    student.RegistrationDate = model.RegistrationDate;
                    student.Notes = model.Notes;
                    student.IsActive = model.IsActive;
                    student.UpdatedBy = currentUserId;
                    student.UpdatedDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Student {StudentCode} updated successfully", model.StudentCode);
                    return Json(new { success = true, message = "Student updated successfully!" });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Please correct the validation errors.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student {StudentId}", model.StudentId);
                return Json(new { success = false, message = "An error occurred while updating the student." });
            }
        }

        // POST: Student/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                // Check if student has related records
                var hasSchedules = await _context.ClassSchedules.AnyAsync(cs => cs.StudentId == id);
                var hasAttendances = await _context.Attendances.AnyAsync(a => a.StudentId == id);
                var hasRegistrations = await _context.Registrations.AnyAsync(r => r.StudentId == id);

                if (hasSchedules || hasAttendances || hasRegistrations)
                {
                    return Json(new { success = false, message = "Cannot delete student with existing schedules, attendances, or registrations." });
                }

                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                student.IsActive = false;
                student.UpdatedBy = currentUserId;
                student.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Student {StudentCode} deactivated successfully", student.StudentCode);
                return Json(new { success = true, message = "Student deactivated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student {StudentId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the student." });
            }
        }
    }
}