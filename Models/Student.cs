using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a student in the KMSI Course Management System
    /// Contains student information, academic progress, and contact details
    /// </summary>
    [Table("Students")]
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Student code is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Student code must be between 5 and 20 characters")]
        [Display(Name = "Student Code")]
        [RegularExpression("^[A-Z]{3}-[0-9]{4}-[0-9]{5}$", ErrorMessage = "Student code format: KMI-2401-01530")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Full name computed column (FirstName + LastName)
        /// </summary>
        [StringLength(101, ErrorMessage = "Full name cannot exceed 101 characters")]
        [Display(Name = "Full Name")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(1, ErrorMessage = "Gender must be a single character")]
        [Display(Name = "Gender")]
        [RegularExpression("^[MF]$", ErrorMessage = "Gender must be M (Male) or F (Female)")]
        public string? Gender { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        [Display(Name = "City")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Parent name is required")]
        [StringLength(100, ErrorMessage = "Parent name cannot exceed 100 characters")]
        [Display(Name = "Parent/Guardian Name")]
        public string ParentName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Parent phone cannot exceed 20 characters")]
        [Display(Name = "Parent/Guardian Phone")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? ParentPhone { get; set; }

        [StringLength(100, ErrorMessage = "Parent email cannot exceed 100 characters")]
        [Display(Name = "Parent/Guardian Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? ParentEmail { get; set; }

        [StringLength(255, ErrorMessage = "Photo path cannot exceed 255 characters")]
        [Display(Name = "Photo")]
        public string? PhotoPath { get; set; }

        [Required(ErrorMessage = "Registration date is required")]
        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Trial, Active, Inactive, Graduated

        [Display(Name = "Current Grade")]
        [ForeignKey("CurrentGrade")]
        public int? CurrentGradeId { get; set; }

        [Display(Name = "Assigned Teacher")]
        [ForeignKey("AssignedTeacher")]
        public int? AssignedTeacherId { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

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
        /// Company this student belongs to
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where this student is registered
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Current grade/level of the student
        /// </summary>
        [Display(Name = "Current Grade")]
        public virtual Grade? CurrentGrade { get; set; }

        /// <summary>
        /// Teacher assigned to this student
        /// </summary>
        [Display(Name = "Assigned Teacher")]
        public virtual Teacher? AssignedTeacher { get; set; }

        /// <summary>
        /// Student's grade history
        /// </summary>
        [Display(Name = "Grade History")]
        public virtual ICollection<StudentGradeHistory> StudentGradeHistories { get; set; } = new List<StudentGradeHistory>();

        /// <summary>
        /// Registration records for this student
        /// </summary>
        [Display(Name = "Registrations")]
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        /// <summary>
        /// Class schedules for this student
        /// </summary>
        [Display(Name = "Class Schedules")]
        public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

        /// <summary>
        /// Attendance records for this student
        /// </summary>
        [Display(Name = "Attendances")]
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        /// <summary>
        /// Examination records for this student
        /// </summary>
        [Display(Name = "Student Examinations")]
        public virtual ICollection<StudentExamination> StudentExaminations { get; set; } = new List<StudentExamination>();

        /// <summary>
        /// Certificates earned by this student
        /// </summary>
        [Display(Name = "Certificates")]
        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        /// <summary>
        /// Billing records for this student
        /// </summary>
        [Display(Name = "Student Billings")]
        public virtual ICollection<StudentBilling> StudentBillings { get; set; } = new List<StudentBilling>();

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this student record
        /// </summary>
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this student record
        /// </summary>
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining code and full name
        /// </summary>
        [NotMapped]
        [Display(Name = "Student")]
        public string DisplayName => $"{StudentCode} - {FullName}";

        /// <summary>
        /// Full display name with status
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Student Info")]
        public string FullDisplayName => $"{StudentCode} - {FullName} ({Status})";

        /// <summary>
        /// Status display with color information
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => Status;

        /// <summary>
        /// Status color for UI styling
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Color")]
        public string StatusColor => Status switch
        {
            "Pending" => "warning",
            "Trial" => "info",
            "Active" => "success",
            "Inactive" => "secondary",
            "Graduated" => "primary",
            _ => "dark"
        };

        /// <summary>
        /// Gender display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Gender")]
        public string GenderDisplay => Gender switch
        {
            "M" => "Male",
            "F" => "Female",
            _ => "Not Specified"
        };

        /// <summary>
        /// Age calculated from date of birth
        /// </summary>
        [NotMapped]
        [Display(Name = "Age")]
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue) return null;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Age group classification
        /// </summary>
        [NotMapped]
        [Display(Name = "Age Group")]
        public string AgeGroup
        {
            get
            {
                if (!Age.HasValue) return "Unknown";
                return Age.Value switch
                {
                    < 6 => "Preschool",
                    >= 6 and <= 12 => "Elementary",
                    >= 13 and <= 15 => "Junior High",
                    >= 16 and <= 18 => "Senior High",
                    >= 19 and <= 25 => "Young Adult",
                    > 25 => "Adult"
                };
            }
        }

        /// <summary>
        /// Days since registration
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Registration")]
        public int DaysSinceRegistration => (DateTime.Today - RegistrationDate.Date).Days;

        /// <summary>
        /// Registration age category
        /// </summary>
        [NotMapped]
        [Display(Name = "Registration Age")]
        public string RegistrationAge
        {
            get
            {
                var days = DaysSinceRegistration;
                return days switch
                {
                    < 30 => "New Student",
                    < 90 => "Recent Student",
                    < 365 => "This Year",
                    < 730 => "Last Year",
                    _ => "Long-term Student"
                };
            }
        }

        /// <summary>
        /// Full address combining address and city
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
                return string.Join(", ", addressParts);
            }
        }

        /// <summary>
        /// Primary contact information (student or parent)
        /// </summary>
        [NotMapped]
        [Display(Name = "Primary Contact")]
        public string PrimaryContactInfo
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Phone) || !string.IsNullOrWhiteSpace(Email))
                {
                    return $"{FullName} ({Phone ?? Email})";
                }
                return $"{ParentName} ({ParentPhone ?? ParentEmail})";
            }
        }

        /// <summary>
        /// Contact preference (Student or Parent)
        /// </summary>
        [NotMapped]
        [Display(Name = "Contact Preference")]
        public string ContactPreference
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Phone) || !string.IsNullOrWhiteSpace(Email))
                    return "Student";
                return "Parent/Guardian";
            }
        }

        /// <summary>
        /// Current grade display
        /// </summary>
        [NotMapped]
        [Display(Name = "Current Grade")]
        public string CurrentGradeDisplay => CurrentGrade?.DisplayName ?? "Not Assigned";

        /// <summary>
        /// Assigned teacher display
        /// </summary>
        [NotMapped]
        [Display(Name = "Assigned Teacher")]
        public string AssignedTeacherDisplay => AssignedTeacher?.DisplayName ?? "Not Assigned";

        /// <summary>
        /// Total classes attended
        /// </summary>
        [NotMapped]
        [Display(Name = "Classes Attended")]
        public int TotalClassesAttended => Attendances?.Count(a => a.Status == "Present") ?? 0;

        /// <summary>
        /// Total classes scheduled
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Classes")]
        public int TotalClassesScheduled => ClassSchedules?.Count(cs => cs.Status == "Completed" || cs.Status == "Scheduled") ?? 0;

        /// <summary>
        /// Attendance rate percentage
        /// </summary>
        [NotMapped]
        [Display(Name = "Attendance Rate")]
        [DisplayFormat(DataFormatString = "{0:P1}", ApplyFormatInEditMode = true)]
        public double AttendanceRate
        {
            get
            {
                if (TotalClassesScheduled == 0) return 0;
                return (double)TotalClassesAttended / TotalClassesScheduled;
            }
        }

        /// <summary>
        /// Current month's classes attended
        /// </summary>
        [NotMapped]
        [Display(Name = "This Month Attendance")]
        public int ThisMonthAttendance
        {
            get
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                return Attendances?.Count(a =>
                    a.AttendanceDate.Month == currentMonth &&
                    a.AttendanceDate.Year == currentYear &&
                    a.Status == "Present") ?? 0;
            }
        }

        /// <summary>
        /// Outstanding billing amount
        /// </summary>
        [NotMapped]
        [Display(Name = "Outstanding Amount")]
        [DataType(DataType.Currency)]
        public decimal OutstandingAmount => StudentBillings?
            .Where(b => b.Status == "Outstanding" || b.Status == "Overdue")
            .Sum(b => b.TotalAmount) ?? 0;

        /// <summary>
        /// Check if student has outstanding bills
        /// </summary>
        [NotMapped]
        [Display(Name = "Has Outstanding Bills")]
        public bool HasOutstandingBills => OutstandingAmount > 0;

        /// <summary>
        /// Number of certificates earned
        /// </summary>
        [NotMapped]
        [Display(Name = "Certificates Earned")]
        public int CertificatesCount => Certificates?.Count ?? 0;

        /// <summary>
        /// Last attendance date
        /// </summary>
        [NotMapped]
        [Display(Name = "Last Attendance")]
        public DateTime? LastAttendanceDate => Attendances?
            .Where(a => a.Status == "Present")
            .OrderByDescending(a => a.AttendanceDate)
            .FirstOrDefault()?.AttendanceDate;

        /// <summary>
        /// Next scheduled class
        /// </summary>
        [NotMapped]
        [Display(Name = "Next Class")]
        public DateTime? NextClassDate => ClassSchedules?
            .Where(cs => cs.ScheduleDate >= DateTime.Today && cs.Status == "Scheduled")
            .OrderBy(cs => cs.ScheduleDate)
            .ThenBy(cs => cs.StartTime)
            .FirstOrDefault()?.ScheduleDate;

        /// <summary>
        /// Days since last attendance
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Last Class")]
        public int? DaysSinceLastAttendance => LastAttendanceDate.HasValue
            ? (DateTime.Today - LastAttendanceDate.Value.Date).Days
            : null;

        /// <summary>
        /// Student engagement level based on attendance
        /// </summary>
        [NotMapped]
        [Display(Name = "Engagement Level")]
        public string EngagementLevel
        {
            get
            {
                if (AttendanceRate >= 0.9) return "Excellent";
                if (AttendanceRate >= 0.8) return "Good";
                if (AttendanceRate >= 0.7) return "Fair";
                if (AttendanceRate >= 0.5) return "Needs Improvement";
                return "Poor";
            }
        }

        /// <summary>
        /// Academic progress based on grade history
        /// </summary>
        [NotMapped]
        [Display(Name = "Academic Progress")]
        public string AcademicProgress
        {
            get
            {
                var currentGradeHistory = StudentGradeHistories?
                    .FirstOrDefault(sgh => sgh.IsCurrentGrade);

                if (currentGradeHistory == null) return "Not Started";

                return currentGradeHistory.CompletionPercentage switch
                {
                    >= 90 => "Ready for Graduation",
                    >= 75 => "Advanced",
                    >= 50 => "Progressing",
                    >= 25 => "Beginning",
                    _ => "Just Started"
                };
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Check if student code is unique within the same company
        /// </summary>
        /// <param name="otherStudents">Other students to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsStudentCodeUnique(IEnumerable<Student> otherStudents)
        {
            return !otherStudents.Any(s =>
                s.StudentId != StudentId &&
                s.StudentCode.ToUpper() == StudentCode.ToUpper() &&
                s.CompanyId == CompanyId);
        }

        /// <summary>
        /// Check if student can be promoted to next grade
        /// </summary>
        /// <returns>True if student is ready for promotion</returns>
        public bool CanBePromoted()
        {
            if (Status != "Active" || CurrentGrade == null) return false;

            var currentGradeHistory = StudentGradeHistories?
                .FirstOrDefault(sgh => sgh.IsCurrentGrade);

            return currentGradeHistory?.CompletionPercentage >= 80 &&
                   AttendanceRate >= 0.75;
        }

        /// <summary>
        /// Get student's current grade completion percentage
        /// </summary>
        /// <returns>Completion percentage (0-100)</returns>
        public decimal GetCurrentGradeCompletion()
        {
            var currentGradeHistory = StudentGradeHistories?
                .FirstOrDefault(sgh => sgh.IsCurrentGrade);

            return currentGradeHistory?.CompletionPercentage ?? 0;
        }

        /// <summary>
        /// Promote student to next grade
        /// </summary>
        /// <param name="nextGradeId">Next grade ID</param>
        /// <param name="promotedBy">User ID who promoted</param>
        /// <returns>True if promotion successful</returns>
        public bool PromoteToNextGrade(int nextGradeId, int promotedBy)
        {
            if (!CanBePromoted()) return false;

            // Complete current grade
            var currentGradeHistory = StudentGradeHistories?
                .FirstOrDefault(sgh => sgh.IsCurrentGrade);

            if (currentGradeHistory != null)
            {
                currentGradeHistory.Status = "Completed";
                currentGradeHistory.EndDate = DateTime.Today;
                currentGradeHistory.IsCurrentGrade = false;
                currentGradeHistory.CompletionPercentage = 100;
            }

            // Add new grade history
            var newGradeHistory = new StudentGradeHistory
            {
                StudentId = StudentId,
                GradeId = nextGradeId,
                StartDate = DateTime.Today,
                Status = "Active",
                IsCurrentGrade = true,
                CompletionPercentage = 0
            };

            StudentGradeHistories.Add(newGradeHistory);
            CurrentGradeId = nextGradeId;
            UpdatedBy = promotedBy;
            UpdatedDate = DateTime.Now;

            return true;
        }

        /// <summary>
        /// Change student status with validation
        /// </summary>
        /// <param name="newStatus">New status</param>
        /// <param name="changedBy">User ID who changed status</param>
        /// <returns>True if change successful</returns>
        public bool ChangeStatus(string newStatus, int changedBy)
        {
            var validStatuses = new[] { "Pending", "Trial", "Active", "Inactive", "Graduated" };
            if (!validStatuses.Contains(newStatus)) return false;

            // Business rules for status changes
            if (Status == "Graduated" && newStatus != "Graduated")
                return false; // Cannot change from graduated

            if (newStatus == "Graduated" && !CanBePromoted())
                return false; // Cannot graduate without meeting requirements

            Status = newStatus;
            UpdatedBy = changedBy;
            UpdatedDate = DateTime.Now;

            // Special handling for graduation
            if (newStatus == "Graduated")
            {
                var currentGradeHistory = StudentGradeHistories?
                    .FirstOrDefault(sgh => sgh.IsCurrentGrade);

                if (currentGradeHistory != null)
                {
                    currentGradeHistory.Status = "Completed";
                    currentGradeHistory.EndDate = DateTime.Today;
                    currentGradeHistory.CompletionPercentage = 100;
                }
            }

            return true;
        }

        /// <summary>
        /// Assign teacher to student
        /// </summary>
        /// <param name="teacherId">Teacher ID</param>
        /// <param name="assignedBy">User ID who assigned</param>
        /// <returns>True if assignment successful</returns>
        public bool AssignTeacher(int teacherId, int assignedBy)
        {
            AssignedTeacherId = teacherId;
            UpdatedBy = assignedBy;
            UpdatedDate = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Get student's performance summary
        /// </summary>
        /// <param name="periodStart">Performance period start</param>
        /// <param name="periodEnd">Performance period end</param>
        /// <returns>Performance summary data</returns>
        public Dictionary<string, object> GetPerformanceSummary(DateTime? periodStart = null, DateTime? periodEnd = null)
        {
            periodStart ??= DateTime.Now.AddMonths(-1);
            periodEnd ??= DateTime.Now;

            var periodAttendances = Attendances?
                .Where(a => a.AttendanceDate >= periodStart && a.AttendanceDate <= periodEnd)
                .ToList() ?? new List<Attendance>();

            var periodSchedules = ClassSchedules?
                .Where(cs => cs.ScheduleDate >= periodStart && cs.ScheduleDate <= periodEnd)
                .ToList() ?? new List<ClassSchedule>();

            return new Dictionary<string, object>
            {
                {"StudentInfo", new { StudentId, StudentCode, FullName, Status, CurrentGrade = CurrentGradeDisplay }},
                {"Period", new { Start = periodStart, End = periodEnd }},
                {"Attendance", new
                {
                    TotalScheduled = periodSchedules.Count,
                    TotalAttended = periodAttendances.Count(a => a.Status == "Present"),
                    AttendanceRate = periodSchedules.Count > 0 ? (double)periodAttendances.Count(a => a.Status == "Present") / periodSchedules.Count : 0,
                    AbsenceCount = periodAttendances.Count(a => a.Status == "Absent"),
                    LateCount = periodAttendances.Count(a => a.Status == "Late")
                }},
                {"Academic", new
                {
                    CurrentGradeCompletion = GetCurrentGradeCompletion(),
                    AcademicProgress = AcademicProgress,
                    CanBePromoted = CanBePromoted(),
                    CertificatesEarned = CertificatesCount
                }},
                {"Financial", new
                {
                    OutstandingAmount = OutstandingAmount,
                    HasOutstandingBills = HasOutstandingBills
                }},
                {"Engagement", new
                {
                    EngagementLevel = EngagementLevel,
                    DaysSinceLastClass = DaysSinceLastAttendance,
                    NextClassDate = NextClassDate
                }}
            };
        }

        /// <summary>
        /// Generate student report card
        /// </summary>
        /// <param name="reportPeriod">Report period (semester, quarter, etc.)</param>
        /// <returns>Report card data</returns>
        public Dictionary<string, object> GenerateReportCard(string reportPeriod = "Current")
        {
            var performance = GetPerformanceSummary();
            var examResults = StudentExaminations?
                .Where(se => se.Result == "Pass")
                .OrderByDescending(se => se.Examination?.ExamDate)
                .Take(5)
                .Select(se => new
                {
                    se.Examination?.ExamName,
                    se.Score,
                    se.MaxScore,
                    se.Percentage,
                    se.Grade,
                    ExamDate = se.Examination?.ExamDate
                })
                .ToList();

            return new Dictionary<string, object>
            {
                {"Header", new
                {
                    StudentCode = StudentCode,
                    StudentName = FullName,
                    CurrentGrade = CurrentGradeDisplay,
                    AssignedTeacher = AssignedTeacherDisplay,
                    Site = Site?.SiteName,
                    ReportPeriod = reportPeriod,
                    GeneratedDate = DateTime.Now
                }},
                {"Performance", performance},
                {"ExamResults", examResults},
                {"Certificates", Certificates?.Select(c => new
                {
                    c.CertificateNumber,
                    c.CertificateTitle,
                    c.IssueDate
                }).ToList()},
                {"TeacherComments", Attendances?
                    .Where(a => !string.IsNullOrEmpty(a.TeacherNotes))
                    .OrderByDescending(a => a.AttendanceDate)
                    .Take(3)
                    .Select(a => new { a.AttendanceDate, a.TeacherNotes })
                    .ToList()},
                {"NextSteps", new
                {
                    CanBePromoted = CanBePromoted(),
                    NextGrade = CurrentGrade?.GetNextGrade()?.DisplayName,
                    RecommendedActions = GetRecommendedActions()
                }}
            };
        }

        /// <summary>
        /// Get recommended actions for student improvement
        /// </summary>
        /// <returns>List of recommended actions</returns>
        private List<string> GetRecommendedActions()
        {
            var actions = new List<string>();

            if (AttendanceRate < 0.8)
                actions.Add("Improve attendance rate - aim for at least 80%");

            if (OutstandingAmount > 0)
                actions.Add("Clear outstanding billing amount");

            if (GetCurrentGradeCompletion() < 50)
                actions.Add("Focus on completing current grade curriculum");

            if (DaysSinceLastAttendance > 7)
                actions.Add("Schedule regular classes to maintain learning momentum");

            if (actions.Count == 0)
                actions.Add("Continue excellent progress!");

            return actions;
        }

        /// <summary>
        /// Validate student business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateStudentRules()
        {
            var errors = new List<string>();

            // Age validation
            if (Age.HasValue && Age < 3)
                errors.Add("Student must be at least 3 years old");

            if (Age.HasValue && Age > 100)
                errors.Add("Invalid date of birth");

            // Contact validation
            if (string.IsNullOrEmpty(Phone) && string.IsNullOrEmpty(Email) &&
                string.IsNullOrEmpty(ParentPhone) && string.IsNullOrEmpty(ParentEmail))
                errors.Add("At least one contact method (student or parent) is required");

            // Grade and teacher consistency
            if (Status == "Active" && !CurrentGradeId.HasValue)
                errors.Add("Active students must be assigned to a grade");

            if (Status == "Active" && !AssignedTeacherId.HasValue)
                errors.Add("Active students must be assigned to a teacher");

            // Status consistency
            if (Status == "Graduated" && CurrentGradeId.HasValue)
            {
                var isHighestGrade = CurrentGrade?.IsAdvancedGrade ?? false;
                if (!isHighestGrade)
                    errors.Add("Can only graduate from the highest grade level");
            }

            return errors;
        }

        /// <summary>
        /// Get students at risk (poor attendance or overdue payments)
        /// </summary>
        /// <param name="students">Collection of students to analyze</param>
        /// <param name="attendanceThreshold">Minimum acceptable attendance rate</param>
        /// <param name="daysSinceLastClass">Maximum days without attendance</param>
        /// <returns>Students at risk</returns>
        public static List<Student> GetStudentsAtRisk(IEnumerable<Student> students,
            double attendanceThreshold = 0.7, int daysSinceLastClass = 14)
        {
            return students.Where(s =>
                s.IsActive &&
                s.Status == "Active" &&
                (s.AttendanceRate < attendanceThreshold ||
                 s.DaysSinceLastAttendance > daysSinceLastClass ||
                 s.HasOutstandingBills))
                .OrderBy(s => s.AttendanceRate)
                .ToList();
        }

        /// <summary>
        /// Get students ready for promotion
        /// </summary>
        /// <param name="students">Collection of students to analyze</param>
        /// <returns>Students ready for promotion</returns>
        public static List<Student> GetStudentsReadyForPromotion(IEnumerable<Student> students)
        {
            return students.Where(s => s.CanBePromoted())
                .OrderByDescending(s => s.GetCurrentGradeCompletion())
                .ToList();
        }

        /// <summary>
        /// Get student demographics summary
        /// </summary>
        /// <param name="students">Collection of students to analyze</param>
        /// <returns>Demographics summary</returns>
        public static Dictionary<string, object> GetDemographicsSummary(IEnumerable<Student> students)
        {
            var activeStudents = students.Where(s => s.IsActive).ToList();

            return new Dictionary<string, object>
            {
                {"Total", students.Count()},
                {"Active", activeStudents.Count},
                {"ByStatus", students.GroupBy(s => s.Status).ToDictionary(g => g.Key, g => g.Count())},
                {"ByGender", activeStudents.GroupBy(s => s.Gender ?? "Unknown").ToDictionary(g => g.Key, g => g.Count())},
                {"ByAgeGroup", activeStudents.GroupBy(s => s.AgeGroup).ToDictionary(g => g.Key, g => g.Count())},
                {"ByGrade", activeStudents.Where(s => s.CurrentGrade != null)
                    .GroupBy(s => s.CurrentGrade!.GradeName).ToDictionary(g => g.Key, g => g.Count())},
                {"BySite", activeStudents.GroupBy(s => s.Site?.SiteName ?? "Unknown").ToDictionary(g => g.Key, g => g.Count())},
                {"ByRegistrationAge", activeStudents.GroupBy(s => s.RegistrationAge).ToDictionary(g => g.Key, g => g.Count())},
                {"AverageAge", activeStudents.Where(s => s.Age.HasValue).Average(s => s.Age!.Value)},
                {"AttendanceStats", new
                {
                    AverageAttendanceRate = activeStudents.Average(s => s.AttendanceRate),
                    HighAttendance = activeStudents.Count(s => s.AttendanceRate >= 0.9),
                    LowAttendance = activeStudents.Count(s => s.AttendanceRate < 0.7)
                }}
            };
        }

        /// <summary>
        /// Generate student performance analytics
        /// </summary>
        /// <param name="students">Collection of students to analyze</param>
        /// <param name="periodStart">Analysis period start</param>
        /// <param name="periodEnd">Analysis period end</param>
        /// <returns>Performance analytics</returns>
        public static Dictionary<string, object> GetPerformanceAnalytics(IEnumerable<Student> students,
            DateTime? periodStart = null, DateTime? periodEnd = null)
        {
            periodStart ??= DateTime.Now.AddMonths(-3);
            periodEnd ??= DateTime.Now;

            var activeStudents = students.Where(s => s.IsActive && s.Status == "Active").ToList();

            return new Dictionary<string, object>
            {
                {"Period", new { Start = periodStart, End = periodEnd }},
                {"Overview", new
                {
                    TotalActiveStudents = activeStudents.Count,
                    HighPerformers = activeStudents.Count(s => s.AttendanceRate >= 0.9),
                    AtRiskStudents = GetStudentsAtRisk(activeStudents).Count,
                    ReadyForPromotion = GetStudentsReadyForPromotion(activeStudents).Count
                }},
                {"AttendanceMetrics", new
                {
                    AverageAttendanceRate = activeStudents.Average(s => s.AttendanceRate),
                    MedianAttendanceRate = GetMedianAttendanceRate(activeStudents),
                    AttendanceDistribution = new
                    {
                        Excellent = activeStudents.Count(s => s.AttendanceRate >= 0.9),
                        Good = activeStudents.Count(s => s.AttendanceRate >= 0.8 && s.AttendanceRate < 0.9),
                        Fair = activeStudents.Count(s => s.AttendanceRate >= 0.7 && s.AttendanceRate < 0.8),
                        Poor = activeStudents.Count(s => s.AttendanceRate < 0.7)
                    }
                }},
                {"AcademicProgress", new
                {
                    AverageCompletion = activeStudents.Average(s => (double)s.GetCurrentGradeCompletion()),
                    CompletionDistribution = activeStudents.GroupBy(s => s.AcademicProgress)
                        .ToDictionary(g => g.Key, g => g.Count())
                }},
                {"FinancialHealth", new
                {
                    StudentsWithOutstandingBills = activeStudents.Count(s => s.HasOutstandingBills),
                    TotalOutstandingAmount = activeStudents.Sum(s => s.OutstandingAmount),
                    AverageOutstandingPerStudent = activeStudents.Where(s => s.HasOutstandingBills)
                        .DefaultIfEmpty().Average(s => s?.OutstandingAmount ?? 0)
                }}
            };
        }

        /// <summary>
        /// Calculate median attendance rate
        /// </summary>
        /// <param name="students">Students to calculate median for</param>
        /// <returns>Median attendance rate</returns>
        private static double GetMedianAttendanceRate(List<Student> students)
        {
            if (!students.Any()) return 0;

            var sortedRates = students.Select(s => s.AttendanceRate).OrderBy(r => r).ToList();
            var count = sortedRates.Count;

            if (count % 2 == 0)
                return (sortedRates[count / 2 - 1] + sortedRates[count / 2]) / 2;
            else
                return sortedRates[count / 2];
        }

        /// <summary>
        /// Generate student retention report
        /// </summary>
        /// <param name="students">Collection of students to analyze</param>
        /// <param name="months">Number of months to analyze</param>
        /// <returns>Retention report data</returns>
        public static Dictionary<string, object> GetRetentionReport(IEnumerable<Student> students, int months = 12)
        {
            var cutoffDate = DateTime.Now.AddMonths(-months);
            var cohortStudents = students.Where(s => s.RegistrationDate >= cutoffDate).ToList();

            var retentionByMonth = new Dictionary<string, object>();
            for (int i = 1; i <= months; i++)
            {
                var monthStart = DateTime.Now.AddMonths(-i);
                var monthStudents = cohortStudents.Where(s => s.RegistrationDate >= monthStart).ToList();
                var stillActive = monthStudents.Count(s => s.Status == "Active");
                var retentionRate = monthStudents.Count > 0 ? (double)stillActive / monthStudents.Count : 0;

                retentionByMonth[$"Month {i}"] = new
                {
                    NewStudents = monthStudents.Count,
                    StillActive = stillActive,
                    RetentionRate = retentionRate
                };
            }

            return new Dictionary<string, object>
            {
                {"AnalysisPeriod", $"Last {months} months"},
                {"TotalCohortStudents", cohortStudents.Count},
                {"CurrentlyActive", cohortStudents.Count(s => s.Status == "Active")},
                {"OverallRetentionRate", cohortStudents.Count > 0 ?
                    (double)cohortStudents.Count(s => s.Status == "Active") / cohortStudents.Count : 0},
                {"RetentionByMonth", retentionByMonth},
                {"ChurnReasons", cohortStudents.Where(s => s.Status == "Inactive")
                    .GroupBy(s => s.Notes ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count())},
                {"GraduationRate", cohortStudents.Count > 0 ?
                    (double)cohortStudents.Count(s => s.Status == "Graduated") / cohortStudents.Count : 0}
            };
        }

        /// <summary>
        /// Get students requiring attention (for dashboard alerts)
        /// </summary>
        /// <param name="students">Collection of students to analyze</param>
        /// <returns>Students requiring attention with reasons</returns>
        public static List<object> GetStudentsRequiringAttention(IEnumerable<Student> students)
        {
            var alerts = new List<object>();

            foreach (var student in students.Where(s => s.IsActive))
            {
                var reasons = new List<string>();

                if (student.AttendanceRate < 0.7)
                    reasons.Add($"Low attendance ({student.AttendanceRate:P1})");

                if (student.DaysSinceLastAttendance > 14)
                    reasons.Add($"No class for {student.DaysSinceLastAttendance} days");

                if (student.HasOutstandingBills)
                    reasons.Add($"Outstanding bills: {student.OutstandingAmount:C}");

                if (student.CanBePromoted())
                    reasons.Add("Ready for promotion");

                if (reasons.Any())
                {
                    alerts.Add(new
                    {
                        Student = new { student.StudentId, student.StudentCode, student.FullName },
                        Priority = GetAlertPriority(reasons),
                        Reasons = reasons,
                        LastUpdate = student.UpdatedDate ?? student.CreatedDate
                    });
                }
            }

            return alerts.OrderBy(a => ((dynamic)a).Priority).ToList();
        }

        /// <summary>
        /// Determine alert priority based on reasons
        /// </summary>
        /// <param name="reasons">List of alert reasons</param>
        /// <returns>Priority level (1=highest, 5=lowest)</returns>
        private static int GetAlertPriority(List<string> reasons)
        {
            if (reasons.Any(r => r.Contains("Outstanding bills"))) return 1;
            if (reasons.Any(r => r.Contains("No class for") && r.Contains("days"))) return 2;
            if (reasons.Any(r => r.Contains("Low attendance"))) return 3;
            if (reasons.Any(r => r.Contains("Ready for promotion"))) return 4;
            return 5;
        }

        /// <summary>
        /// Generate comprehensive student summary for dashboard
        /// </summary>
        /// <returns>Complete student summary data</returns>
        public Dictionary<string, object> GetStudentDashboardSummary()
        {
            return new Dictionary<string, object>
            {
                {"BasicInfo", new
                {
                    StudentId = StudentId,
                    StudentCode = StudentCode,
                    FullName = FullName,
                    Status = Status,
                    StatusColor = StatusColor,
                    Age = Age,
                    AgeGroup = AgeGroup,
                    Gender = GenderDisplay,
                    RegistrationAge = RegistrationAge
                }},
                {"Academic", new
                {
                    CurrentGrade = CurrentGradeDisplay,
                    AssignedTeacher = AssignedTeacherDisplay,
                    GradeCompletion = GetCurrentGradeCompletion(),
                    AcademicProgress = AcademicProgress,
                    CanBePromoted = CanBePromoted(),
                    CertificatesCount = CertificatesCount
                }},
                {"Attendance", new
                {
                    TotalClassesAttended = TotalClassesAttended,
                    TotalClassesScheduled = TotalClassesScheduled,
                    AttendanceRate = AttendanceRate,
                    EngagementLevel = EngagementLevel,
                    ThisMonthAttendance = ThisMonthAttendance,
                    LastAttendanceDate = LastAttendanceDate,
                    DaysSinceLastAttendance = DaysSinceLastAttendance,
                    NextClassDate = NextClassDate
                }},
                {"Financial", new
                {
                    OutstandingAmount = OutstandingAmount,
                    HasOutstandingBills = HasOutstandingBills
                }},
                {"Contact", new
                {
                    PrimaryContactInfo = PrimaryContactInfo,
                    ContactPreference = ContactPreference,
                    StudentPhone = Phone,
                    StudentEmail = Email,
                    ParentName = ParentName,
                    ParentPhone = ParentPhone,
                    ParentEmail = ParentEmail,
                    FullAddress = FullAddress
                }},
                {"Metadata", new
                {
                    RegistrationDate = RegistrationDate,
                    DaysSinceRegistration = DaysSinceRegistration,
                    Site = Site?.SiteName,
                    Company = Company?.CompanyName,
                    CreatedDate = CreatedDate,
                    LastUpdated = UpdatedDate
                }}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the student</returns>
        public override string ToString() => DisplayName;
    }
}
