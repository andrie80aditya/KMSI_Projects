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
                    .AsNoTracking()
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId);

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Get allowed company IDs first
                var allowedCompanyIds = await _context.Companies
                    .AsNoTracking()
                    .Where(c => c.CompanyId == currentUser.CompanyId || c.ParentCompanyId == currentUser.CompanyId)
                    .Select(c => c.CompanyId)
                    .ToListAsync();

                // Get students dengan AsNoTracking untuk menghindari lazy loading navigation properties
                var studentsQuery = _context.Students.AsNoTracking();

                // Project ke anonymous type dulu untuk menghindari navigation property loading
                var studentData = await studentsQuery
                    .Where(s => allowedCompanyIds.Contains(s.CompanyId))
                    .Select(s => new
                    {
                        s.StudentId,
                        s.CompanyId,
                        s.SiteId,
                        s.StudentCode,
                        s.FirstName,
                        s.LastName,
                        s.FullName,
                        s.DateOfBirth,
                        s.Gender,
                        s.Phone,
                        s.Email,
                        s.ParentEmail,
                        s.CurrentGradeId,
                        s.AssignedTeacherId,
                        s.Status,
                        s.RegistrationDate,
                        s.Notes,
                        s.IsActive,
                        s.CreatedDate,
                        s.UpdatedDate
                    })
                    .OrderBy(s => s.StudentCode)
                    .ToListAsync();

                // Convert back ke Student objects
                var students = studentData.Select(s => new Student
                {
                    StudentId = s.StudentId,
                    CompanyId = s.CompanyId,
                    SiteId = s.SiteId,
                    StudentCode = s.StudentCode,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    Phone = s.Phone,
                    Email = s.Email,
                    ParentEmail = s.ParentEmail,
                    CurrentGradeId = s.CurrentGradeId,
                    AssignedTeacherId = s.AssignedTeacherId,
                    Status = s.Status,
                    RegistrationDate = s.RegistrationDate,
                    Notes = s.Notes,
                    IsActive = s.IsActive,
                    CreatedDate = s.CreatedDate,
                    UpdatedDate = s.UpdatedDate
                }).ToList();

                // Load related data menggunakan separate queries
                var companyIds = students.Select(s => s.CompanyId).Distinct().ToList();
                var siteIds = students.Select(s => s.SiteId).Distinct().ToList();
                var gradeIds = students.Where(s => s.CurrentGradeId.HasValue).Select(s => s.CurrentGradeId.Value).Distinct().ToList();
                var teacherIds = students.Where(s => s.AssignedTeacherId.HasValue).Select(s => s.AssignedTeacherId.Value).Distinct().ToList();

                var companies = await _context.Companies
                    .AsNoTracking()
                    .Where(c => companyIds.Contains(c.CompanyId))
                    .ToDictionaryAsync(c => c.CompanyId, c => c);

                var sites = await _context.Sites
                    .AsNoTracking()
                    .Where(s => siteIds.Contains(s.SiteId))
                    .ToDictionaryAsync(s => s.SiteId, s => s);

                var grades = await _context.Grades
                    .AsNoTracking()
                    .Where(g => gradeIds.Contains(g.GradeId))
                    .ToDictionaryAsync(g => g.GradeId, g => g);

                var teachers = await _context.Teachers
                    .AsNoTracking()
                    .Include(t => t.User)
                    .Where(t => teacherIds.Contains(t.TeacherId))
                    .ToDictionaryAsync(t => t.TeacherId, t => t);

                // Assign navigation properties secara manual
                foreach (var student in students)
                {
                    student.Company = companies.GetValueOrDefault(student.CompanyId);
                    student.Site = sites.GetValueOrDefault(student.SiteId);
                    if (student.CurrentGradeId.HasValue)
                        student.CurrentGrade = grades.GetValueOrDefault(student.CurrentGradeId.Value);
                    if (student.AssignedTeacherId.HasValue)
                        student.AssignedTeacher = teachers.GetValueOrDefault(student.AssignedTeacherId.Value);
                }

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
                    .FirstOrDefaultAsync(m => m.StudentId == id);

                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                // Load related data separately
                if (student.CurrentGradeId.HasValue)
                {
                    student.CurrentGrade = await _context.Grades
                        .FirstOrDefaultAsync(g => g.GradeId == student.CurrentGradeId.Value);
                }

                if (student.AssignedTeacherId.HasValue)
                {
                    student.AssignedTeacher = await _context.Teachers
                        .Include(t => t.User)
                        .FirstOrDefaultAsync(t => t.TeacherId == student.AssignedTeacherId.Value);
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

                return Json(new
                {
                    success = true,
                    companies = companies
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
                    .FirstOrDefaultAsync(s => s.StudentId == id);

                if (student == null)
                {
                    return Json(new { success = false, message = "Student not found." });
                }

                // Load related data separately
                if (student.CurrentGradeId.HasValue)
                {
                    student.CurrentGrade = await _context.Grades
                        .FirstOrDefaultAsync(g => g.GradeId == student.CurrentGradeId.Value);
                }

                if (student.AssignedTeacherId.HasValue)
                {
                    student.AssignedTeacher = await _context.Teachers
                        .Include(t => t.User)
                        .FirstOrDefaultAsync(t => t.TeacherId == student.AssignedTeacherId.Value);
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
                    companies = companies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit form data");
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

                return Json(new { success = true, sites = sites });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sites for company {CompanyId}", companyId);
                return Json(new { success = false, message = "Error loading sites." });
            }
        }

        // NEW: GET: Student/GetGradesByCompany/5
        public async Task<IActionResult> GetGradesByCompany(int companyId)
        {
            try
            {
                var grades = await _context.Grades
                    .Where(g => g.CompanyId == companyId && g.IsActive)
                    .Select(g => new { g.GradeId, g.GradeName })
                    .OrderBy(g => g.GradeName)
                    .ToListAsync();

                return Json(new { success = true, grades = grades });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading grades for company {CompanyId}", companyId);
                return Json(new { success = false, message = "Error loading grades." });
            }
        }

        // NEW: GET: Student/GetTeachersByCompanyAndSite
        public async Task<IActionResult> GetTeachersByCompanyAndSite(int companyId, int siteId)
        {
            try
            {
                var teachersQuery = _context.Teachers
                    .Include(t => t.User)
                    .Where(t => t.IsActive && t.CompanyId == companyId && t.SiteId == siteId);

                // Filter by site if provided
                //if (siteId.HasValue && siteId.Value > 0)
                //{
                //    teachersQuery = teachersQuery.Where(t => t.SiteId == siteId.Value);
                //}

                var teachers = await teachersQuery
                    .Select(t => new {
                        t.TeacherId,
                        TeacherName = t.User.FirstName + " " + t.User.LastName + " (" + t.TeacherCode + ")"
                    })
                    .OrderBy(t => t.TeacherName)
                    .ToListAsync();

                return Json(new { success = true, teachers = teachers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading teachers for company {CompanyId} and site {SiteId}", companyId, siteId);
                return Json(new { success = false, message = "Error loading teachers." });
            }
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check for duplicate student code
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
                        StudentCode = model.StudentCode.ToUpper(),
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        Phone = model.Phone,
                        Email = model.Email,
                        ParentEmail = model.ParentEmail,
                        CurrentGradeId = model.CurrentGradeId,
                        AssignedTeacherId = model.AssignedTeacherId,
                        Status = model.Status,
                        RegistrationDate = model.RegistrationDate,
                        Notes = model.Notes,
                        IsActive = true,
                        CreatedBy = currentUserId,
                        CreatedDate = DateTime.Now
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = "Student created successfully!",
                        data = new
                        {
                            studentId = student.StudentId,
                            studentCode = student.StudentCode,
                            fullName = student.FullName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
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
        public async Task<IActionResult> Edit(StudentViewModel model)
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

                    // Check for duplicate student code (excluding current student)
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
                    student.StudentCode = model.StudentCode.ToUpper();
                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.DateOfBirth = model.DateOfBirth;
                    student.Gender = model.Gender;
                    student.Phone = model.Phone;
                    student.Email = model.Email;
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

                    return Json(new
                    {
                        success = true,
                        message = "Student updated successfully!",
                        data = new
                        {
                            studentId = student.StudentId,
                            studentCode = student.StudentCode,
                            fullName = student.FullName
                        }
                    });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Validation failed.", errors = errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student");
                return Json(new { success = false, message = "An error occurred while updating the student." });
            }
        }

        // POST: Student/Delete
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

                // Soft delete - set IsActive to false
                student.IsActive = false;
                student.UpdatedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                student.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Student deleted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student");
                return Json(new { success = false, message = "An error occurred while deleting the student." });
            }
        }
    }
}