using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents attendance records for class sessions
    /// Captures detailed information about lesson delivery, student progress, and teacher notes
    /// Includes performance tracking and homework assignments
    /// </summary>
    [Table("Attendances")]
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceId { get; set; }

        [Required(ErrorMessage = "Class schedule is required")]
        [Display(Name = "Class Schedule")]
        [ForeignKey("ClassSchedule")]
        public int ClassScheduleId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Teacher is required")]
        [Display(Name = "Teacher")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Attendance date is required")]
        [Display(Name = "Attendance Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AttendanceDate { get; set; }

        [Display(Name = "Actual Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? ActualStartTime { get; set; }

        [Display(Name = "Actual End Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? ActualEndTime { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty; // Present, Absent, Late, Excused

        [StringLength(255, ErrorMessage = "Lesson topic cannot exceed 255 characters")]
        [Display(Name = "Lesson Topic")]
        public string? LessonTopic { get; set; }

        [StringLength(500, ErrorMessage = "Student progress cannot exceed 500 characters")]
        [Display(Name = "Student Progress")]
        [DataType(DataType.MultilineText)]
        public string? StudentProgress { get; set; }

        [StringLength(1000, ErrorMessage = "Teacher notes cannot exceed 1000 characters")]
        [Display(Name = "Teacher Notes")]
        [DataType(DataType.MultilineText)]
        public string? TeacherNotes { get; set; }

        [StringLength(500, ErrorMessage = "Homework assigned cannot exceed 500 characters")]
        [Display(Name = "Homework Assigned")]
        [DataType(DataType.MultilineText)]
        public string? HomeworkAssigned { get; set; }

        [StringLength(500, ErrorMessage = "Next lesson prep cannot exceed 500 characters")]
        [Display(Name = "Next Lesson Preparation")]
        [DataType(DataType.MultilineText)]
        public string? NextLessonPrep { get; set; }

        [Range(1, 10, ErrorMessage = "Student performance score must be between 1 and 10")]
        [Display(Name = "Performance Score")]
        public byte? StudentPerformanceScore { get; set; }

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
        /// The class schedule this attendance record relates to
        /// </summary>
        [Required]
        [Display(Name = "Class Schedule")]
        public virtual ClassSchedule ClassSchedule { get; set; } = null!;

        /// <summary>
        /// The student who attended (or didn't attend) the class
        /// </summary>
        [Required]
        [Display(Name = "Student")]
        public virtual Student Student { get; set; } = null!;

        /// <summary>
        /// The teacher who conducted the class
        /// </summary>
        [Required]
        [Display(Name = "Teacher")]
        public virtual Teacher Teacher { get; set; } = null!;

        /// <summary>
        /// User who created this attendance record
        /// </summary>
        [Display(Name = "Created By User")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this attendance record
        /// </summary>
        [Display(Name = "Updated By User")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped - calculated properties)

        /// <summary>
        /// Actual duration in minutes (computed column in database)
        /// </summary>
        [NotMapped]
        [Display(Name = "Actual Duration (Minutes)")]
        public int? ActualDuration
        {
            get
            {
                if (ActualStartTime.HasValue && ActualEndTime.HasValue)
                {
                    return (int)(ActualEndTime.Value - ActualStartTime.Value).TotalMinutes;
                }
                return null;
            }
        }

        /// <summary>
        /// Formatted actual duration as hours and minutes
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration")]
        public string FormattedDuration
        {
            get
            {
                if (!ActualDuration.HasValue) return "Not recorded";

                var minutes = ActualDuration.Value;
                if (minutes < 60)
                    return $"{minutes} min";

                var hours = minutes / 60;
                var remainingMinutes = minutes % 60;
                return remainingMinutes > 0 ? $"{hours}h {remainingMinutes}m" : $"{hours}h";
            }
        }

        /// <summary>
        /// Check if student was present
        /// </summary>
        [NotMapped]
        [Display(Name = "Was Present")]
        public bool WasPresent => Status.Equals("Present", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Check if student was late
        /// </summary>
        [NotMapped]
        [Display(Name = "Was Late")]
        public bool WasLate => Status.Equals("Late", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Check if absence was excused
        /// </summary>
        [NotMapped]
        [Display(Name = "Excused Absence")]
        public bool IsExcusedAbsence => Status.Equals("Excused", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Check if student was absent without excuse
        /// </summary>
        [NotMapped]
        [Display(Name = "Unexcused Absence")]
        public bool IsUnexcusedAbsence => Status.Equals("Absent", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Performance grade based on score
        /// </summary>
        [NotMapped]
        [Display(Name = "Performance Grade")]
        public string PerformanceGrade
        {
            get
            {
                if (!StudentPerformanceScore.HasValue) return "Not Graded";

                return StudentPerformanceScore.Value switch
                {
                    >= 9 => "Excellent",
                    >= 8 => "Very Good",
                    >= 7 => "Good",
                    >= 6 => "Satisfactory",
                    >= 5 => "Needs Improvement",
                    _ => "Poor"
                };
            }
        }

        /// <summary>
        /// Color code for performance display
        /// </summary>
        [NotMapped]
        [Display(Name = "Performance Color")]
        public string PerformanceColor
        {
            get
            {
                if (!StudentPerformanceScore.HasValue) return "secondary";

                return StudentPerformanceScore.Value switch
                {
                    >= 8 => "success",     // Green
                    >= 6 => "warning",     // Yellow
                    >= 4 => "info",        // Blue
                    _ => "danger"          // Red
                };
            }
        }

        /// <summary>
        /// Status color for UI display
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Color")]
        public string StatusColor
        {
            get
            {
                return Status.ToUpper() switch
                {
                    "PRESENT" => "success",   // Green
                    "LATE" => "warning",      // Orange
                    "EXCUSED" => "info",      // Blue
                    "ABSENT" => "danger",     // Red
                    _ => "secondary"          // Gray
                };
            }
        }

        /// <summary>
        /// Status icon for UI display
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Icon")]
        public string StatusIcon
        {
            get
            {
                return Status.ToUpper() switch
                {
                    "PRESENT" => "fas fa-check-circle",
                    "LATE" => "fas fa-clock",
                    "EXCUSED" => "fas fa-info-circle",
                    "ABSENT" => "fas fa-times-circle",
                    _ => "fas fa-question-circle"
                };
            }
        }

        /// <summary>
        /// Check if the class was conducted (has actual times)
        /// </summary>
        [NotMapped]
        [Display(Name = "Class Conducted")]
        public bool WasClassConducted => ActualStartTime.HasValue && ActualEndTime.HasValue;

        /// <summary>
        /// Check if homework was assigned
        /// </summary>
        [NotMapped]
        [Display(Name = "Homework Assigned")]
        public bool HasHomeworkAssigned => !string.IsNullOrWhiteSpace(HomeworkAssigned);

        /// <summary>
        /// Check if next lesson preparation was noted
        /// </summary>
        [NotMapped]
        [Display(Name = "Next Lesson Planned")]
        public bool HasNextLessonPrep => !string.IsNullOrWhiteSpace(NextLessonPrep);

        /// <summary>
        /// Comprehensive lesson completion percentage
        /// </summary>
        [NotMapped]
        [Display(Name = "Lesson Completion")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal LessonCompletionPercentage
        {
            get
            {
                int completedItems = 0;
                int totalItems = 5; // Total checkpoints

                if (WasClassConducted) completedItems++;
                if (!string.IsNullOrWhiteSpace(LessonTopic)) completedItems++;
                if (!string.IsNullOrWhiteSpace(StudentProgress)) completedItems++;
                if (!string.IsNullOrWhiteSpace(TeacherNotes)) completedItems++;
                if (StudentPerformanceScore.HasValue) completedItems++;

                return totalItems > 0 ? (decimal)completedItems / totalItems : 0;
            }
        }

        /// <summary>
        /// Display name for dropdowns and lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{Student?.FullName ?? "Unknown Student"} - {AttendanceDate:MMM dd, yyyy} ({Status})";

        /// <summary>
        /// Summary description for quick overview
        /// </summary>
        [NotMapped]
        [Display(Name = "Summary")]
        public string Summary => $"{AttendanceDate:MMM dd}: {Status}" +
                               (StudentPerformanceScore.HasValue ? $" - Score: {StudentPerformanceScore}/10" : "");

        // Business Logic Methods

        /// <summary>
        /// Validate business rules for attendance record
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateBusinessRules()
        {
            var errors = new List<string>();

            // Valid status values
            var validStatuses = new[] { "Present", "Absent", "Late", "Excused" };
            if (!validStatuses.Contains(Status, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Status must be one of: {string.Join(", ", validStatuses)}");
            }

            // End time must be after start time
            if (ActualStartTime.HasValue && ActualEndTime.HasValue && ActualEndTime <= ActualStartTime)
            {
                errors.Add("End time must be after start time");
            }

            // Attendance date should not be in the future
            if (AttendanceDate.Date > DateTime.Today)
            {
                errors.Add("Attendance date cannot be in the future");
            }

            // Performance score validation
            if (StudentPerformanceScore.HasValue && (StudentPerformanceScore < 1 || StudentPerformanceScore > 10))
            {
                errors.Add("Performance score must be between 1 and 10");
            }

            // If present or late, should have actual times
            if ((Status.Equals("Present", StringComparison.OrdinalIgnoreCase) ||
                 Status.Equals("Late", StringComparison.OrdinalIgnoreCase)) &&
                (!ActualStartTime.HasValue || !ActualEndTime.HasValue))
            {
                errors.Add("Actual start and end times are required when student is present or late");
            }

            // Duration should be reasonable (between 5 minutes and 8 hours)
            if (ActualDuration.HasValue)
            {
                if (ActualDuration.Value < 5)
                {
                    errors.Add("Class duration seems too short (less than 5 minutes)");
                }
                else if (ActualDuration.Value > 480) // 8 hours
                {
                    errors.Add("Class duration seems too long (more than 8 hours)");
                }
            }

            // If performance score is given, student should be present
            if (StudentPerformanceScore.HasValue &&
                !Status.Equals("Present", StringComparison.OrdinalIgnoreCase) &&
                !Status.Equals("Late", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Performance score should only be given when student is present or late");
            }

            return errors;
        }

        /// <summary>
        /// Check if attendance record is complete
        /// </summary>
        /// <returns>True if all essential fields are filled</returns>
        public bool IsComplete()
        {
            if (Status.Equals("Absent", StringComparison.OrdinalIgnoreCase))
            {
                // For absent students, just need the status
                return true;
            }

            // For present/late students, need more details
            return WasClassConducted &&
                   !string.IsNullOrWhiteSpace(LessonTopic) &&
                   !string.IsNullOrWhiteSpace(TeacherNotes);
        }

        /// <summary>
        /// Mark student as present with actual times
        /// </summary>
        /// <param name="startTime">Actual start time</param>
        /// <param name="endTime">Actual end time</param>
        /// <param name="isLate">Whether student arrived late</param>
        public void MarkPresent(TimeSpan startTime, TimeSpan endTime, bool isLate = false)
        {
            Status = isLate ? "Late" : "Present";
            ActualStartTime = startTime;
            ActualEndTime = endTime;
        }

        /// <summary>
        /// Mark student as absent
        /// </summary>
        /// <param name="isExcused">Whether absence is excused</param>
        public void MarkAbsent(bool isExcused = false)
        {
            Status = isExcused ? "Excused" : "Absent";
            ActualStartTime = null;
            ActualEndTime = null;
            StudentPerformanceScore = null;
        }

        /// <summary>
        /// Add lesson details and assessment
        /// </summary>
        /// <param name="topic">Lesson topic covered</param>
        /// <param name="progress">Student progress notes</param>
        /// <param name="teacherNotes">Teacher observations</param>
        /// <param name="performanceScore">Performance score (1-10)</param>
        public void AddLessonDetails(string topic, string? progress = null, string? teacherNotes = null, byte? performanceScore = null)
        {
            LessonTopic = topic;
            StudentProgress = progress;
            TeacherNotes = teacherNotes;

            if (performanceScore.HasValue && performanceScore >= 1 && performanceScore <= 10)
            {
                StudentPerformanceScore = performanceScore;
            }
        }

        /// <summary>
        /// Add homework and next lesson preparation
        /// </summary>
        /// <param name="homework">Homework assigned</param>
        /// <param name="nextPrep">Next lesson preparation notes</param>
        public void AddHomeworkAndPrep(string? homework = null, string? nextPrep = null)
        {
            HomeworkAssigned = homework;
            NextLessonPrep = nextPrep;
        }

        /// <summary>
        /// Calculate attendance points for reporting
        /// </summary>
        /// <returns>Attendance points (Present=1, Late=0.5, Excused=0.25, Absent=0)</returns>
        public decimal GetAttendancePoints()
        {
            return Status.ToUpper() switch
            {
                "PRESENT" => 1.0m,
                "LATE" => 0.5m,
                "EXCUSED" => 0.25m,
                "ABSENT" => 0.0m,
                _ => 0.0m
            };
        }

        /// <summary>
        /// Get attendance rate description
        /// </summary>
        /// <returns>Descriptive text for attendance</returns>
        public string GetAttendanceDescription()
        {
            var points = GetAttendancePoints();
            return points switch
            {
                1.0m => "Full Attendance",
                >= 0.5m => "Partial Attendance",
                >= 0.25m => "Excused Absence",
                _ => "Unexcused Absence"
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the attendance record</returns>
        public override string ToString() => DisplayName;
    }
}
