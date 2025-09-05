using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a class schedule in the KMSI Course Management System
    /// Manages individual and recurring class appointments between teachers and students
    /// </summary>
    [Table("ClassSchedules")]
    public class ClassSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClassScheduleId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Teacher is required")]
        [Display(Name = "Teacher")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        [Display(Name = "Grade")]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Schedule date is required")]
        [Display(Name = "Schedule Date")]
        [DataType(DataType.Date)]
        public DateTime ScheduleDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Start time is required")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(15, 480, ErrorMessage = "Duration must be between 15 and 480 minutes")]
        [Display(Name = "Duration (Minutes)")]
        public int Duration { get; set; } = 60;

        [Required(ErrorMessage = "Schedule type is required")]
        [StringLength(20, ErrorMessage = "Schedule type cannot exceed 20 characters")]
        [Display(Name = "Schedule Type")]
        public string ScheduleType { get; set; } = "Regular"; // Regular, Trial, Makeup, Examination

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, No Show

        [StringLength(50, ErrorMessage = "Room cannot exceed 50 characters")]
        [Display(Name = "Room")]
        public string? Room { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Display(Name = "Is Recurring")]
        public bool IsRecurring { get; set; } = false;

        [StringLength(100, ErrorMessage = "Recurrence pattern cannot exceed 100 characters")]
        [Display(Name = "Recurrence Pattern")]
        public string? RecurrencePattern { get; set; } // Weekly, Biweekly, etc

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
        /// Company that owns this schedule
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where the class takes place
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Student attending the class
        /// </summary>
        [Required]
        [Display(Name = "Student")]
        public virtual Student Student { get; set; } = null!;

        /// <summary>
        /// Teacher conducting the class
        /// </summary>
        [Required]
        [Display(Name = "Teacher")]
        public virtual Teacher Teacher { get; set; } = null!;

        /// <summary>
        /// Grade level being taught
        /// </summary>
        [Required]
        [Display(Name = "Grade")]
        public virtual Grade Grade { get; set; } = null!;

        /// <summary>
        /// User who created this schedule
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this schedule
        /// </summary>
        [Display(Name = "Updated By")]
        public virtual User? UpdatedByUser { get; set; }

        /// <summary>
        /// Attendance records for this scheduled class
        /// </summary>
        [Display(Name = "Attendances")]
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{Student?.FullName} - {Teacher?.User?.FullName} ({ScheduleDate:dd/MM/yyyy} {StartTime:hh\\:mm})";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplayName => $"{Student?.FirstName} - {ScheduleDate:dd/MM} {StartTime:hh\\:mm}";

        /// <summary>
        /// Schedule type display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Schedule Type Display")]
        public string ScheduleTypeDisplay => ScheduleType switch
        {
            "Regular" => "📚 Regular Class",
            "Trial" => "🎵 Trial Class",
            "Makeup" => "⏰ Makeup Class",
            "Examination" => "📝 Examination",
            _ => $"📅 {ScheduleType}"
        };

        /// <summary>
        /// Status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => Status switch
        {
            "Scheduled" => "📅 Scheduled",
            "Completed" => "✅ Completed",
            "Cancelled" => "❌ Cancelled",
            "No Show" => "👻 No Show",
            _ => Status
        };

        /// <summary>
        /// Formatted schedule time display
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Display")]
        public string TimeDisplay => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

        /// <summary>
        /// Full schedule display with date and time
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Schedule Display")]
        public string FullScheduleDisplay => $"{ScheduleDate:dddd, dd MMMM yyyy} at {TimeDisplay}";

        /// <summary>
        /// Day of week for the scheduled date
        /// </summary>
        [NotMapped]
        [Display(Name = "Day of Week")]
        public DayOfWeek DayOfWeek => ScheduleDate.DayOfWeek;

        /// <summary>
        /// Day of week display in Indonesian
        /// </summary>
        [NotMapped]
        [Display(Name = "Day Name")]
        public string DayName => DayOfWeek switch
        {
            DayOfWeek.Sunday => "Minggu",
            DayOfWeek.Monday => "Senin",
            DayOfWeek.Tuesday => "Selasa",
            DayOfWeek.Wednesday => "Rabu",
            DayOfWeek.Thursday => "Kamis",
            DayOfWeek.Friday => "Jumat",
            DayOfWeek.Saturday => "Sabtu",
            _ => DayOfWeek.ToString()
        };

        /// <summary>
        /// Indicates if schedule is today
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Today")]
        public bool IsToday => ScheduleDate.Date == DateTime.Today;

        /// <summary>
        /// Indicates if schedule is in the past
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Past")]
        public bool IsPast => ScheduleDate.Date < DateTime.Today;

        /// <summary>
        /// Indicates if schedule is in the future
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Future")]
        public bool IsFuture => ScheduleDate.Date > DateTime.Today;

        /// <summary>
        /// Days until scheduled class
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Class")]
        public int DaysUntilClass => (ScheduleDate.Date - DateTime.Today).Days;

        /// <summary>
        /// Schedule urgency indicator
        /// </summary>
        [NotMapped]
        [Display(Name = "Urgency")]
        public string Urgency
        {
            get
            {
                if (IsPast && Status == "Scheduled") return "Overdue";
                if (IsToday) return "Today";

                var days = DaysUntilClass;
                return days switch
                {
                    1 => "Tomorrow",
                    <= 7 => "This Week",
                    <= 30 => "This Month",
                    _ => "Future"
                };
            }
        }

        /// <summary>
        /// Indicates if class is active (scheduled or in progress)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Active")]
        public bool IsActive => Status == "Scheduled";

        /// <summary>
        /// Indicates if class is completed
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Completed")]
        public bool IsCompleted => Status == "Completed";

        /// <summary>
        /// Indicates if class was cancelled
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Cancelled")]
        public bool IsCancelled => Status == "Cancelled";

        /// <summary>
        /// Indicates if student didn't show up
        /// </summary>
        [NotMapped]
        [Display(Name = "Is No Show")]
        public bool IsNoShow => Status == "No Show";

        /// <summary>
        /// Time until class starts (if today)
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Until Class")]
        public TimeSpan? TimeUntilClass
        {
            get
            {
                if (!IsToday) return null;
                var classDateTime = ScheduleDate.Date + StartTime;
                var now = DateTime.Now;
                return classDateTime > now ? classDateTime - now : null;
            }
        }

        /// <summary>
        /// Class priority based on type and urgency
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority")]
        public string Priority
        {
            get
            {
                if (ScheduleType == "Examination") return "High";
                if (ScheduleType == "Trial") return "High";
                if (IsToday || Urgency == "Tomorrow") return "Medium";
                return "Normal";
            }
        }

        /// <summary>
        /// Room display with fallback
        /// </summary>
        [NotMapped]
        [Display(Name = "Room Display")]
        public string RoomDisplay => !string.IsNullOrEmpty(Room) ? Room : "Room TBD";

        /// <summary>
        /// Has attendance record
        /// </summary>
        [NotMapped]
        [Display(Name = "Has Attendance")]
        public bool HasAttendance => Attendances?.Any() ?? false;

        /// <summary>
        /// Latest attendance status
        /// </summary>
        [NotMapped]
        [Display(Name = "Attendance Status")]
        public string? AttendanceStatus => Attendances?.OrderByDescending(a => a.CreatedDate).FirstOrDefault()?.Status;

        // Static Constants for Schedule Types and Statuses
        public static class ScheduleTypes
        {
            public const string Regular = "Regular";
            public const string Trial = "Trial";
            public const string Makeup = "Makeup";
            public const string Examination = "Examination";
        }

        public static class ScheduleStatuses
        {
            public const string Scheduled = "Scheduled";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
            public const string NoShow = "No Show";
        }

        public static class RecurrencePatterns
        {
            public const string Weekly = "Weekly";
            public const string Biweekly = "Biweekly";
            public const string Monthly = "Monthly";
        }

        // Business Logic Methods

        /// <summary>
        /// Validate class schedule business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateScheduleRules()
        {
            var errors = new List<string>();

            // Time validation
            if (StartTime >= EndTime)
            {
                errors.Add("End time must be after start time");
            }

            // Duration validation
            var calculatedDuration = (int)(EndTime - StartTime).TotalMinutes;
            if (Math.Abs(Duration - calculatedDuration) > 1) // Allow 1 minute tolerance
            {
                errors.Add($"Duration ({Duration} min) doesn't match time difference ({calculatedDuration} min)");
            }

            // Date validation
            if (ScheduleDate.Date < DateTime.Today && Status == "Scheduled")
            {
                errors.Add("Cannot schedule classes in the past");
            }

            // Business hours validation (8 AM to 10 PM)
            if (StartTime < TimeSpan.FromHours(8) || EndTime > TimeSpan.FromHours(22))
            {
                errors.Add("Classes must be scheduled between 8:00 AM and 10:00 PM");
            }

            // Trial class duration validation
            if (ScheduleType == ScheduleTypes.Trial && Duration > 45)
            {
                errors.Add("Trial classes should not exceed 45 minutes");
            }

            // Regular class minimum duration
            if (ScheduleType == ScheduleTypes.Regular && Duration < 30)
            {
                errors.Add("Regular classes should be at least 30 minutes");
            }

            // Recurrence validation
            if (IsRecurring && string.IsNullOrEmpty(RecurrencePattern))
            {
                errors.Add("Recurring schedules must have a recurrence pattern");
            }

            return errors;
        }

        /// <summary>
        /// Check if teacher is available at scheduled time
        /// </summary>
        /// <param name="existingSchedules">Other schedules for the teacher</param>
        /// <returns>True if teacher is available</returns>
        public bool IsTeacherAvailable(IEnumerable<ClassSchedule> existingSchedules)
        {
            var conflictingSchedules = existingSchedules
                .Where(s => s.ClassScheduleId != ClassScheduleId)
                .Where(s => s.ScheduleDate.Date == ScheduleDate.Date)
                .Where(s => s.Status == ScheduleStatuses.Scheduled)
                .Where(s => s.StartTime < EndTime && s.EndTime > StartTime);

            return !conflictingSchedules.Any();
        }

        /// <summary>
        /// Check if student is available at scheduled time
        /// </summary>
        /// <param name="existingSchedules">Other schedules for the student</param>
        /// <returns>True if student is available</returns>
        public bool IsStudentAvailable(IEnumerable<ClassSchedule> existingSchedules)
        {
            var conflictingSchedules = existingSchedules
                .Where(s => s.ClassScheduleId != ClassScheduleId)
                .Where(s => s.ScheduleDate.Date == ScheduleDate.Date)
                .Where(s => s.Status == ScheduleStatuses.Scheduled)
                .Where(s => s.StartTime < EndTime && s.EndTime > StartTime);

            return !conflictingSchedules.Any();
        }

        /// <summary>
        /// Update schedule status with validation
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
        /// Get valid status transitions based on current status and date
        /// </summary>
        /// <returns>List of valid next statuses</returns>
        public List<string> GetValidStatusTransitions()
        {
            var validStatuses = new List<string>();

            switch (Status)
            {
                case ScheduleStatuses.Scheduled:
                    validStatuses.AddRange(new[] {
                        ScheduleStatuses.Completed,
                        ScheduleStatuses.Cancelled,
                        ScheduleStatuses.NoShow
                    });
                    break;
                case ScheduleStatuses.Cancelled:
                    if (IsFuture) // Can reschedule if in future
                        validStatuses.Add(ScheduleStatuses.Scheduled);
                    break;
                case ScheduleStatuses.NoShow:
                    validStatuses.AddRange(new[] {
                        ScheduleStatuses.Completed,
                        ScheduleStatuses.Cancelled
                    });
                    break;
                    // Completed is terminal status
            }

            return validStatuses;
        }

        /// <summary>
        /// Generate recurring schedules based on pattern
        /// </summary>
        /// <param name="numberOfOccurrences">Number of recurring schedules to generate</param>
        /// <param name="createdBy">User creating the schedules</param>
        /// <returns>List of new recurring schedules</returns>
        public List<ClassSchedule> GenerateRecurringSchedules(int numberOfOccurrences, int? createdBy = null)
        {
            if (!IsRecurring || string.IsNullOrEmpty(RecurrencePattern))
                return new List<ClassSchedule>();

            var recurringSchedules = new List<ClassSchedule>();
            var currentDate = ScheduleDate;

            for (int i = 1; i <= numberOfOccurrences; i++)
            {
                currentDate = RecurrencePattern switch
                {
                    RecurrencePatterns.Weekly => currentDate.AddDays(7),
                    RecurrencePatterns.Biweekly => currentDate.AddDays(14),
                    RecurrencePatterns.Monthly => currentDate.AddMonths(1),
                    _ => currentDate.AddDays(7) // Default to weekly
                };

                var recurringSchedule = new ClassSchedule
                {
                    CompanyId = CompanyId,
                    SiteId = SiteId,
                    StudentId = StudentId,
                    TeacherId = TeacherId,
                    GradeId = GradeId,
                    ScheduleDate = currentDate,
                    StartTime = StartTime,
                    EndTime = EndTime,
                    Duration = Duration,
                    ScheduleType = ScheduleType,
                    Status = ScheduleStatuses.Scheduled,
                    Room = Room,
                    Notes = Notes,
                    IsRecurring = true,
                    RecurrencePattern = RecurrencePattern,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Now
                };

                recurringSchedules.Add(recurringSchedule);
            }

            return recurringSchedules;
        }

        /// <summary>
        /// Calculate schedule statistics
        /// </summary>
        /// <returns>Dictionary with schedule statistics</returns>
        public Dictionary<string, object> GetScheduleStatistics()
        {
            return new Dictionary<string, object>
            {
                {"ClassScheduleId", ClassScheduleId},
                {"StudentName", Student?.FullName},
                {"TeacherName", Teacher?.User?.FullName},
                {"GradeName", Grade?.GradeName},
                {"ScheduleDate", ScheduleDate},
                {"TimeDisplay", TimeDisplay},
                {"Duration", Duration},
                {"ScheduleType", ScheduleType},
                {"Status", Status},
                {"Room", RoomDisplay},
                {"IsToday", IsToday},
                {"IsPast", IsPast},
                {"IsFuture", IsFuture},
                {"DaysUntilClass", DaysUntilClass},
                {"Urgency", Urgency},
                {"Priority", Priority},
                {"HasAttendance", HasAttendance},
                {"AttendanceStatus", AttendanceStatus},
                {"IsRecurring", IsRecurring},
                {"RecurrencePattern", RecurrencePattern}
            };
        }

        /// <summary>
        /// Create a new class schedule
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="studentId">Student ID</param>
        /// <param name="teacherId">Teacher ID</param>
        /// <param name="gradeId">Grade ID</param>
        /// <param name="scheduleDate">Schedule date</param>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <param name="scheduleType">Schedule type</param>
        /// <param name="room">Room (optional)</param>
        /// <param name="createdBy">Creator user ID</param>
        /// <returns>New class schedule instance</returns>
        public static ClassSchedule CreateSchedule(int companyId, int siteId, int studentId, int teacherId,
            int gradeId, DateTime scheduleDate, TimeSpan startTime, TimeSpan endTime,
            string scheduleType = ScheduleTypes.Regular, string? room = null, int? createdBy = null)
        {
            var duration = (int)(endTime - startTime).TotalMinutes;

            return new ClassSchedule
            {
                CompanyId = companyId,
                SiteId = siteId,
                StudentId = studentId,
                TeacherId = teacherId,
                GradeId = gradeId,
                ScheduleDate = scheduleDate,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration,
                ScheduleType = scheduleType,
                Status = ScheduleStatuses.Scheduled,
                Room = room,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get schedules by date range
        /// </summary>
        /// <param name="schedules">Collection of schedules</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Filtered schedules</returns>
        public static IEnumerable<ClassSchedule> GetByDateRange(IEnumerable<ClassSchedule> schedules,
            DateTime startDate, DateTime endDate)
        {
            return schedules.Where(s => s.ScheduleDate.Date >= startDate.Date &&
                                       s.ScheduleDate.Date <= endDate.Date);
        }

        /// <summary>
        /// Get today's schedules
        /// </summary>
        /// <param name="schedules">Collection of schedules</param>
        /// <returns>Today's schedules</returns>
        public static IEnumerable<ClassSchedule> GetTodaySchedules(IEnumerable<ClassSchedule> schedules)
        {
            return schedules.Where(s => s.IsToday);
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the class schedule</returns>
        public override string ToString() => DisplayName;
    }
}
