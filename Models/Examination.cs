using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents an examination/test session in the KMSI Course Management System
    /// Manages exam scheduling, capacity, and examiner assignments for grade progression
    /// </summary>
    [Table("Examinations")]
    public class Examination
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExaminationId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Exam code is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Exam code must be between 3 and 20 characters")]
        [Display(Name = "Exam Code")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Exam code must contain only uppercase letters, numbers, and hyphens")]
        public string ExamCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Exam name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Exam name must be between 3 and 100 characters")]
        [Display(Name = "Exam Name")]
        public string ExamName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade is required")]
        [Display(Name = "Grade")]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Exam date is required")]
        [Display(Name = "Exam Date")]
        [DataType(DataType.Date)]
        public DateTime ExamDate { get; set; } = DateTime.Today.AddDays(7); // Default to next week

        [Required(ErrorMessage = "Start time is required")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Examiner teacher is required")]
        [Display(Name = "Examiner Teacher")]
        [ForeignKey("ExaminerTeacher")]
        public int ExaminerTeacherId { get; set; }

        [Range(1, 50, ErrorMessage = "Maximum capacity must be between 1 and 50")]
        [Display(Name = "Maximum Capacity")]
        public int MaxCapacity { get; set; } = 10;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled"; // Scheduled, In Progress, Completed, Cancelled

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

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
        /// Company organizing this examination
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where the examination takes place
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Grade level being examined
        /// </summary>
        [Required]
        [Display(Name = "Grade")]
        public virtual Grade Grade { get; set; } = null!;

        /// <summary>
        /// Teacher conducting the examination
        /// </summary>
        [Required]
        [Display(Name = "Examiner Teacher")]
        public virtual Teacher ExaminerTeacher { get; set; } = null!;

        /// <summary>
        /// User who created this examination record
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this examination record
        /// </summary>
        [Display(Name = "Updated By")]
        public virtual User? UpdatedByUser { get; set; }

        /// <summary>
        /// Student examinations (students taking this exam)
        /// </summary>
        [Display(Name = "Student Examinations")]
        public virtual ICollection<StudentExamination> StudentExaminations { get; set; } = new List<StudentExamination>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{ExamName} - {ExamDate:dd/MM/yyyy}";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplayName => $"{ExamCode} - {Grade?.GradeCode}";

        /// <summary>
        /// Exam duration in minutes
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes
        {
            get
            {
                return (int)(EndTime - StartTime).TotalMinutes;
            }
        }

        /// <summary>
        /// Formatted exam schedule
        /// </summary>
        [NotMapped]
        [Display(Name = "Schedule")]
        public string ScheduleDisplay => $"{ExamDate:dd/MM/yyyy} at {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

        /// <summary>
        /// Status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => Status switch
        {
            "Scheduled" => "📅 Scheduled",
            "In Progress" => "⏰ In Progress",
            "Completed" => "✅ Completed",
            "Cancelled" => "❌ Cancelled",
            _ => Status
        };

        /// <summary>
        /// Current enrollment count
        /// </summary>
        [NotMapped]
        [Display(Name = "Enrolled Students")]
        public int EnrolledStudentsCount => StudentExaminations?.Count ?? 0;

        /// <summary>
        /// Available capacity
        /// </summary>
        [NotMapped]
        [Display(Name = "Available Capacity")]
        public int AvailableCapacity => MaxCapacity - EnrolledStudentsCount;

        /// <summary>
        /// Indicates if exam is full
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Full")]
        public bool IsFull => EnrolledStudentsCount >= MaxCapacity;

        /// <summary>
        /// Indicates if exam is open for registration
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Open for Registration")]
        public bool IsOpenForRegistration => Status == "Scheduled" && !IsFull && ExamDate > DateTime.Today;

        /// <summary>
        /// Indicates if exam is active (scheduled or in progress)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Active")]
        public bool IsActive => Status == "Scheduled" || Status == "In Progress";

        /// <summary>
        /// Indicates if exam is completed
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Completed")]
        public bool IsCompleted => Status == "Completed";

        /// <summary>
        /// Days until exam
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Exam")]
        public int DaysUntilExam => (ExamDate.Date - DateTime.Today).Days;

        /// <summary>
        /// Exam status indicator for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Exam Status Indicator")]
        public string ExamStatusIndicator
        {
            get
            {
                if (Status == "Cancelled") return "Cancelled";
                if (Status == "Completed") return "Completed";

                var daysUntil = DaysUntilExam;
                return daysUntil switch
                {
                    < 0 => "Overdue",
                    0 => "Today",
                    1 => "Tomorrow",
                    <= 7 => "This Week",
                    <= 30 => "This Month",
                    _ => "Future"
                };
            }
        }

        /// <summary>
        /// Attendance count (students who attended)
        /// </summary>
        [NotMapped]
        [Display(Name = "Attendance Count")]
        public int AttendanceCount => StudentExaminations?.Count(se => se.AttendanceStatus == "Present") ?? 0;

        /// <summary>
        /// Pass rate percentage
        /// </summary>
        [NotMapped]
        [Display(Name = "Pass Rate")]
        public decimal PassRate
        {
            get
            {
                var completedExams = StudentExaminations?.Where(se => !string.IsNullOrEmpty(se.Result)).ToList();
                if (completedExams == null || !completedExams.Any()) return 0;

                var passedCount = completedExams.Count(se => se.Result == "Pass");
                return Math.Round((decimal)passedCount / completedExams.Count * 100, 2);
            }
        }

        /// <summary>
        /// Average score of all students
        /// </summary>
        [NotMapped]
        [Display(Name = "Average Score")]
        public decimal AverageScore
        {
            get
            {
                var scoredExams = StudentExaminations?.Where(se => se.Score.HasValue).ToList();
                if (scoredExams == null || !scoredExams.Any()) return 0;

                return Math.Round(scoredExams.Average(se => se.Score!.Value), 2);
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Validate examination business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateExaminationRules()
        {
            var errors = new List<string>();

            // Time validation
            if (StartTime >= EndTime)
            {
                errors.Add("End time must be after start time");
            }

            // Duration validation
            if (DurationMinutes < 15)
            {
                errors.Add("Examination duration should be at least 15 minutes");
            }

            if (DurationMinutes > 480) // 8 hours
            {
                errors.Add("Examination duration should not exceed 8 hours");
            }

            // Date validation
            if (ExamDate.Date < DateTime.Today && Status == "Scheduled")
            {
                errors.Add("Scheduled examination date cannot be in the past");
            }

            // Capacity validation
            if (MaxCapacity <= 0)
            {
                errors.Add("Maximum capacity must be greater than zero");
            }

            if (MaxCapacity > 100)
            {
                errors.Add("Maximum capacity seems excessive (>100), please verify");
            }

            // Enrollment validation
            if (EnrolledStudentsCount > MaxCapacity)
            {
                errors.Add($"Current enrollment ({EnrolledStudentsCount}) exceeds maximum capacity ({MaxCapacity})");
            }

            return errors;
        }

        /// <summary>
        /// Generate next exam code for a site and date
        /// </summary>
        /// <param name="siteCode">Site code</param>
        /// <param name="gradeCode">Grade code</param>
        /// <param name="examDate">Exam date</param>
        /// <param name="existingCodes">Existing exam codes</param>
        /// <returns>Next exam code</returns>
        public static string GenerateExamCode(string siteCode, string gradeCode, DateTime examDate, IEnumerable<string> existingCodes)
        {
            var dateCode = examDate.ToString("yyMM");
            var prefix = $"EX-{siteCode}-{gradeCode}-{dateCode}-";

            var existingNumbers = existingCodes
                .Where(c => c.StartsWith(prefix))
                .Select(c =>
                {
                    var parts = c.Split('-');
                    return parts.Length == 5 && int.TryParse(parts[4], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingNumbers.Max() + 1;
            return $"{prefix}{nextNumber:00}";
        }

        /// <summary>
        /// Check if a student can register for this exam
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <returns>True if student can register</returns>
        public bool CanStudentRegister(int studentId)
        {
            if (!IsOpenForRegistration) return false;

            // Check if student is already registered
            return !StudentExaminations.Any(se => se.StudentId == studentId);
        }

        /// <summary>
        /// Register a student for the examination
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="registeredBy">User ID who registered the student</param>
        /// <returns>True if registration was successful</returns>
        public bool RegisterStudent(int studentId, int? registeredBy = null)
        {
            if (!CanStudentRegister(studentId)) return false;

            var studentExam = new StudentExamination
            {
                ExaminationId = ExaminationId,
                StudentId = studentId,
                RegistrationDate = DateTime.Now,
                CreatedBy = registeredBy
            };

            StudentExaminations.Add(studentExam);
            return true;
        }

        /// <summary>
        /// Update examination status with validation
        /// </summary>
        /// <param name="newStatus">New status</param>
        /// <returns>True if status was updated successfully</returns>
        public bool UpdateStatus(string newStatus)
        {
            var validTransitions = GetValidStatusTransitions();
            if (!validTransitions.Contains(newStatus))
                return false;

            Status = newStatus;
            UpdatedDate = DateTime.Now;

            return true;
        }

        /// <summary>
        /// Get valid status transitions based on current status
        /// </summary>
        /// <returns>List of valid next statuses</returns>
        public List<string> GetValidStatusTransitions()
        {
            return Status switch
            {
                "Scheduled" => new List<string> { "In Progress", "Cancelled" },
                "In Progress" => new List<string> { "Completed", "Cancelled" },
                "Completed" => new List<string>(), // Terminal status
                "Cancelled" => new List<string> { "Scheduled" }, // Can reschedule
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Calculate exam statistics
        /// </summary>
        /// <returns>Dictionary with exam statistics</returns>
        public Dictionary<string, object> CalculateExamStatistics()
        {
            var completedExams = StudentExaminations.Where(se => !string.IsNullOrEmpty(se.Result)).ToList();
            var scoredExams = StudentExaminations.Where(se => se.Score.HasValue).ToList();

            return new Dictionary<string, object>
            {
                {"ExaminationId", ExaminationId},
                {"ExamCode", ExamCode},
                {"ExamName", ExamName},
                {"ExamDate", ExamDate},
                {"Status", Status},
                {"MaxCapacity", MaxCapacity},
                {"EnrolledCount", EnrolledStudentsCount},
                {"AttendanceCount", AttendanceCount},
                {"CompletedCount", completedExams.Count},
                {"PassedCount", completedExams.Count(se => se.Result == "Pass")},
                {"FailedCount", completedExams.Count(se => se.Result == "Fail")},
                {"PassRate", PassRate},
                {"AverageScore", AverageScore},
                {"HighestScore", scoredExams.Any() ? scoredExams.Max(se => se.Score!.Value) : (decimal?)null},
                {"LowestScore", scoredExams.Any() ? scoredExams.Min(se => se.Score!.Value) : (decimal?)null},
                {"DurationMinutes", DurationMinutes},
                {"Location", Location},
                {"ExaminerName", ExaminerTeacher?.User?.FullName}
            };
        }

        /// <summary>
        /// Get students eligible for this examination
        /// </summary>
        /// <returns>Query for eligible students</returns>
        public ICollection<Student> GetEligibleStudents()
        {
            // Students in the same grade who are not already registered
            var registeredStudentIds = StudentExaminations.Select(se => se.StudentId).ToList();

            return Grade.Students
                .Where(s => s.IsActive && s.Status == "Active")
                .Where(s => !registeredStudentIds.Contains(s.StudentId)).ToList();
        }

        /// <summary>
        /// Create a new examination
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="gradeId">Grade ID</param>
        /// <param name="examinerTeacherId">Examiner teacher ID</param>
        /// <param name="examDate">Exam date</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <param name="examName">Exam name</param>
        /// <param name="location">Location</param>
        /// <param name="maxCapacity">Maximum capacity</param>
        /// <param name="createdBy">Creator user ID</param>
        /// <returns>New examination instance</returns>
        public static Examination CreateExamination(int companyId, int siteId, int gradeId, int examinerTeacherId,
            DateTime examDate, TimeSpan startTime, TimeSpan endTime, string examName,
            string? location = null, int maxCapacity = 10, int? createdBy = null)
        {
            return new Examination
            {
                CompanyId = companyId,
                SiteId = siteId,
                GradeId = gradeId,
                ExaminerTeacherId = examinerTeacherId,
                ExamDate = examDate,
                StartTime = startTime,
                EndTime = endTime,
                ExamName = examName,
                Location = location,
                MaxCapacity = maxCapacity,
                Status = "Scheduled",
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the examination</returns>
        public override string ToString() => DisplayName;
    }
}
