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
        private readonly IWebHostEnvironment _environment;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger, IWebHostEnvironment environment)
            : base(context, logger)
        {
            _environment = environment;
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
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorTitle = "An Error Occurred",
                ErrorMessage = TempData["ErrorMessage"]?.ToString(),
                IsDevelopment = _environment.IsDevelopment(),
                SuggestedAction = "Please try again later or contact support if the problem persists.",
                SupportEmail = "support@kawaimusic.id"
            };

            // In development, you might want to add more details
            if (_environment.IsDevelopment())
            {
                errorViewModel.ExceptionDetails = TempData["ExceptionDetails"]?.ToString();
                errorViewModel.StackTrace = TempData["StackTrace"]?.ToString();
            }

            return View(errorViewModel);
        }

        // Helper methods for dashboard data
        private async Task<decimal> CalculateMonthlyRevenueAsync()
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                var revenue = await _context.StudentBillings
                    .Where(sb => sb.CompanyId == CurrentCompanyId
                                && sb.CreatedDate.Month == currentMonth
                                && sb.CreatedDate.Year == currentYear
                                && sb.Status == "Paid")
                    .SumAsync(sb => sb.TotalAmount);

                return revenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating monthly revenue");
                return 0;
            }
        }

        private async Task<List<RecentActivityViewModel>> GetRecentActivitiesAsync()
        {
            try
            {
                var activities = new List<RecentActivityViewModel>();

                // Get recent student registrations
                var recentStudents = await _context.Students
                    .Where(s => s.CompanyId == CurrentCompanyId)
                    .OrderByDescending(s => s.CreatedDate)
                    .Take(5)
                    .Select(s => new RecentActivityViewModel
                    {
                        Type = "New Student",
                        Description = $"Student {s.FirstName} {s.LastName} registered",
                        Date = s.CreatedDate,
                        Icon = "fas fa-user-plus",
                        Color = "success"
                    })
                    .ToListAsync();

                activities.AddRange(recentStudents);

                // Get recent examinations
                var recentExams = await _context.Examinations
                    .Where(e => e.CompanyId == CurrentCompanyId)
                    .OrderByDescending(e => e.CreatedDate)
                    .Take(3)
                    .Select(e => new RecentActivityViewModel
                    {
                        Type = "Examination",
                        Description = $"Exam {e.ExamName} scheduled",
                        Date = e.CreatedDate,
                        Icon = "fas fa-clipboard-check",
                        Color = "info"
                    })
                    .ToListAsync();

                activities.AddRange(recentExams);

                return activities.OrderByDescending(a => a.Date).Take(10).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent activities");
                return new List<RecentActivityViewModel>();
            }
        }
    }
}
