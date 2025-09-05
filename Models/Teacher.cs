using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a teacher in the KMSI Course Management System
    /// Extends the User model with teacher-specific information and functionality
    /// </summary>
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Teacher code is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Teacher code must be between 3 and 20 characters")]
        [Display(Name = "Teacher Code")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Teacher code must contain only uppercase letters, numbers, and hyphens")]
        public string TeacherCode { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
        [Display(Name = "Specialization")]
        public string? Specialization { get; set; }

        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
        [Display(Name = "Experience (Years)")]
        public int? ExperienceYears { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Hourly rate must be between 0 and 9,999,999.99")]
        [Display(Name = "Hourly Rate")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? HourlyRate { get; set; }

        [Range(1, 20, ErrorMessage = "Maximum students per day must be between 1 and 20")]
        [Display(Name = "Max Students Per Day")]
        public int MaxStudentsPerDay { get; set; } = 8;

        [Display(Name = "Available for Trial")]
        public bool IsAvailableForTrial { get; set; } = true;

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
        /// User account associated with this teacher
        /// </summary>
        [Required]
        [Display(Name = "User")]
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// Company that employs this teacher
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where this teacher works
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Teacher's working schedule
        /// </summary>
        [Display(Name = "Teacher Schedules")]
        public virtual ICollection<TeacherSchedule> TeacherSchedules { get; set; } = new List<TeacherSchedule>();

        /// <summary>
        /// Students assigned to this teacher
        /// </summary>
        [Display(Name = "Assigned Students")]
        public virtual ICollection<Student> AssignedStudents { get; set; } = new List<Student>();

        /// <summary>
        /// Class schedules for this teacher
        /// </summary>
        [Display(Name = "Class Schedules")]
        public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

        /// <summary>
        /// Attendance records taught by this teacher
        /// </summary>
        [Display(Name = "Attendances")]
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        /// <summary>
        /// Registrations assigned to this teacher
        /// </summary>
        [Display(Name = "Registrations")]
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        /// <summary>
        /// Examinations conducted by this teacher
        /// </summary>
        [Display(Name = "Examinations")]
        public virtual ICollection<Examination> ExaminationsConducted { get; set; } = new List<Examination>();

        /// <summary>
        /// Teacher payroll records
        /// </summary>
        [Display(Name = "Payrolls")]
        public virtual ICollection<TeacherPayroll> Payrolls { get; set; } = new List<TeacherPayroll>();

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this teacher record
        /// </summary>
        [InverseProperty("CreatedTeachers")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this teacher record
        /// </summary>
        [InverseProperty("UpdatedTeachers")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Teacher's full name from User
        /// </summary>
        [NotMapped]
        [Display(Name = "Teacher Name")]
        public string TeacherName => User?.FullName ?? "Unknown";

        /// <summary>
        /// Display name combining code and name
        /// </summary>
        [NotMapped]
        [Display(Name = "Teacher")]
        public string DisplayName => $"{TeacherCode} - {TeacherName}";

        /// <summary>
        /// Full display name with site information
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Teacher Info")]
        public string FullDisplayName => $"{TeacherCode} - {TeacherName} ({Site?.SiteName})";

        /// <summary>
        /// Status display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Experience level display
        /// </summary>
        [NotMapped]
        [Display(Name = "Experience Level")]
        public string ExperienceLevelDisplay
        {
            get
            {
                if (!ExperienceYears.HasValue) return "Not Specified";

                return ExperienceYears.Value switch
                {
                    0 => "Fresh Graduate",
                    >= 1 and <= 2 => "Junior Teacher",
                    >= 3 and <= 5 => "Experienced Teacher",
                    >= 6 and <= 10 => "Senior Teacher",
                    > 10 => "Expert Teacher",
                    _ => "Unknown"
                };
            }
        }

        /// <summary>
        /// Trial availability display
        /// </summary>
        [NotMapped]
        [Display(Name = "Trial Status")]
        public string TrialAvailabilityDisplay => IsAvailableForTrial ? "Available for Trial" : "Not Available for Trial";

        /// <summary>
        /// Count of active students assigned to this teacher
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Students")]
        public int ActiveStudentsCount => AssignedStudents?.Count(s => s.IsActive && s.Status == "Active") ?? 0;

        /// <summary>
        /// Count of trial students assigned to this teacher
        /// </summary>
        [NotMapped]
        [Display(Name = "Trial Students")]
        public int TrialStudentsCount => AssignedStudents?.Count(s => s.IsActive && s.Status == "Trial") ?? 0;

        /// <summary>
        /// Total students under this teacher
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Students")]
        public int TotalStudentsCount => AssignedStudents?.Count(s => s.IsActive) ?? 0;

        /// <summary>
        /// Current month's teaching hours
        /// </summary>
        [NotMapped]
        [Display(Name = "Current Month Hours")]
        public decimal CurrentMonthHours
        {
            get
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                return Attendances?
                    .Where(a => a.AttendanceDate.Month == currentMonth &&
                               a.AttendanceDate.Year == currentYear &&
                               a.Status == "Present" &&
                               a.ActualDuration.HasValue)
                    .Sum(a => a.ActualDuration.Value / 60m) ?? 0;
            }
        }

        /// <summary>
        /// Current month's earnings based on teaching hours
        /// </summary>
        [NotMapped]
        [Display(Name = "Current Month Earnings")]
        public decimal CurrentMonthEarnings => CurrentMonthHours * (HourlyRate ?? 0);

        /// <summary>
        /// Average weekly teaching hours
        /// </summary>
        [NotMapped]
        [Display(Name = "Average Weekly Hours")]
        public decimal AverageWeeklyHours
        {
            get
            {
                var last30Days = DateTime.Now.AddDays(-30);
                var totalHours = Attendances?
                    .Where(a => a.AttendanceDate >= last30Days &&
                               a.Status == "Present" &&
                               a.ActualDuration.HasValue)
                    .Sum(a => a.ActualDuration.Value / 60m) ?? 0;

                return totalHours / 4; // Approximate weeks in 30 days
            }
        }

        /// <summary>
        /// Teacher utilization rate (percentage of capacity used)
        /// </summary>
        [NotMapped]
        [Display(Name = "Utilization Rate")]
        public double UtilizationRate
        {
            get
            {
                if (MaxStudentsPerDay <= 0) return 0;
                return (double)ActiveStudentsCount / MaxStudentsPerDay * 100;
            }
        }

        /// <summary>
        /// Check if teacher is at full capacity
        /// </summary>
        [NotMapped]
        [Display(Name = "Is At Capacity")]
        public bool IsAtCapacity => ActiveStudentsCount >= MaxStudentsPerDay;

        /// <summary>
        /// Available slots for new students
        /// </summary>
        [NotMapped]
        [Display(Name = "Available Slots")]
        public int AvailableSlots => Math.Max(0, MaxStudentsPerDay - ActiveStudentsCount);

        /// <summary>
        /// Count of scheduled classes this week
        /// </summary>
        [NotMapped]
        [Display(Name = "This Week Classes")]
        public int ThisWeekClassesCount
        {
            get
            {
                var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(6);

                return ClassSchedules?.Count(cs =>
                    cs.ScheduleDate >= startOfWeek &&
                    cs.ScheduleDate <= endOfWeek &&
                    cs.Status != "Cancelled") ?? 0;
            }
        }

        /// <summary>
        /// Count of completed classes this month
        /// </summary>
        [NotMapped]
        [Display(Name = "This Month Completed Classes")]
        public int ThisMonthCompletedClassesCount
        {
            get
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                return ClassSchedules?.Count(cs =>
                    cs.ScheduleDate.Month == currentMonth &&
                    cs.ScheduleDate.Year == currentYear &&
                    cs.Status == "Completed") ?? 0;
            }
        }

        /// <summary>
        /// Teacher's performance rating based on attendance and completion
        /// </summary>
        [NotMapped]
        [Display(Name = "Performance Rating")]
        public string PerformanceRating
        {
            get
            {
                var last30DaysAttendances = Attendances?
                    .Where(a => a.AttendanceDate >= DateTime.Now.AddDays(-30))
                    .ToList();

                if (last30DaysAttendances?.Any() != true) return "No Data";

                var completionRate = (double)last30DaysAttendances.Count(a => a.Status == "Present") /
                                   last30DaysAttendances.Count * 100;

                return completionRate switch
                {
                    >= 95 => "Excellent",
                    >= 85 => "Very Good",
                    >= 75 => "Good",
                    >= 60 => "Fair",
                    _ => "Needs Improvement"
                };
            }
        }

        /// <summary>
        /// Next scheduled class date
        /// </summary>
        [NotMapped]
        [Display(Name = "Next Class")]
        public DateTime? NextClassDate
        {
            get
            {
                return ClassSchedules?
                    .Where(cs => cs.ScheduleDate >= DateTime.Today && cs.Status == "Scheduled")
                    .OrderBy(cs => cs.ScheduleDate)
                    .ThenBy(cs => cs.StartTime)
                    .FirstOrDefault()?.ScheduleDate;
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Check if teacher code is unique within the same site
        /// </summary>
        /// <param name="otherTeachers">Other teachers to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsTeacherCodeUnique(IEnumerable<Teacher> otherTeachers)
        {
            return !otherTeachers.Any(t =>
                t.TeacherId != TeacherId &&
                t.TeacherCode.ToUpper() == TeacherCode.ToUpper() &&
                t.SiteId == SiteId);
        }

        /// <summary>
        /// Check if teacher is available on a specific date and time
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <returns>True if available</returns>
        public bool IsAvailableAt(DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            if (!IsActive) return false;

            var dayOfWeek = (int)date.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // Sunday = 7 in our system

            // Check if teacher has working schedule for this day and time
            var hasSchedule = TeacherSchedules?.Any(ts =>
                ts.IsActive &&
                ts.DayOfWeek == dayOfWeek &&
                ts.StartTime <= startTime &&
                ts.EndTime >= endTime) ?? false;

            if (!hasSchedule) return false;

            // Check if teacher doesn't have conflicting class
            var hasConflict = ClassSchedules?.Any(cs =>
                cs.ScheduleDate.Date == date.Date &&
                cs.StartTime < endTime &&
                cs.EndTime > startTime &&
                cs.Status != "Cancelled") ?? false;

            return !hasConflict;
        }

        /// <summary>
        /// Get teacher's available time slots for a specific date
        /// </summary>
        /// <param name="date">Date to get available slots</param>
        /// <returns>List of available time slots</returns>
        public List<(TimeSpan StartTime, TimeSpan EndTime)> GetAvailableTimeSlots(DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;

            var workingSchedules = TeacherSchedules?
                .Where(ts => ts.IsActive && ts.DayOfWeek == dayOfWeek)
                .OrderBy(ts => ts.StartTime)
                .ToList() ?? new List<TeacherSchedule>();

            var busySlots = (ClassSchedules ?? Enumerable.Empty<ClassSchedule>())
                            .Where(cs => cs.ScheduleDate.Date == date.Date && cs.Status != "Cancelled")
                            .Select(cs => (cs.StartTime, cs.EndTime))
                            .OrderBy(cs => cs.StartTime)
                            .ToList();

            var availableSlots = new List<(TimeSpan, TimeSpan)>();

            foreach (var schedule in workingSchedules)
            {
                var currentTime = schedule.StartTime;
                var endTime = schedule.EndTime;

                while (currentTime.Add(TimeSpan.FromHours(1)) <= endTime)
                {
                    var slotEndTime = currentTime.Add(TimeSpan.FromHours(1));

                    // Check if this slot conflicts with existing classes
                    var hasConflict = busySlots.Any(busy =>
                        currentTime < busy.EndTime && slotEndTime > busy.StartTime);

                    if (!hasConflict)
                    {
                        availableSlots.Add((currentTime, slotEndTime));
                    }

                    currentTime = slotEndTime;
                }
            }

            return availableSlots;
        }

        /// <summary>
        /// Check if teacher can take more students
        /// </summary>
        /// <returns>True if teacher can accommodate more students</returns>
        public bool CanTakeMoreStudents()
        {
            return IsActive && ActiveStudentsCount < MaxStudentsPerDay;
        }

        /// <summary>
        /// Get students by status assigned to this teacher
        /// </summary>
        /// <param name="status">Student status to filter by</param>
        /// <returns>Students with the specified status</returns>
        public IEnumerable<Student> GetStudentsByStatus(string status)
        {
            return AssignedStudents?.Where(s => s.IsActive && s.Status == status) ?? Enumerable.Empty<Student>();
        }

        /// <summary>
        /// Get active students assigned to this teacher
        /// </summary>
        /// <returns>Active students</returns>
        public IEnumerable<Student> GetActiveStudents()
        {
            return GetStudentsByStatus("Active");
        }

        /// <summary>
        /// Get trial students assigned to this teacher
        /// </summary>
        /// <returns>Trial students</returns>
        public IEnumerable<Student> GetTrialStudents()
        {
            return GetStudentsByStatus("Trial");
        }

        /// <summary>
        /// Calculate total teaching hours for a specific period
        /// </summary>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Total teaching hours</returns>
        public decimal CalculateTeachingHours(DateTime startDate, DateTime endDate)
        {
            return Attendances?
                .Where(a => a.AttendanceDate >= startDate &&
                           a.AttendanceDate <= endDate &&
                           a.Status == "Present" &&
                           a.ActualDuration.HasValue)
                .Sum(a => a.ActualDuration.Value / 60m) ?? 0;
        }

        /// <summary>
        /// Calculate earnings for a specific period
        /// </summary>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Total earnings</returns>
        public decimal CalculateEarnings(DateTime startDate, DateTime endDate)
        {
            var hours = CalculateTeachingHours(startDate, endDate);
            return hours * (HourlyRate ?? 0);
        }

        /// <summary>
        /// Get upcoming classes for this teacher
        /// </summary>
        /// <param name="days">Number of days to look ahead</param>
        /// <returns>Upcoming classes</returns>
        public IEnumerable<ClassSchedule> GetUpcomingClasses(int days = 7)
        {
            var endDate = DateTime.Today.AddDays(days);
            return ClassSchedules?
                .Where(cs => cs.ScheduleDate >= DateTime.Today &&
                           cs.ScheduleDate <= endDate &&
                           cs.Status == "Scheduled")
                .OrderBy(cs => cs.ScheduleDate)
                .ThenBy(cs => cs.StartTime) ?? Enumerable.Empty<ClassSchedule>();
        }

        /// <summary>
        /// Get teacher's performance statistics
        /// </summary>
        /// <param name="startDate">Analysis start date</param>
        /// <param name="endDate">Analysis end date</param>
        /// <returns>Performance statistics</returns>
        public Dictionary<string, object> GetPerformanceStatistics(DateTime startDate, DateTime endDate)
        {
            var periodAttendances = Attendances?
                .Where(a => a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
                .ToList() ?? new List<Attendance>();

            var totalClasses = periodAttendances.Count;
            var completedClasses = periodAttendances.Count(a => a.Status == "Present");
            var totalHours = periodAttendances
                .Where(a => a.Status == "Present" && a.ActualDuration.HasValue)
                .Sum(a => a.ActualDuration.Value / 60m);

            return new Dictionary<string, object>
            {
                {"TotalClasses", totalClasses},
                {"CompletedClasses", completedClasses},
                {"CompletionRate", totalClasses > 0 ? (double)completedClasses / totalClasses * 100 : 0},
                {"TotalHours", totalHours},
                {"AverageHoursPerClass", completedClasses > 0 ? totalHours / completedClasses : 0},
                {"TotalEarnings", totalHours * (HourlyRate ?? 0)},
                {"ActiveStudents", ActiveStudentsCount},
                {"TrialStudents", TrialStudentsCount},
                {"UtilizationRate", UtilizationRate},
                {"AvailableSlots", AvailableSlots}
            };
        }

        /// <summary>
        /// Generate teacher performance report
        /// </summary>
        /// <param name="startDate">Report start date</param>
        /// <param name="endDate">Report end date</param>
        /// <returns>Performance report data</returns>
        public Dictionary<string, object> GeneratePerformanceReport(DateTime startDate, DateTime endDate)
        {
            var stats = GetPerformanceStatistics(startDate, endDate);
            var workingDays = GetWorkingDaysCount(startDate, endDate);

            return new Dictionary<string, object>
            {
                {"Teacher", new { TeacherId, TeacherCode, TeacherName, Site = Site?.SiteName }},
                {"Period", new { StartDate = startDate, EndDate = endDate, WorkingDays = workingDays }},
                {"Statistics", stats},
                {"Students", new
                {
                    Active = GetActiveStudents().Select(s => new { s.StudentId, s.StudentCode, s.FullName }),
                    Trial = GetTrialStudents().Select(s => new { s.StudentId, s.StudentCode, s.FullName })
                }},
                {"Schedule", new
                {
                    WeeklySchedule = TeacherSchedules?.Where(ts => ts.IsActive).OrderBy(ts => ts.DayOfWeek),
                    Utilization = UtilizationRate,
                    Availability = IsAvailableForTrial
                }}
            };
        }

        /// <summary>
        /// Get working days count for a period based on teacher schedule
        /// </summary>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Number of working days</returns>
        private int GetWorkingDaysCount(DateTime startDate, DateTime endDate)
        {
            var workingDays = TeacherSchedules?
                .Where(ts => ts.IsActive)
                .Select(ts => ts.DayOfWeek)
                .Distinct()
                .ToList() ?? new List<int>();

            if (!workingDays.Any()) return 0;

            var count = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dayOfWeek = (int)date.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7;

                if (workingDays.Contains(dayOfWeek))
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Validate teacher business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateTeacherRules()
        {
            var errors = new List<string>();

            // User must be assigned to the same site
            if (User?.SiteId != SiteId)
            {
                errors.Add("Teacher's user account must be assigned to the same site");
            }

            // User must have TEACHER user level
            if (User?.UserLevel?.LevelCode != "TEACHER")
            {
                errors.Add("User must have TEACHER user level");
            }

            // Teacher must have at least one working schedule if active
            if (IsActive && (!TeacherSchedules?.Any(ts => ts.IsActive) ?? true))
            {
                errors.Add("Active teacher must have at least one working schedule");
            }

            // Hourly rate should be set for payroll calculation
            if (IsActive && !HourlyRate.HasValue)
            {
                errors.Add("Active teacher should have hourly rate set for payroll calculation");
            }

            // Cannot exceed site capacity
            if (ActiveStudentsCount > MaxStudentsPerDay)
            {
                errors.Add($"Teacher has {ActiveStudentsCount} students but maximum capacity is {MaxStudentsPerDay}");
            }

            return errors;
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the teacher</returns>
        public override string ToString() => DisplayName;
    }
}
