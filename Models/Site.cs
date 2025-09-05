using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a site/branch location within a company
    /// Each site is a physical location where courses are conducted
    /// </summary>
    [Table("Sites")]
    public class Site
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site code is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Site code must be between 2 and 10 characters")]
        [Display(Name = "Site Code")]
        [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "Site code must contain only uppercase letters and numbers")]
        public string SiteCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Site name is required")]
        [StringLength(100, ErrorMessage = "Site name cannot exceed 100 characters")]
        [Display(Name = "Site Name")]
        public string SiteName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "Province name cannot exceed 50 characters")]
        [Display(Name = "Province")]
        public string? Province { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters")]
        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        [Display(Name = "Updated Date")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Updated By")]
        [ForeignKey("UpdatedByUser")]
        public int? UpdatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Company that owns this site
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Users assigned to this site
        /// </summary>
        [Display(Name = "Users")]
        public virtual ICollection<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Teachers working at this site
        /// </summary>
        [Display(Name = "Teachers")]
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        /// <summary>
        /// Students registered at this site
        /// </summary>
        [Display(Name = "Students")]
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();

        /// <summary>
        /// Class schedules for this site
        /// </summary>
        [Display(Name = "Class Schedules")]
        public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

        /// <summary>
        /// Examinations conducted at this site
        /// </summary>
        [Display(Name = "Examinations")]
        public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

        /// <summary>
        /// Student billings for this site
        /// </summary>
        [Display(Name = "Student Billings")]
        public virtual ICollection<StudentBilling> StudentBillings { get; set; } = new List<StudentBilling>();

        /// <summary>
        /// Teacher payrolls for this site
        /// </summary>
        [Display(Name = "Teacher Payrolls")]
        public virtual ICollection<TeacherPayroll> TeacherPayrolls { get; set; } = new List<TeacherPayroll>();

        /// <summary>
        /// Inventory records for this site
        /// </summary>
        [Display(Name = "Inventory")]
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        /// <summary>
        /// Stock movements for this site
        /// </summary>
        [Display(Name = "Stock Movements")]
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

        /// <summary>
        /// Stock movements originating from this site (transfers out)
        /// </summary>
        [Display(Name = "Stock Movements From")]
        [InverseProperty("FromSite")]
        public virtual ICollection<StockMovement> StockMovementsFrom { get; set; } = new List<StockMovement>();

        /// <summary>
        /// Stock movements coming to this site (transfers in)
        /// </summary>
        [Display(Name = "Stock Movements To")]
        [InverseProperty("ToSite")]
        public virtual ICollection<StockMovement> StockMovementsTo { get; set; } = new List<StockMovement>();

        /// <summary>
        /// Book requisitions made by this site
        /// </summary>
        [Display(Name = "Book Requisitions")]
        public virtual ICollection<BookRequisition> BookRequisitions { get; set; } = new List<BookRequisition>();

        /// <summary>
        /// Book prices specific to this site
        /// </summary>
        [Display(Name = "Book Prices")]
        public virtual ICollection<BookPrice> BookPrices { get; set; } = new List<BookPrice>();

        /// <summary>
        /// Registrations processed at this site
        /// </summary>
        [Display(Name = "Registrations")]
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this site record
        /// </summary>
        [InverseProperty("CreatedSites")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this site record
        /// </summary>
        [InverseProperty("UpdatedSites")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining code and name
        /// </summary>
        [NotMapped]
        [Display(Name = "Site")]
        public string DisplayName => $"{SiteCode} - {SiteName}";

        /// <summary>
        /// Full display name with company code
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Site Name")]
        public string FullDisplayName => $"{Company?.CompanyCode} {SiteCode} - {SiteName}";

        /// <summary>
        /// Full address combining all address fields
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Address")]
        public string FullAddress
        {
            get
            {
                var addressParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Address)) addressParts.Add(Address);
                if (!string.IsNullOrWhiteSpace(City)) addressParts.Add(City);
                if (!string.IsNullOrWhiteSpace(Province)) addressParts.Add(Province);
                return string.Join(", ", addressParts);
            }
        }

        /// <summary>
        /// Status display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Location display (City, Province)
        /// </summary>
        [NotMapped]
        [Display(Name = "Location")]
        public string LocationDisplay
        {
            get
            {
                var locationParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(City)) locationParts.Add(City);
                if (!string.IsNullOrWhiteSpace(Province)) locationParts.Add(Province);
                return string.Join(", ", locationParts);
            }
        }

        /// <summary>
        /// Total active students at this site
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Students")]
        public int ActiveStudentsCount => Students?.Count(s => s.IsActive && s.Status == "Active") ?? 0;

        /// <summary>
        /// Total active teachers at this site
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Teachers")]
        public int ActiveTeachersCount => Teachers?.Count(t => t.IsActive) ?? 0;

        /// <summary>
        /// Total trial students at this site
        /// </summary>
        [NotMapped]
        [Display(Name = "Trial Students")]
        public int TrialStudentsCount => Students?.Count(s => s.IsActive && s.Status == "Trial") ?? 0;

        /// <summary>
        /// Total pending registrations at this site
        /// </summary>
        [NotMapped]
        [Display(Name = "Pending Registrations")]
        public int PendingRegistrationsCount => Registrations?.Count(r => r.Status == "Pending") ?? 0;

        /// <summary>
        /// Total outstanding billings for this site
        /// </summary>
        [NotMapped]
        [Display(Name = "Outstanding Billings")]
        public decimal OutstandingBillingsAmount => StudentBillings?
            .Where(b => b.Status == "Outstanding" || b.Status == "Overdue")
            .Sum(b => b.TotalAmount) ?? 0;

        /// <summary>
        /// Current month's revenue for this site
        /// </summary>
        [NotMapped]
        [Display(Name = "Current Month Revenue")]
        public decimal CurrentMonthRevenue
        {
            get
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                return StudentBillings?
                    .Where(b => b.Status == "Paid" &&
                               b.PaymentDate?.Month == currentMonth &&
                               b.PaymentDate?.Year == currentYear)
                    .Sum(b => b.PaymentAmount ?? 0) ?? 0;
            }
        }

        /// <summary>
        /// Check if site has low stock items
        /// </summary>
        [NotMapped]
        [Display(Name = "Has Low Stock")]
        public bool HasLowStockItems => Inventories?.Any(i => i.CurrentStock <= i.MinimumStock) ?? false;

        /// <summary>
        /// Count of low stock items
        /// </summary>
        [NotMapped]
        [Display(Name = "Low Stock Items Count")]
        public int LowStockItemsCount => Inventories?.Count(i => i.CurrentStock <= i.MinimumStock) ?? 0;

        // Business Logic Methods

        /// <summary>
        /// Check if site code is unique within the same company
        /// </summary>
        /// <param name="otherSites">Other sites to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsSiteCodeUnique(IEnumerable<Site> otherSites)
        {
            return !otherSites.Any(s =>
                s.SiteId != SiteId &&
                s.SiteCode == SiteCode &&
                s.CompanyId == CompanyId);
        }

        /// <summary>
        /// Get students by status
        /// </summary>
        /// <param name="status">Student status to filter by</param>
        /// <returns>Students with the specified status</returns>
        public IEnumerable<Student> GetStudentsByStatus(string status)
        {
            return Students?.Where(s => s.IsActive && s.Status == status) ?? Enumerable.Empty<Student>();
        }

        /// <summary>
        /// Get available teachers for a specific date and time
        /// </summary>
        /// <param name="date">Date to check availability</param>
        /// <param name="startTime">Start time to check</param>
        /// <param name="endTime">End time to check</param>
        /// <returns>Available teachers</returns>
        public IEnumerable<Teacher> GetAvailableTeachers(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // Sunday = 7 in our system

            return Teachers?.Where(t => t.IsActive &&
                t.TeacherSchedules.Any(ts => ts.IsActive &&
                    ts.DayOfWeek == dayOfWeek &&
                    ts.StartTime <= startTime &&
                    ts.EndTime >= endTime) &&
                !ClassSchedules.Any(cs => cs.TeacherId == t.TeacherId &&
                    cs.ScheduleDate.Date == date.Date &&
                    cs.StartTime < endTime &&
                    cs.EndTime > startTime &&
                    cs.Status != "Cancelled"))
                ?? Enumerable.Empty<Teacher>();
        }

        /// <summary>
        /// Get monthly revenue for a specific month and year
        /// </summary>
        /// <param name="month">Month (1-12)</param>
        /// <param name="year">Year</param>
        /// <returns>Total revenue for the month</returns>
        public decimal GetMonthlyRevenue(int month, int year)
        {
            return StudentBillings?
                .Where(b => b.Status == "Paid" &&
                           b.PaymentDate?.Month == month &&
                           b.PaymentDate?.Year == year)
                .Sum(b => b.PaymentAmount ?? 0) ?? 0;
        }

        /// <summary>
        /// Get inventory items that need reordering
        /// </summary>
        /// <returns>Inventory items below reorder level</returns>
        public IEnumerable<Inventory> GetItemsNeedingReorder()
        {
            return Inventories?.Where(i => i.CurrentStock <= i.ReorderLevel) ?? Enumerable.Empty<Inventory>();
        }

        /// <summary>
        /// Calculate teacher utilization rate for this site
        /// </summary>
        /// <param name="startDate">Start date for calculation</param>
        /// <param name="endDate">End date for calculation</param>
        /// <returns>Utilization percentage (0-100)</returns>
        public double GetTeacherUtilizationRate(DateTime startDate, DateTime endDate)
        {
            if (!Teachers.Any(t => t.IsActive)) return 0;

            var totalPossibleHours = Teachers.Where(t => t.IsActive)
                .SelectMany(t => t.TeacherSchedules.Where(ts => ts.IsActive))
                .Sum(ts => (ts.EndTime - ts.StartTime).TotalHours) *
                (endDate - startDate).Days / 7; // Weekly schedule

            var totalActualHours = ClassSchedules
                .Where(cs => cs.ScheduleDate >= startDate && cs.ScheduleDate <= endDate && cs.Status == "Completed")
                .Sum(cs => cs.Duration / 60.0);

            return totalPossibleHours > 0 ? (totalActualHours / totalPossibleHours) * 100 : 0;
        }

        /// <summary>
        /// Get site capacity utilization
        /// </summary>
        /// <returns>Percentage of capacity being used</returns>
        public double GetCapacityUtilization()
        {
            var maxCapacity = Teachers.Where(t => t.IsActive).Sum(t => t.MaxStudentsPerDay);
            var currentStudents = ActiveStudentsCount;
            return maxCapacity > 0 ? (double)currentStudents / maxCapacity * 100 : 0;
        }

        /// <summary>
        /// Generate site-specific book requisition number
        /// </summary>
        /// <param name="existingRequisitions">Existing requisition numbers</param>
        /// <returns>Next requisition number</returns>
        public string GenerateRequisitionNumber(IEnumerable<string> existingRequisitions)
        {
            var now = DateTime.Now;
            var yearMonth = $"{now.Year % 100:00}{now.Month:00}";
            var prefix = $"REQ-{SiteCode}-{yearMonth}-";

            var existingNumbers = existingRequisitions
                .Where(req => req.StartsWith(prefix))
                .Select(req =>
                {
                    var parts = req.Split('-');
                    return parts.Length == 4 && int.TryParse(parts[3], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingNumbers.Max() + 1;
            return $"{prefix}{nextNumber:000}";
        }

        /// <summary>
        /// Check if site can accommodate new student registration
        /// </summary>
        /// <returns>True if site can accommodate more students</returns>
        public bool CanAccommodateNewStudent()
        {
            var maxCapacity = Teachers.Where(t => t.IsActive).Sum(t => t.MaxStudentsPerDay);
            return ActiveStudentsCount < maxCapacity;
        }

        /// <summary>
        /// Get site performance metrics for dashboard
        /// </summary>
        /// <returns>Dictionary of performance metrics</returns>
        public Dictionary<string, object> GetPerformanceMetrics()
        {
            return new Dictionary<string, object>
            {
                {"ActiveStudents", ActiveStudentsCount},
                {"ActiveTeachers", ActiveTeachersCount},
                {"TrialStudents", TrialStudentsCount},
                {"PendingRegistrations", PendingRegistrationsCount},
                {"OutstandingBillings", OutstandingBillingsAmount},
                {"CurrentMonthRevenue", CurrentMonthRevenue},
                {"CapacityUtilization", GetCapacityUtilization()},
                {"LowStockItems", LowStockItemsCount},
                {"HasLowStock", HasLowStockItems}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the site</returns>
        public override string ToString() => DisplayName;
    }
}
