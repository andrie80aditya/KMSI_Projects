using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a teacher's working schedule in the KMSI Course Management System
    /// Defines when a teacher is available for teaching throughout the week
    /// </summary>
    [Table("TeacherSchedules")]
    public class TeacherSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeacherScheduleId { get; set; }

        [Required(ErrorMessage = "Teacher is required")]
        [Display(Name = "Teacher")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Day of week is required")]
        [Range(1, 7, ErrorMessage = "Day of week must be between 1 (Monday) and 7 (Sunday)")]
        [Display(Name = "Day of Week")]
        public int DayOfWeek { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Teacher this schedule belongs to
        /// </summary>
        [Required]
        [Display(Name = "Teacher")]
        public virtual Teacher Teacher { get; set; } = null!;

        /// <summary>
        /// User who created this schedule record
        /// </summary>
        public virtual User? CreatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Day of week display name
        /// </summary>
        [NotMapped]
        [Display(Name = "Day")]
        public string DayOfWeekDisplay => DayOfWeek switch
        {
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            7 => "Sunday",
            _ => "Unknown"
        };

        /// <summary>
        /// Short day of week display (3 letters)
        /// </summary>
        [NotMapped]
        [Display(Name = "Day")]
        public string DayOfWeekShort => DayOfWeek switch
        {
            1 => "Mon",
            2 => "Tue",
            3 => "Wed",
            4 => "Thu",
            5 => "Fri",
            6 => "Sat",
            7 => "Sun",
            _ => "???"
        };

        /// <summary>
        /// Time range display
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Range")]
        public string TimeRangeDisplay => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

        /// <summary>
        /// Time range with 24-hour format
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Range (24h)")]
        public string TimeRange24HourDisplay => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

        /// <summary>
        /// Time range with 12-hour format
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Range (12h)")]
        public string TimeRange12HourDisplay
        {
            get
            {
                var start = DateTime.Today.Add(StartTime);
                var end = DateTime.Today.Add(EndTime);
                return $"{start:h:mm tt} - {end:h:mm tt}";
            }
        }

        /// <summary>
        /// Full schedule display combining day and time
        /// </summary>
        [NotMapped]
        [Display(Name = "Schedule")]
        public string FullScheduleDisplay => $"{DayOfWeekDisplay}, {TimeRangeDisplay}";

        /// <summary>
        /// Compact schedule display for lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Schedule")]
        public string CompactScheduleDisplay => $"{DayOfWeekShort} {TimeRangeDisplay}";

        /// <summary>
        /// Duration of the working period in hours
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration (Hours)")]
        [DisplayFormat(DataFormatString = "{0:F1}", ApplyFormatInEditMode = true)]
        public double DurationHours => (EndTime - StartTime).TotalHours;

        /// <summary>
        /// Duration of the working period in minutes
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;

        /// <summary>
        /// Status display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Status color for UI styling
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Color")]
        public string StatusColor => IsActive ? "success" : "secondary";

        /// <summary>
        /// Check if this is a morning schedule (before 12:00)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Morning Schedule")]
        public bool IsMorningSchedule => StartTime < new TimeSpan(12, 0, 0);

        /// <summary>
        /// Check if this is an afternoon schedule (12:00 - 18:00)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Afternoon Schedule")]
        public bool IsAfternoonSchedule => StartTime >= new TimeSpan(12, 0, 0) && StartTime < new TimeSpan(18, 0, 0);

        /// <summary>
        /// Check if this is an evening schedule (after 18:00)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Evening Schedule")]
        public bool IsEveningSchedule => StartTime >= new TimeSpan(18, 0, 0);

        /// <summary>
        /// Check if this is a weekend schedule (Saturday or Sunday)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Weekend Schedule")]
        public bool IsWeekendSchedule => DayOfWeek == 6 || DayOfWeek == 7;

        /// <summary>
        /// Check if this is a weekday schedule (Monday to Friday)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Weekday Schedule")]
        public bool IsWeekdaySchedule => DayOfWeek >= 1 && DayOfWeek <= 5;

        /// <summary>
        /// Schedule period classification
        /// </summary>
        [NotMapped]
        [Display(Name = "Schedule Period")]
        public string SchedulePeriod
        {
            get
            {
                if (IsMorningSchedule) return "Morning";
                if (IsAfternoonSchedule) return "Afternoon";
                if (IsEveningSchedule) return "Evening";
                return "Unknown";
            }
        }

        /// <summary>
        /// Schedule type classification
        /// </summary>
        [NotMapped]
        [Display(Name = "Schedule Type")]
        public string ScheduleType => IsWeekendSchedule ? "Weekend" : "Weekday";

        /// <summary>
        /// Maximum possible classes in this time slot (assuming 1-hour classes)
        /// </summary>
        [NotMapped]
        [Display(Name = "Max Classes")]
        public int MaxPossibleClasses => (int)Math.Floor(DurationHours);

        /// <summary>
        /// Next occurrence of this schedule from today
        /// </summary>
        [NotMapped]
        [Display(Name = "Next Occurrence")]
        public DateTime NextOccurrence
        {
            get
            {
                var today = DateTime.Today;
                var todayDayOfWeek = (int)today.DayOfWeek;
                if (todayDayOfWeek == 0) todayDayOfWeek = 7; // Sunday = 7

                var daysUntilNext = (DayOfWeek - todayDayOfWeek + 7) % 7;
                if (daysUntilNext == 0 && DateTime.Now.TimeOfDay > EndTime)
                {
                    daysUntilNext = 7; // If today but time has passed, next week
                }

                return today.AddDays(daysUntilNext).Add(StartTime);
            }
        }

        /// <summary>
        /// Check if the current time falls within this schedule
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Current Time")]
        public bool IsCurrentTimeInSchedule
        {
            get
            {
                var now = DateTime.Now;
                var nowDayOfWeek = (int)now.DayOfWeek;
                if (nowDayOfWeek == 0) nowDayOfWeek = 7;

                return nowDayOfWeek == DayOfWeek &&
                       now.TimeOfDay >= StartTime &&
                       now.TimeOfDay <= EndTime &&
                       IsActive;
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Check if this schedule overlaps with another schedule
        /// </summary>
        /// <param name="otherSchedule">Other schedule to check against</param>
        /// <returns>True if schedules overlap</returns>
        public bool OverlapsWith(TeacherSchedule otherSchedule)
        {
            if (DayOfWeek != otherSchedule.DayOfWeek) return false;

            return StartTime < otherSchedule.EndTime && EndTime > otherSchedule.StartTime;
        }

        /// <summary>
        /// Check if a specific time falls within this schedule
        /// </summary>
        /// <param name="dayOfWeek">Day of week (1-7)</param>
        /// <param name="time">Time to check</param>
        /// <returns>True if time is within schedule</returns>
        public bool ContainsTime(int dayOfWeek, TimeSpan time)
        {
            return IsActive && DayOfWeek == dayOfWeek && time >= StartTime && time <= EndTime;
        }

        /// <summary>
        /// Check if a time range falls within this schedule
        /// </summary>
        /// <param name="dayOfWeek">Day of week (1-7)</param>
        /// <param name="startTime">Start time of range</param>
        /// <param name="endTime">End time of range</param>
        /// <returns>True if entire range is within schedule</returns>
        public bool ContainsTimeRange(int dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            return IsActive &&
                   DayOfWeek == dayOfWeek &&
                   startTime >= StartTime &&
                   endTime <= EndTime;
        }

        /// <summary>
        /// Check if schedule is valid (end time after start time, reasonable duration)
        /// </summary>
        /// <returns>True if schedule is valid</returns>
        public bool IsValidSchedule()
        {
            return EndTime > StartTime && DurationHours <= 12; // Max 12 hours per day
        }

        /// <summary>
        /// Get available time slots within this schedule (assuming 1-hour slots)
        /// </summary>
        /// <param name="slotDuration">Duration of each slot in minutes</param>
        /// <returns>List of available time slots</returns>
        public List<(TimeSpan StartTime, TimeSpan EndTime)> GetAvailableTimeSlots(int slotDuration = 60)
        {
            var slots = new List<(TimeSpan, TimeSpan)>();
            var currentTime = StartTime;
            var slotDurationSpan = TimeSpan.FromMinutes(slotDuration);

            while (currentTime.Add(slotDurationSpan) <= EndTime)
            {
                var slotEndTime = currentTime.Add(slotDurationSpan);
                slots.Add((currentTime, slotEndTime));
                currentTime = slotEndTime;
            }

            return slots;
        }

        /// <summary>
        /// Calculate overlap duration with another schedule
        /// </summary>
        /// <param name="otherSchedule">Other schedule to check overlap with</param>
        /// <returns>Overlap duration in hours, 0 if no overlap</returns>
        public double GetOverlapDurationHours(TeacherSchedule otherSchedule)
        {
            if (DayOfWeek != otherSchedule.DayOfWeek) return 0;

            var overlapStart = StartTime > otherSchedule.StartTime ? StartTime : otherSchedule.StartTime;
            var overlapEnd = EndTime < otherSchedule.EndTime ? EndTime : otherSchedule.EndTime;

            if (overlapStart >= overlapEnd) return 0;

            return (overlapEnd - overlapStart).TotalHours;
        }

        /// <summary>
        /// Check if this schedule can accommodate a specific class duration
        /// </summary>
        /// <param name="classDurationMinutes">Class duration in minutes</param>
        /// <returns>True if schedule can accommodate the class</returns>
        public bool CanAccommodateClass(int classDurationMinutes)
        {
            return IsActive && DurationMinutes >= classDurationMinutes;
        }

        /// <summary>
        /// Get the next specific date when this schedule occurs
        /// </summary>
        /// <param name="fromDate">Start date to search from</param>
        /// <returns>Next occurrence date</returns>
        public DateTime GetNextOccurrence(DateTime fromDate)
        {
            var fromDayOfWeek = (int)fromDate.DayOfWeek;
            if (fromDayOfWeek == 0) fromDayOfWeek = 7;

            var daysUntilNext = (DayOfWeek - fromDayOfWeek + 7) % 7;
            if (daysUntilNext == 0 && fromDate.TimeOfDay > EndTime)
            {
                daysUntilNext = 7;
            }

            return fromDate.Date.AddDays(daysUntilNext).Add(StartTime);
        }

        /// <summary>
        /// Get all occurrences of this schedule within a date range
        /// </summary>
        /// <param name="startDate">Range start date</param>
        /// <param name="endDate">Range end date</param>
        /// <returns>List of occurrence dates</returns>
        public List<DateTime> GetOccurrencesInRange(DateTime startDate, DateTime endDate)
        {
            var occurrences = new List<DateTime>();
            var currentDate = GetNextOccurrence(startDate);

            while (currentDate.Date <= endDate.Date)
            {
                if (currentDate.Date >= startDate.Date)
                {
                    occurrences.Add(currentDate);
                }
                currentDate = currentDate.AddDays(7); // Next week same day
            }

            return occurrences;
        }

        /// <summary>
        /// Calculate total working hours per week for this schedule
        /// </summary>
        /// <returns>Working hours per week</returns>
        public double WeeklyWorkingHours => DurationHours;

        /// <summary>
        /// Calculate total working hours per month for this schedule
        /// </summary>
        /// <returns>Approximate working hours per month (4.33 weeks)</returns>
        public double MonthlyWorkingHours => DurationHours * 4.33;

        /// <summary>
        /// Get schedule flexibility score (based on duration and time coverage)
        /// </summary>
        /// <returns>Flexibility score (0-100)</returns>
        public double GetFlexibilityScore()
        {
            // Longer duration = more flexible
            var durationScore = Math.Min(DurationHours / 8 * 50, 50); // Max 50 points for 8+ hours

            // Time coverage score (morning + afternoon + evening)
            var coverageScore = 0.0;
            if (IsMorningSchedule && EndTime > new TimeSpan(12, 0, 0)) coverageScore += 25;
            if (IsAfternoonSchedule) coverageScore += 25;
            if (IsEveningSchedule || EndTime > new TimeSpan(18, 0, 0)) coverageScore += 25;

            return Math.Min(durationScore + coverageScore, 100);
        }

        /// <summary>
        /// Generate schedule summary for reporting
        /// </summary>
        /// <returns>Schedule summary data</returns>
        public Dictionary<string, object> GetScheduleSummary()
        {
            return new Dictionary<string, object>
            {
                {"TeacherScheduleId", TeacherScheduleId},
                {"Teacher", new { Teacher?.TeacherId, Teacher?.TeacherCode, Teacher?.TeacherName }},
                {"DayOfWeek", DayOfWeek},
                {"DayOfWeekDisplay", DayOfWeekDisplay},
                {"StartTime", StartTime.ToString(@"hh\:mm")},
                {"EndTime", EndTime.ToString(@"hh\:mm")},
                {"TimeRange", TimeRangeDisplay},
                {"DurationHours", DurationHours},
                {"DurationMinutes", DurationMinutes},
                {"SchedulePeriod", SchedulePeriod},
                {"ScheduleType", ScheduleType},
                {"IsActive", IsActive},
                {"MaxPossibleClasses", MaxPossibleClasses},
                {"FlexibilityScore", GetFlexibilityScore()},
                {"NextOccurrence", NextOccurrence},
                {"WeeklyHours", WeeklyWorkingHours},
                {"MonthlyHours", MonthlyWorkingHours}
            };
        }

        /// <summary>
        /// Validate schedule business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateScheduleRules()
        {
            var errors = new List<string>();

            // End time must be after start time
            if (EndTime <= StartTime)
            {
                errors.Add("End time must be after start time");
            }

            // Schedule cannot exceed 12 hours
            if (DurationHours > 12)
            {
                errors.Add($"Schedule duration ({DurationHours:F1} hours) cannot exceed 12 hours");
            }

            // Schedule should be at least 30 minutes
            if (DurationMinutes < 30)
            {
                errors.Add($"Schedule duration ({DurationMinutes} minutes) should be at least 30 minutes");
            }

            // Start time should not be too early (before 6 AM)
            if (StartTime < new TimeSpan(6, 0, 0))
            {
                errors.Add("Start time should not be before 6:00 AM");
            }

            // End time should not be too late (after 10 PM)
            if (EndTime > new TimeSpan(22, 0, 0))
            {
                errors.Add("End time should not be after 10:00 PM");
            }

            return errors;
        }

        /// <summary>
        /// Check if this schedule conflicts with other schedules for the same teacher
        /// </summary>
        /// <param name="otherSchedules">Other schedules to check against</param>
        /// <returns>List of conflicting schedules</returns>
        public List<TeacherSchedule> GetConflictingSchedules(IEnumerable<TeacherSchedule> otherSchedules)
        {
            return otherSchedules
                .Where(schedule =>
                    schedule.TeacherScheduleId != TeacherScheduleId &&
                    schedule.TeacherId == TeacherId &&
                    schedule.IsActive &&
                    OverlapsWith(schedule))
                .ToList();
        }

        /// <summary>
        /// Generate weekly schedule view data
        /// </summary>
        /// <param name="allSchedules">All schedules for the teacher</param>
        /// <returns>Weekly schedule data</returns>
        public static Dictionary<string, object> GenerateWeeklyScheduleView(IEnumerable<TeacherSchedule> allSchedules)
        {
            var weeklyView = new Dictionary<string, List<object>>();
            var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            for (int i = 1; i <= 7; i++)
            {
                var daySchedules = allSchedules
                    .Where(s => s.DayOfWeek == i && s.IsActive)
                    .OrderBy(s => s.StartTime)
                    .Select(s => new
                    {
                        s.TeacherScheduleId,
                        s.TimeRangeDisplay,
                        s.DurationHours,
                        s.SchedulePeriod
                    })
                    .Cast<object>()
                    .ToList();

                weeklyView[days[i - 1]] = daySchedules;
            }

            return new Dictionary<string, object>
            {
                {"WeeklySchedule", weeklyView},
                {"TotalWeeklyHours", allSchedules.Where(s => s.IsActive).Sum(s => s.DurationHours)},
                {"ActiveSchedulesCount", allSchedules.Count(s => s.IsActive)},
                {"WorkingDays", allSchedules.Where(s => s.IsActive).Select(s => s.DayOfWeek).Distinct().Count()}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the schedule</returns>
        public override string ToString() => FullScheduleDisplay;
    }
}
