using KMSI_Projects.Data;
using KMSI_Projects.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KMSI_Projects.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
            : base(context, logger)
        {
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Dashboard statistics
                var dashboardData = new DashboardViewModel
                {
                    TotalStudents = await _context.Students.CountAsync(s => s.CompanyId == CurrentCompanyId),
                    ActiveStudents = await _context.Students.CountAsync(s => s.CompanyId == CurrentCompanyId && s.IsActive),
                    TotalTeachers = await _context.Teachers.CountAsync(t => t.CompanyId == CurrentCompanyId),
                    ActiveTeachers = await _context.Teachers.CountAsync(t => t.CompanyId == CurrentCompanyId && t.IsActive),
                    TotalSites = await _context.Sites.CountAsync(s => s.CompanyId == CurrentCompanyId),
                    PendingExaminations = await _context.Examinations.CountAsync(e => e.CompanyId == CurrentCompanyId && e.Status == "Scheduled"),
                    MonthlyRevenue = await CalculateMonthlyRevenueAsync(),
                    RecentActivities = await GetRecentActivitiesAsync()
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "loading dashboard");
            }
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Helper methods for dashboard data
        private async Task<decimal> CalculateMonthlyRevenueAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            return await _context.StudentBillings
                .Where(sb => sb.CompanyId == CurrentCompanyId
                    && sb.BillingDate.Month == currentMonth
                    && sb.BillingDate.Year == currentYear
                    && sb.Status == "Paid")
                .SumAsync(sb => sb.TotalAmount);
        }

        private async Task<List<RecentActivityViewModel>> GetRecentActivitiesAsync()
        {
            var activities = new List<RecentActivityViewModel>();

            // Recent student enrollments
            var recentEnrollments = await _context.Registrations
                .Include(se => se.Student)
                .Where(se => se.CompanyId == CurrentCompanyId && se.RegistrationDate >= DateTime.Now.AddDays(-7))
                .OrderByDescending(se => se.RegistrationDate)
                .Take(5)
                .ToListAsync();

            activities.AddRange(recentEnrollments.Select(e => new RecentActivityViewModel
            {
                Type = "Enrollment",
                Description = $"New student enrollment: {e.Student.FullName}",
                Date = e.RegistrationDate,
                Icon = "fas fa-user-plus",
                Color = "success"
            }));

            // Recent examinations
            var recentExams = await _context.Examinations
                .Where(ex => ex.CompanyId == CurrentCompanyId && ex.ExamDate >= DateTime.Now.AddDays(-7))
                .OrderByDescending(ex => ex.ExamDate)
                .Take(5)
                .ToListAsync();

            activities.AddRange(recentExams.Select(ex => new RecentActivityViewModel
            {
                Type = "Examination",
                Description = $"Examination scheduled: {ex.ExamName}",
                Date = ex.ExamDate,
                Icon = "fas fa-clipboard-check",
                Color = "info"
            }));

            return activities.OrderByDescending(a => a.Date).Take(10).ToList();
        }
    }
}
