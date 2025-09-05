using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a grade/level in the music course curriculum
    /// Each grade defines a specific learning stage with associated books and duration
    /// </summary>
    [Table("Grades")]
    public class Grade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Grade code is required")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Grade code must be between 1 and 10 characters")]
        [Display(Name = "Grade Code")]
        [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "Grade code must contain only uppercase letters and numbers")]
        public string GradeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade name is required")]
        [StringLength(50, ErrorMessage = "Grade name cannot exceed 50 characters")]
        [Display(Name = "Grade Name")]
        public string GradeName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Range(1, 104, ErrorMessage = "Duration must be between 1 and 104 weeks")]
        [Display(Name = "Duration (Weeks)")]
        public int? Duration { get; set; }

        [Range(1, 100, ErrorMessage = "Sort order must be between 1 and 100")]
        [Display(Name = "Sort Order")]
        public int? SortOrder { get; set; }

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
        /// Company that owns this grade
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Books associated with this grade
        /// </summary>
        [Display(Name = "Grade Books")]
        public virtual ICollection<GradeBook> GradeBooks { get; set; } = new List<GradeBook>();

        /// <summary>
        /// Students currently in this grade
        /// </summary>
        [Display(Name = "Current Students")]
        public virtual ICollection<Student> CurrentStudents { get; set; } = new List<Student>();

        /// <summary>
        /// Student grade histories for this grade
        /// </summary>
        [Display(Name = "Student Grade Histories")]
        public virtual ICollection<StudentGradeHistory> StudentGradeHistories { get; set; } = new List<StudentGradeHistory>();

        /// <summary>
        /// Registrations requesting this grade
        /// </summary>
        [Display(Name = "Registrations")]
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        /// <summary>
        /// Class schedules for this grade
        /// </summary>
        [Display(Name = "Class Schedules")]
        public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

        /// <summary>
        /// Examinations for this grade
        /// </summary>
        [Display(Name = "Examinations")]
        public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

        /// <summary>
        /// Student billings for this grade
        /// </summary>
        [Display(Name = "Student Billings")]
        public virtual ICollection<StudentBilling> StudentBillings { get; set; } = new List<StudentBilling>();

        /// <summary>
        /// Certificates issued for this grade
        /// </summary>
        [Display(Name = "Certificates")]
        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this grade record
        /// </summary>
        [InverseProperty("CreatedGrades")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this grade record
        /// </summary>
        [InverseProperty("UpdatedGrades")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining code and name
        /// </summary>
        [NotMapped]
        [Display(Name = "Grade")]
        public string DisplayName => $"{GradeCode} - {GradeName}";

        /// <summary>
        /// Full display name with duration
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Grade Name")]
        public string FullDisplayName => Duration.HasValue
            ? $"{GradeCode} - {GradeName} ({Duration} weeks)"
            : $"{GradeCode} - {GradeName}";

        /// <summary>
        /// Status display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Duration display in human-readable format
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration")]
        public string DurationDisplay
        {
            get
            {
                if (!Duration.HasValue) return "Not specified";

                var weeks = Duration.Value;
                if (weeks < 4) return $"{weeks} week{(weeks != 1 ? "s" : "")}";

                var months = weeks / 4;
                var remainingWeeks = weeks % 4;

                var result = $"{months} month{(months != 1 ? "s" : "")}";
                if (remainingWeeks > 0)
                    result += $" {remainingWeeks} week{(remainingWeeks != 1 ? "s" : "")}";

                return result;
            }
        }

        /// <summary>
        /// Count of active students currently in this grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Students")]
        public int ActiveStudentsCount => CurrentStudents?.Count(s => s.IsActive && s.Status == "Active") ?? 0;

        /// <summary>
        /// Count of trial students in this grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Trial Students")]
        public int TrialStudentsCount => CurrentStudents?.Count(s => s.IsActive && s.Status == "Trial") ?? 0;

        /// <summary>
        /// Count of students who completed this grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Completed Students")]
        public int CompletedStudentsCount => StudentGradeHistories?.Count(sgh => sgh.Status == "Completed") ?? 0;

        /// <summary>
        /// Count of required books for this grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Required Books")]
        public int RequiredBooksCount => GradeBooks?.Count(gb => gb.IsRequired) ?? 0;

        /// <summary>
        /// Count of optional books for this grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Optional Books")]
        public int OptionalBooksCount => GradeBooks?.Count(gb => !gb.IsRequired) ?? 0;

        /// <summary>
        /// Total books associated with this grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Books")]
        public int TotalBooksCount => GradeBooks?.Count ?? 0;

        /// <summary>
        /// Average completion rate for this grade (percentage)
        /// </summary>
        [NotMapped]
        [Display(Name = "Avg Completion Rate")]
        public double AverageCompletionRate
        {
            get
            {
                var histories = StudentGradeHistories?.Where(sgh => sgh.Status != "Active").ToList();
                if (histories == null || !histories.Any()) return 0;

                // Solusi terbaik: Menggunakan GetValueOrDefault()
                return histories.Average(sgh => (double)sgh.CompletionPercentage);
            }
        }

        /// <summary>
        /// Count of scheduled examinations for this grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Scheduled Exams")]
        public int ScheduledExaminationsCount => Examinations?.Count(e => e.Status == "Scheduled") ?? 0;

        /// <summary>
        /// Monthly revenue from this grade
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
        /// Check if this is a beginner grade (lowest sort order)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Beginner Grade")]
        public bool IsBeginnerGrade => SortOrder == 1 || (SortOrder.HasValue && Company?.Grades.Where(g => g.IsActive).Min(g => g.SortOrder) == SortOrder);

        /// <summary>
        /// Check if this is an advanced grade (highest sort order)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Advanced Grade")]
        public bool IsAdvancedGrade => SortOrder.HasValue && Company?.Grades.Where(g => g.IsActive).Max(g => g.SortOrder) == SortOrder;

        // Business Logic Methods

        /// <summary>
        /// Check if grade code is unique within the same company
        /// </summary>
        /// <param name="otherGrades">Other grades to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsGradeCodeUnique(IEnumerable<Grade> otherGrades)
        {
            return !otherGrades.Any(g =>
                g.GradeId != GradeId &&
                g.GradeCode == GradeCode &&
                g.CompanyId == CompanyId);
        }

        /// <summary>
        /// Get students by status in this grade
        /// </summary>
        /// <param name="status">Student status to filter by</param>
        /// <returns>Students with the specified status</returns>
        public IEnumerable<Student> GetStudentsByStatus(string status)
        {
            return CurrentStudents?.Where(s => s.IsActive && s.Status == status) ?? Enumerable.Empty<Student>();
        }

        /// <summary>
        /// Get required books for this grade
        /// </summary>
        /// <returns>Collection of required books</returns>
        public IEnumerable<Book> GetRequiredBooks()
        {
            return GradeBooks?.Where(gb => gb.IsRequired)
                             .OrderBy(gb => gb.SortOrder)
                             .Select(gb => gb.Book)
                ?? Enumerable.Empty<Book>();
        }

        /// <summary>
        /// Get optional books for this grade
        /// </summary>
        /// <returns>Collection of optional books</returns>
        public IEnumerable<Book> GetOptionalBooks()
        {
            return GradeBooks?.Where(gb => !gb.IsRequired)
                             .OrderBy(gb => gb.SortOrder)
                             .Select(gb => gb.Book)
                ?? Enumerable.Empty<Book>();
        }

        /// <summary>
        /// Get all books for this grade ordered by sort order
        /// </summary>
        /// <returns>All books associated with this grade</returns>
        public IEnumerable<Book> GetAllBooks()
        {
            return GradeBooks?.OrderBy(gb => gb.SortOrder)
                             .Select(gb => gb.Book)
                ?? Enumerable.Empty<Book>();
        }

        /// <summary>
        /// Calculate total book cost for this grade at a specific site
        /// </summary>
        /// <param name="siteId">Site ID for pricing</param>
        /// <param name="includeOptional">Include optional books in calculation</param>
        /// <returns>Total book cost</returns>
        public decimal CalculateTotalBookCost(int siteId, bool includeOptional = false)
        {
            var relevantGradeBooks = includeOptional
                ? GradeBooks
                : GradeBooks?.Where(gb => gb.IsRequired);

            return relevantGradeBooks?
                .SelectMany(gb => gb.Book.BookPrices.Where(bp => bp.SiteId == siteId && bp.IsActive))
                .Sum(bp => bp.Price * GradeBooks.First(gb => gb.BookId == bp.BookId).Quantity) ?? 0;
        }

        /// <summary>
        /// Get next grade in sequence based on sort order
        /// </summary>
        /// <returns>Next grade or null if this is the highest grade</returns>
        public Grade? GetNextGrade()
        {
            if (!SortOrder.HasValue) return null;

            return Company?.Grades
                .Where(g => g.IsActive && g.SortOrder > SortOrder)
                .OrderBy(g => g.SortOrder)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get previous grade in sequence based on sort order
        /// </summary>
        /// <returns>Previous grade or null if this is the lowest grade</returns>
        public Grade? GetPreviousGrade()
        {
            if (!SortOrder.HasValue) return null;

            return Company?.Grades
                .Where(g => g.IsActive && g.SortOrder < SortOrder)
                .OrderByDescending(g => g.SortOrder)
                .FirstOrDefault();
        }

        /// <summary>
        /// Check if a student can be promoted to this grade
        /// </summary>
        /// <param name="student">Student to check</param>
        /// <returns>True if student can be promoted to this grade</returns>
        public bool CanStudentBePromotedTo(Student student)
        {
            if (student.CurrentGradeId == null) return IsBeginnerGrade;

            var currentGrade = Company?.Grades.FirstOrDefault(g => g.GradeId == student.CurrentGradeId);
            if (currentGrade == null) return false;

            return currentGrade.GetNextGrade()?.GradeId == GradeId;
        }

        /// <summary>
        /// Get students ready for examination
        /// </summary>
        /// <returns>Students who completed sufficient lessons and ready for exam</returns>
        public IEnumerable<Student> GetStudentsReadyForExam()
        {
            if (!Duration.HasValue) return Enumerable.Empty<Student>();

            var cutoffDate = DateTime.Now.AddDays(-Duration.Value * 7);

            return CurrentStudents?.Where(s =>
                s.IsActive &&
                s.Status == "Active" &&
                s.StudentGradeHistories.Any(sgh =>
                    sgh.GradeId == GradeId &&
                    sgh.IsCurrentGrade &&
                    sgh.StartDate <= cutoffDate &&
                    sgh.CompletionPercentage >= 80)) ?? Enumerable.Empty<Student>();
        }

        /// <summary>
        /// Get completion statistics for this grade
        /// </summary>
        /// <returns>Dictionary containing completion statistics</returns>
        public Dictionary<string, object> GetCompletionStatistics()
        {
            var histories = StudentGradeHistories?.ToList() ?? new List<StudentGradeHistory>();
            var totalStudents = histories.Count;
            var completedStudents = histories.Count(h => h.Status == "Completed");
            var activeStudents = histories.Count(h => h.Status == "Active");
            var extendedStudents = histories.Count(h => h.Status == "Extended");

            return new Dictionary<string, object>
            {
                {"TotalStudents", totalStudents},
                {"CompletedStudents", completedStudents},
                {"ActiveStudents", activeStudents},
                {"ExtendedStudents", extendedStudents},
                {"CompletionRate", totalStudents > 0 ? (double)completedStudents / totalStudents * 100 : 0},
                {"AverageCompletionPercentage", AverageCompletionRate},
                {"ExtensionRate", totalStudents > 0 ? (double)extendedStudents / totalStudents * 100 : 0}
            };
        }

        /// <summary>
        /// Generate grade progression report
        /// </summary>
        /// <param name="startDate">Report start date</param>
        /// <param name="endDate">Report end date</param>
        /// <returns>Grade progression data</returns>
        public Dictionary<string, object> GetProgressionReport(DateTime startDate, DateTime endDate)
        {
            var periodHistories = StudentGradeHistories?
                .Where(sgh => sgh.StartDate >= startDate && sgh.StartDate <= endDate)
                .ToList() ?? new List<StudentGradeHistory>();

            var newEnrollments = periodHistories.Count(h => h.StartDate >= startDate);
            var completions = periodHistories.Count(h => h.EndDate.HasValue &&
                                                       h.EndDate >= startDate &&
                                                       h.EndDate <= endDate &&
                                                       h.Status == "Completed");

            return new Dictionary<string, object>
            {
                {"PeriodStart", startDate},
                {"PeriodEnd", endDate},
                {"NewEnrollments", newEnrollments},
                {"Completions", completions},
                {"CurrentActive", ActiveStudentsCount},
                {"Revenue", GetMonthlyRevenue(startDate, endDate)}
            };
        }

        /// <summary>
        /// Get monthly revenue for a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Total revenue for the period</returns>
        private decimal GetMonthlyRevenue(DateTime startDate, DateTime endDate)
        {
            return StudentBillings?
                .Where(b => b.Status == "Paid" &&
                           b.PaymentDate >= startDate &&
                           b.PaymentDate <= endDate)
                .Sum(b => b.PaymentAmount ?? 0) ?? 0;
        }

        /// <summary>
        /// Validate grade progression rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateGradeRules()
        {
            var errors = new List<string>();

            // Check if there are required books
            if (!GradeBooks.Any(gb => gb.IsRequired))
            {
                errors.Add("Grade must have at least one required book");
            }

            // Check if duration is reasonable
            if (Duration.HasValue && Duration < 4)
            {
                errors.Add("Grade duration should be at least 4 weeks");
            }

            // Check if sort order is unique within company
            var duplicateSortOrder = Company?.Grades
                .Where(g => g.IsActive && g.GradeId != GradeId && g.SortOrder == SortOrder)
                .Any() == true;

            if (duplicateSortOrder)
            {
                errors.Add("Sort order must be unique within the company");
            }

            return errors;
        }

        /// <summary>
        /// Students registered for this examination
        /// </summary>
        [Display(Name = "Students")]
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the grade</returns>
        public override string ToString() => DisplayName;
    }
}
