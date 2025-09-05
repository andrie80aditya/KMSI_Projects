using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a student's grade history record in the KMSI Course Management System
    /// Tracks student's progression through different grade levels over time
    /// </summary>
    [Table("StudentGradeHistories")]
    public class StudentGradeHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentGradeHistoryId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        [Display(Name = "Grade")]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Active, Completed, Extended

        [Range(0, 100, ErrorMessage = "Completion percentage must be between 0 and 100")]
        [Display(Name = "Completion Percentage")]
        [DisplayFormat(DataFormatString = "{0:F1}%", ApplyFormatInEditMode = true)]
        public decimal CompletionPercentage { get; set; } = 0;

        [Display(Name = "Is Current Grade")]
        public bool IsCurrentGrade { get; set; } = false;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Student this grade history belongs to
        /// </summary>
        [Required]
        [Display(Name = "Student")]
        public virtual Student Student { get; set; } = null!;

        /// <summary>
        /// Grade/level associated with this history record
        /// </summary>
        [Required]
        [Display(Name = "Grade")]
        public virtual Grade Grade { get; set; } = null!;

        /// <summary>
        /// User who created this grade history record
        /// </summary>
        public virtual User? CreatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining student and grade information
        /// </summary>
        [NotMapped]
        [Display(Name = "Grade History")]
        public string DisplayName => $"{Student?.FullName} - {Grade?.GradeName} ({Status})";

        /// <summary>
        /// Short display for lists
        /// </summary>
        [NotMapped]
        [Display(Name = "History")]
        public string ShortDisplay => $"{Grade?.GradeCode} - {Status} ({CompletionPercentage:F1}%)";

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
            "Active" => "primary",
            "Completed" => "success",
            "Extended" => "warning",
            "Dropped" => "danger",
            "Transferred" => "info",
            _ => "secondary"
        };

        /// <summary>
        /// Duration in this grade (in days)
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration (Days)")]
        public int DurationDays
        {
            get
            {
                var endDate = EndDate ?? DateTime.Today;
                return (int)(endDate - StartDate).TotalDays;
            }
        }

        /// <summary>
        /// Duration in this grade (in weeks)
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration (Weeks)")]
        public int DurationWeeks => (int)Math.Round(DurationDays / 7.0);

        /// <summary>
        /// Duration in this grade (in months)
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration (Months)")]
        public int DurationMonths => (int)Math.Round(DurationDays / 30.0);

        /// <summary>
        /// Duration display in human-readable format
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration")]
        public string DurationDisplay
        {
            get
            {
                if (DurationDays < 7) return $"{DurationDays} day{(DurationDays != 1 ? "s" : "")}";
                if (DurationDays < 60) return $"{DurationWeeks} week{(DurationWeeks != 1 ? "s" : "")}";
                return $"{DurationMonths} month{(DurationMonths != 1 ? "s" : "")}";
            }
        }

        /// <summary>
        /// Period display (start - end or start - ongoing)
        /// </summary>
        [NotMapped]
        [Display(Name = "Period")]
        public string PeriodDisplay
        {
            get
            {
                var start = StartDate.ToString("MMM yyyy");
                if (EndDate.HasValue)
                    return $"{start} - {EndDate.Value.ToString("MMM yyyy")}";
                return $"{start} - Ongoing";
            }
        }

        /// <summary>
        /// Completion status description
        /// </summary>
        [NotMapped]
        [Display(Name = "Progress Status")]
        public string ProgressStatus => CompletionPercentage switch
        {
            >= 100 => "Completed",
            >= 90 => "Nearly Complete",
            >= 75 => "Advanced",
            >= 50 => "Halfway",
            >= 25 => "Beginning",
            > 0 => "Just Started",
            _ => "Not Started"
        };

        /// <summary>
        /// Check if this grade history is currently active
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Active")]
        public bool IsActive => Status == "Active" && IsCurrentGrade && !EndDate.HasValue;

        /// <summary>
        /// Check if this grade history is completed
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Completed")]
        public bool IsCompleted => Status == "Completed" && EndDate.HasValue;

        /// <summary>
        /// Check if this grade is taking longer than expected
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Extended")]
        public bool IsExtended
        {
            get
            {
                if (Grade?.Duration == null) return false;
                var expectedDays = Grade.Duration.Value * 7; // Convert weeks to days
                return DurationDays > expectedDays * 1.5; // 50% longer than expected
            }
        }

        /// <summary>
        /// Expected completion date based on grade duration
        /// </summary>
        [NotMapped]
        [Display(Name = "Expected Completion")]
        public DateTime? ExpectedCompletionDate
        {
            get
            {
                if (Grade?.Duration == null) return null;
                return StartDate.AddDays(Grade.Duration.Value * 7);
            }
        }

        /// <summary>
        /// Days remaining until expected completion
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Remaining")]
        public int? DaysRemaining
        {
            get
            {
                if (!ExpectedCompletionDate.HasValue || IsCompleted) return null;
                var remaining = (ExpectedCompletionDate.Value - DateTime.Today).Days;
                return Math.Max(0, remaining);
            }
        }

        /// <summary>
        /// Progress velocity (completion percentage per week)
        /// </summary>
        [NotMapped]
        [Display(Name = "Progress Velocity")]
        [DisplayFormat(DataFormatString = "{0:F2}% per week", ApplyFormatInEditMode = true)]
        public decimal ProgressVelocity
        {
            get
            {
                if (DurationWeeks <= 0) return 0;
                return CompletionPercentage / DurationWeeks;
            }
        }

        /// <summary>
        /// Estimated completion date based on current progress velocity
        /// </summary>
        [NotMapped]
        [Display(Name = "Estimated Completion")]
        public DateTime? EstimatedCompletionDate
        {
            get
            {
                if (IsCompleted || ProgressVelocity <= 0) return null;

                var remainingPercentage = 100 - CompletionPercentage;
                var estimatedWeeks = remainingPercentage / ProgressVelocity;
                return DateTime.Today.AddDays((double)estimatedWeeks * 7);
            }
        }

        /// <summary>
        /// Performance indicator based on progress vs time
        /// </summary>
        [NotMapped]
        [Display(Name = "Performance Indicator")]
        public string PerformanceIndicator
        {
            get
            {
                if (Grade?.Duration == null) return "Unknown";

                var expectedProgress = (decimal)DurationWeeks / Grade.Duration.Value * 100;
                var actualProgress = CompletionPercentage;

                var ratio = expectedProgress > 0 ? actualProgress / expectedProgress : 0;

                return ratio switch
                {
                    >= 1.2m => "Excellent",
                    >= 1.0m => "On Track",
                    >= 0.8m => "Slightly Behind",
                    >= 0.6m => "Behind Schedule",
                    _ => "Significantly Behind"
                };
            }
        }

        /// <summary>
        /// Academic year this grade history falls into
        /// </summary>
        [NotMapped]
        [Display(Name = "Academic Year")]
        public string AcademicYear
        {
            get
            {
                // Academic year typically starts in September
                var year = StartDate.Month >= 9 ? StartDate.Year : StartDate.Year - 1;
                return $"{year}/{year + 1}";
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Mark grade as completed
        /// </summary>
        /// <param name="completionDate">Date of completion</param>
        /// <param name="completedBy">User ID who marked as completed</param>
        public void MarkAsCompleted(DateTime? completionDate = null, int? completedBy = null)
        {
            Status = "Completed";
            EndDate = completionDate ?? DateTime.Today;
            CompletionPercentage = 100;
            IsCurrentGrade = false;

            if (completedBy.HasValue)
                CreatedBy = completedBy.Value;
        }

        /// <summary>
        /// Extend the grade period
        /// </summary>
        /// <param name="extendedBy">User ID who extended</param>
        /// <param name="reason">Reason for extension</param>
        public void ExtendGrade(int extendedBy, string? reason = null)
        {
            Status = "Extended";
            CreatedBy = extendedBy;

            if (!string.IsNullOrEmpty(reason))
                Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}; Extended: {reason}";
        }

        /// <summary>
        /// Update completion percentage
        /// </summary>
        /// <param name="percentage">New completion percentage (0-100)</param>
        /// <param name="updatedBy">User ID who updated</param>
        public void UpdateCompletion(decimal percentage, int? updatedBy = null)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Completion percentage must be between 0 and 100");

            CompletionPercentage = percentage;

            // Auto-complete if 100%
            if (percentage >= 100 && Status == "Active")
            {
                MarkAsCompleted(completedBy: updatedBy);
            }

            if (updatedBy.HasValue)
                CreatedBy = updatedBy.Value;
        }

        /// <summary>
        /// Set as current grade for the student
        /// </summary>
        /// <param name="studentGradeHistories">All grade histories for the student</param>
        public void SetAsCurrent(ICollection<StudentGradeHistory> studentGradeHistories)
        {
            // Remove current flag from other grades
            foreach (var history in studentGradeHistories.Where(h => h.StudentId == StudentId))
            {
                history.IsCurrentGrade = false;
            }

            // Set this as current
            IsCurrentGrade = true;
            Status = "Active";
            EndDate = null;
        }

        /// <summary>
        /// Calculate expected progress based on time elapsed
        /// </summary>
        /// <returns>Expected completion percentage based on duration</returns>
        public decimal GetExpectedProgress()
        {
            if (Grade?.Duration == null) return 0;

            var expectedDurationDays = Grade.Duration.Value * 7;
            var progressRatio = Math.Min(1.0, (double)DurationDays / expectedDurationDays);

            return (decimal)(progressRatio * 100);
        }

        /// <summary>
        /// Check if student is ready for next grade
        /// </summary>
        /// <param name="minimumCompletion">Minimum completion percentage required</param>
        /// <returns>True if ready for promotion</returns>
        public bool IsReadyForPromotion(decimal minimumCompletion = 80)
        {
            return Status == "Active" && CompletionPercentage >= minimumCompletion;
        }

        /// <summary>
        /// Get progress milestones achieved
        /// </summary>
        /// <returns>List of milestones achieved</returns>
        public List<string> GetMilestonesAchieved()
        {
            var milestones = new List<string>();

            if (CompletionPercentage >= 25) milestones.Add("25% Complete");
            if (CompletionPercentage >= 50) milestones.Add("Halfway Point");
            if (CompletionPercentage >= 75) milestones.Add("75% Complete");
            if (CompletionPercentage >= 90) milestones.Add("Nearly Complete");
            if (CompletionPercentage >= 100) milestones.Add("Grade Completed");

            return milestones;
        }

        /// <summary>
        /// Generate progress report for this grade history
        /// </summary>
        /// <returns>Comprehensive progress report</returns>
        public Dictionary<string, object> GenerateProgressReport()
        {
            return new Dictionary<string, object>
            {
                {"Basic", new
                {
                    StudentGradeHistoryId = StudentGradeHistoryId,
                    Student = Student?.FullName,
                    Grade = Grade?.GradeName,
                    Status = Status,
                    IsCurrentGrade = IsCurrentGrade
                }},
                {"Timeline", new
                {
                    StartDate = StartDate,
                    EndDate = EndDate,
                    DurationDays = DurationDays,
                    DurationWeeks = DurationWeeks,
                    DurationMonths = DurationMonths,
                    PeriodDisplay = PeriodDisplay,
                    AcademicYear = AcademicYear
                }},
                {"Progress", new
                {
                    CompletionPercentage = CompletionPercentage,
                    ProgressStatus = ProgressStatus,
                    ProgressVelocity = ProgressVelocity,
                    ExpectedProgress = GetExpectedProgress(),
                    PerformanceIndicator = PerformanceIndicator,
                    MilestonesAchieved = GetMilestonesAchieved()
                }},
                {"Projections", new
                {
                    ExpectedCompletionDate = ExpectedCompletionDate,
                    EstimatedCompletionDate = EstimatedCompletionDate,
                    DaysRemaining = DaysRemaining,
                    IsExtended = IsExtended,
                    IsReadyForPromotion = IsReadyForPromotion()
                }},
                {"Notes", Notes}
            };
        }

        /// <summary>
        /// Get students with similar grade history patterns
        /// </summary>
        /// <param name="allHistories">All grade histories to compare against</param>
        /// <param name="similarityThreshold">Similarity threshold for matching</param>
        /// <returns>Students with similar patterns</returns>
        public List<StudentGradeHistory> GetSimilarPatterns(IEnumerable<StudentGradeHistory> allHistories,
            decimal similarityThreshold = 10)
        {
            return allHistories.Where(h =>
                h.StudentGradeHistoryId != StudentGradeHistoryId &&
                h.GradeId == GradeId &&
                h.Status == Status &&
                Math.Abs(h.CompletionPercentage - CompletionPercentage) <= similarityThreshold)
                .OrderBy(h => Math.Abs(h.CompletionPercentage - CompletionPercentage))
                .ToList();
        }

        /// <summary>
        /// Calculate grade completion statistics for a group of histories
        /// </summary>
        /// <param name="histories">Grade histories to analyze</param>
        /// <param name="gradeId">Specific grade to analyze (optional)</param>
        /// <returns>Completion statistics</returns>
        public static Dictionary<string, object> GetCompletionStatistics(IEnumerable<StudentGradeHistory> histories,
            int? gradeId = null)
        {
            var filteredHistories = gradeId.HasValue
                ? histories.Where(h => h.GradeId == gradeId.Value).ToList()
                : histories.ToList();

            if (!filteredHistories.Any())
                return new Dictionary<string, object>();

            var completed = filteredHistories.Where(h => h.Status == "Completed").ToList();
            var active = filteredHistories.Where(h => h.Status == "Active").ToList();
            var extended = filteredHistories.Where(h => h.Status == "Extended").ToList();

            return new Dictionary<string, object>
            {
                {"Total", filteredHistories.Count},
                {"Completed", completed.Count},
                {"Active", active.Count},
                {"Extended", extended.Count},
                {"CompletionRate", filteredHistories.Count > 0 ? (double)completed.Count / filteredHistories.Count : 0},
                {"AverageCompletion", filteredHistories.Average(h => (double)h.CompletionPercentage)},
                {"AverageDurationDays", completed.Any() ? completed.Average(h => h.DurationDays) : 0},
                {"MedianDurationDays", GetMedianDuration(completed)},
                {"ProgressDistribution", new
                {
                    JustStarted = filteredHistories.Count(h => h.CompletionPercentage < 25),
                    Beginning = filteredHistories.Count(h => h.CompletionPercentage >= 25 && h.CompletionPercentage < 50),
                    Halfway = filteredHistories.Count(h => h.CompletionPercentage >= 50 && h.CompletionPercentage < 75),
                    Advanced = filteredHistories.Count(h => h.CompletionPercentage >= 75 && h.CompletionPercentage < 100),
                    Completed = filteredHistories.Count(h => h.CompletionPercentage >= 100)
                }}
            };
        }

        /// <summary>
        /// Calculate median duration for completed grades
        /// </summary>
        /// <param name="completedHistories">Completed grade histories</param>
        /// <returns>Median duration in days</returns>
        private static double GetMedianDuration(List<StudentGradeHistory> completedHistories)
        {
            if (!completedHistories.Any()) return 0;

            var sortedDurations = completedHistories.Select(h => h.DurationDays).OrderBy(d => d).ToList();
            var count = sortedDurations.Count;

            if (count % 2 == 0)
                return (sortedDurations[count / 2 - 1] + sortedDurations[count / 2]) / 2.0;
            else
                return sortedDurations[count / 2];
        }

        /// <summary>
        /// Get grade progression report for a student
        /// </summary>
        /// <param name="studentHistories">All grade histories for a student</param>
        /// <returns>Student progression report</returns>
        public static Dictionary<string, object> GetStudentProgressionReport(IEnumerable<StudentGradeHistory> studentHistories)
        {
            var histories = studentHistories.OrderBy(h => h.StartDate).ToList();
            var currentGrade = histories.FirstOrDefault(h => h.IsCurrentGrade);
            var completedGrades = histories.Where(h => h.Status == "Completed").ToList();

            return new Dictionary<string, object>
            {
                {"StudentId", histories.FirstOrDefault()?.StudentId},
                {"TotalGrades", histories.Count},
                {"CompletedGrades", completedGrades.Count},
                {"CurrentGrade", currentGrade?.Grade?.GradeName},
                {"CurrentProgress", currentGrade?.CompletionPercentage ?? 0},
                {"TotalStudyTime", histories.Sum(h => h.DurationDays)},
                {"AverageGradeDuration", completedGrades.Any() ? completedGrades.Average(h => h.DurationDays) : 0},
                {"GradeProgression", histories.Select(h => new
                {
                    Grade = h.Grade?.GradeName,
                    StartDate = h.StartDate,
                    EndDate = h.EndDate,
                    Duration = h.DurationDisplay,
                    Completion = h.CompletionPercentage,
                    Status = h.Status
                }).ToList()},
                {"PerformanceTrend", AnalyzePerformanceTrend(histories)}
            };
        }

        /// <summary>
        /// Analyze performance trend across grades
        /// </summary>
        /// <param name="histories">Student's grade histories</param>
        /// <returns>Performance trend analysis</returns>
        private static string AnalyzePerformanceTrend(List<StudentGradeHistory> histories)
        {
            var completed = histories.Where(h => h.Status == "Completed").OrderBy(h => h.StartDate).ToList();
            if (completed.Count < 2) return "Insufficient Data";

            var recentPerformance = completed.TakeLast(3).Average(h => (double)h.ProgressVelocity);
            var earlierPerformance = completed.Take(completed.Count - 3).Average(h => (double)h.ProgressVelocity);

            if (recentPerformance > earlierPerformance * 1.1) return "Improving";
            if (recentPerformance < earlierPerformance * 0.9) return "Declining";
            return "Consistent";
        }

        /// <summary>
        /// Validate grade history business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateGradeHistoryRules()
        {
            var errors = new List<string>();

            // End date must be after start date
            if (EndDate.HasValue && EndDate.Value <= StartDate)
            {
                errors.Add("End date must be after start date");
            }

            // Completed status must have end date
            if (Status == "Completed" && !EndDate.HasValue)
            {
                errors.Add("Completed grade must have an end date");
            }

            // Active status should not have end date
            if (Status == "Active" && EndDate.HasValue)
            {
                errors.Add("Active grade should not have an end date");
            }

            // Completion percentage validation
            if (Status == "Completed" && CompletionPercentage < 100)
            {
                errors.Add("Completed grade must have 100% completion");
            }

            // Current grade consistency
            if (IsCurrentGrade && Status != "Active")
            {
                errors.Add("Current grade must have Active status");
            }

            // Future start date validation
            if (StartDate > DateTime.Today.AddDays(1))
            {
                errors.Add("Start date cannot be in the future");
            }

            return errors;
        }

        /// <summary>
        /// Mark a completion with updated by information
        /// </summary>
        /// <param name="completionPercentage">Completion percentage</param>
        /// <param name="notes">Optional notes</param>
        /// <param name="updatedBy">User who updated the record</param>
        public void MarkCompleted(decimal completionPercentage, string? notes = null, int? updatedBy = null)
        {
            CompletionPercentage = completionPercentage;
            Status = completionPercentage >= 100 ? "Completed" : "Active";
            Notes = notes;
            IsCurrentGrade = Status == "Active";

            // Set audit fields if provided
            if (updatedBy.HasValue)
            {
                // Note: You'll need to add UpdatedBy and UpdatedDate properties to the model
                // UpdatedBy = updatedBy;
                // UpdatedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the grade history</returns>
        public override string ToString() => DisplayName;
    }
}
