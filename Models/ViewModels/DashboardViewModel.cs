namespace KMSI_Projects.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int ActiveTeachers { get; set; }
        public int TotalSites { get; set; }
        public int PendingExaminations { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();
    }
}
