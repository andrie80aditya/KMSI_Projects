using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a student's examination record in the KMSI Course Management System
    /// Links students to examinations with their results and performance details
    /// </summary>
    [Table("StudentExaminations")]
    public class StudentExamination
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentExaminationId { get; set; }

        [Required(ErrorMessage = "Examination is required")]
        [Display(Name = "Examination")]
        [ForeignKey("Examination")]
        public int ExaminationId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Display(Name = "Registration Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [StringLength(20, ErrorMessage = "Attendance status cannot exceed 20 characters")]
        [Display(Name = "Attendance Status")]
        public string? AttendanceStatus { get; set; } // Present, Absent, Late

        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan? EndTime { get; set; }

        /// <summary>
        /// Actual duration computed column (EndTime - StartTime in minutes)
        /// </summary>
        [Display(Name = "Actual Duration (Minutes)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? ActualDuration { get; set; }

        [Range(0, 9999.99, ErrorMessage = "Score must be between 0 and 9999.99")]
        [Display(Name = "Score")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal? Score { get; set; }

        [Range(0, 9999.99, ErrorMessage = "Maximum score must be between 0 and 9999.99")]
        [Display(Name = "Maximum Score")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal MaxScore { get; set; } = 100;

        /// <summary>
        /// Percentage computed column (Score / MaxScore * 100)
        /// </summary>
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
        [Display(Name = "Percentage")]
        [DisplayFormat(DataFormatString = "{0:F1}%", ApplyFormatInEditMode = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? Percentage { get; set; }

        [StringLength(2, ErrorMessage = "Grade cannot exceed 2 characters")]
        [Display(Name = "Grade")]
        public string? Grade { get; set; } // A+, A, B+, B, C+, C, D, F

        [StringLength(20, ErrorMessage = "Result cannot exceed 20 characters")]
        [Display(Name = "Result")]
        public string? Result { get; set; } // Pass, Fail, Need Retake

        [StringLength(1000, ErrorMessage = "Teacher notes cannot exceed 1000 characters")]
        [Display(Name = "Teacher Notes")]
        [DataType(DataType.MultilineText)]
        public string? TeacherNotes { get; set; }

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
        /// Examination this student examination belongs to
        /// </summary>
        [Required]
        [Display(Name = "Examination")]
        public virtual Examination Examination { get; set; } = null!;

        /// <summary>
        /// Student who took this examination
        /// </summary>
        [Required]
        [Display(Name = "Student")]
        public virtual Student Student { get; set; } = null!;

        /// <summary>
        /// Certificate issued for this examination (if applicable)
        /// </summary>
        [Display(Name = "Certificate")]
        public virtual Certificate? Certificate { get; set; }

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this examination record
        /// </summary>
        [InverseProperty("CreatedStudentExaminations")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this examination record
        /// </summary>
        [InverseProperty("UpdatedStudentExaminations")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining student and examination
        /// </summary>
        [NotMapped]
        [Display(Name = "Student Examination")]
        public string DisplayName => $"{Student?.FullName} - {Examination?.ExamName}";

        /// <summary>
        /// Short display for lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Exam Result")]
        public string ShortDisplay => $"{Examination?.ExamCode} - {Grade ?? "Pending"} ({Result ?? "Pending"})";

        /// <summary>
        /// Attendance status display
        /// </summary>
        [NotMapped]
        [Display(Name = "Attendance")]
        public string AttendanceStatusDisplay => AttendanceStatus ?? "Not Recorded";

        /// <summary>
        /// Attendance status color for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Attendance Color")]
        public string AttendanceStatusColor => AttendanceStatus switch
        {
            "Present" => "success",
            "Late" => "warning",
            "Absent" => "danger",
            _ => "secondary"
        };

        /// <summary>
        /// Result status color for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Result Color")]
        public string ResultStatusColor => Result switch
        {
            "Pass" => "success",
            "Fail" => "danger",
            "Need Retake" => "warning",
            _ => "secondary"
        };

        /// <summary>
        /// Grade color for UI styling
        /// </summary>
        [NotMapped]
        [Display(Name = "Grade Color")]
        public string GradeColor => Grade switch
        {
            "A+" or "A" => "success",
            "B+" or "B" => "info",
            "C+" or "C" => "warning",
            "D" => "danger",
            "F" => "dark",
            _ => "secondary"
        };

        /// <summary>
        /// Examination date from related examination
        /// </summary>
        [NotMapped]
        [Display(Name = "Exam Date")]
        public DateTime? ExamDate => Examination?.ExamDate;

        /// <summary>
        /// Examination time display
        /// </summary>
        [NotMapped]
        [Display(Name = "Exam Time")]
        public string ExamTimeDisplay
        {
            get
            {
                if (StartTime.HasValue && EndTime.HasValue)
                    return $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
                return Examination?.StartTime != null && Examination?.EndTime != null
                    ? $"{Examination.StartTime:hh\\:mm} - {Examination.EndTime:hh\\:mm}"
                    : "Not Specified";
            }
        }

        /// <summary>
        /// Duration display in human readable format
        /// </summary>
        [NotMapped]
        [Display(Name = "Duration")]
        public string DurationDisplay
        {
            get
            {
                if (ActualDuration.HasValue)
                {
                    var hours = ActualDuration.Value / 60;
                    var minutes = ActualDuration.Value % 60;
                    if (hours > 0)
                        return $"{hours}h {minutes}m";
                    return $"{minutes}m";
                }
                return "Not Recorded";
            }
        }

        /// <summary>
        /// Score display with maximum score
        /// </summary>
        [NotMapped]
        [Display(Name = "Score")]
        public string ScoreDisplay => Score.HasValue ? $"{Score:F1}/{MaxScore:F0}" : "Not Graded";

        /// <summary>
        /// Performance level based on percentage
        /// </summary>
        [NotMapped]
        [Display(Name = "Performance Level")]
        public string PerformanceLevel
        {
            get
            {
                if (!Percentage.HasValue) return "Not Graded";
                return Percentage.Value switch
                {
                    >= 95 => "Outstanding",
                    >= 85 => "Excellent",
                    >= 75 => "Good",
                    >= 65 => "Satisfactory",
                    >= 50 => "Needs Improvement",
                    _ => "Poor"
                };
            }
        }

        /// <summary>
        /// Check if examination was attended
        /// </summary>
        [NotMapped]
        [Display(Name = "Was Attended")]
        public bool WasAttended => AttendanceStatus == "Present" || AttendanceStatus == "Late";

        /// <summary>
        /// Check if examination was completed
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Completed")]
        public bool IsCompleted => WasAttended && Score.HasValue;

        /// <summary>
        /// Check if examination was passed
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Passed")]
        public bool IsPassed => Result == "Pass";

        /// <summary>
        /// Check if examination needs retake
        /// </summary>
        [NotMapped]
        [Display(Name = "Needs Retake")]
        public bool NeedsRetake => Result == "Need Retake";

        /// <summary>
        /// Check if result is pending
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Pending")]
        public bool IsPending => string.IsNullOrEmpty(Result) || (!Score.HasValue && WasAttended);

        /// <summary>
        /// Time until exam (if future) or time since exam (if past)
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Reference")]
        public string TimeReference
        {
            get
            {
                if (!ExamDate.HasValue) return "Unknown Date";

                var timeDiff = ExamDate.Value.Date - DateTime.Today;
                var days = (int)timeDiff.TotalDays;

                if (days > 0) return $"In {days} day{(days != 1 ? "s" : "")}";
                if (days == 0) return "Today";
                if (days > -7) return $"{Math.Abs(days)} day{(Math.Abs(days) != 1 ? "s" : "")} ago";
                if (days > -30) return $"{Math.Abs(days / 7)} week{(Math.Abs(days / 7) != 1 ? "s" : "")} ago";
                return ExamDate.Value.ToString("MMM yyyy");
            }
        }

        /// <summary>
        /// Improvement needed to pass (if failed)
        /// </summary>
        [NotMapped]
        [Display(Name = "Points Needed")]
        public decimal? PointsNeededToPass
        {
            get
            {
                if (!Score.HasValue || IsPassed) return null;
                var passingScore = MaxScore * 0.6m; // Assuming 60% is passing
                return Math.Max(0, passingScore - Score.Value);
            }
        }

        /// <summary>
        /// Rank among all students who took this exam
        /// </summary>
        [NotMapped]
        [Display(Name = "Rank")]
        public int? ExamRank { get; set; } // This would be calculated in service layer

        /// <summary>
        /// Certificate status
        /// </summary>
        [NotMapped]
        [Display(Name = "Certificate Status")]
        public string CertificateStatus => Certificate != null ? "Issued" : (IsPassed ? "Eligible" : "Not Eligible");

        // Business Logic Methods

        /// <summary>
        /// Record attendance for the examination
        /// </summary>
        /// <param name="status">Attendance status (Present, Late, Absent)</param>
        /// <param name="startTime">Actual start time</param>
        /// <param name="recordedBy">User who recorded attendance</param>
        public void RecordAttendance(string status, TimeSpan? startTime = null, int? recordedBy = null)
        {
            AttendanceStatus = status;
            StartTime = startTime;

            if (recordedBy.HasValue)
                UpdatedBy = recordedBy.Value;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Complete the examination with time tracking
        /// </summary>
        /// <param name="endTime">Actual end time</param>
        /// <param name="completedBy">User who marked as completed</param>
        public void CompleteExamination(TimeSpan endTime, int? completedBy = null)
        {
            EndTime = endTime;

            // Calculate actual duration
            if (StartTime.HasValue)
            {
                ActualDuration = (int)(endTime - StartTime.Value).TotalMinutes;
            }

            if (completedBy.HasValue)
                UpdatedBy = completedBy.Value;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Grade the examination
        /// </summary>
        /// <param name="score">Student's score</param>
        /// <param name="gradedBy">User who graded the exam</param>
        /// <param name="notes">Grading notes</param>
        public void GradeExamination(decimal score, int? gradedBy = null, string? notes = null)
        {
            Score = score;
            TeacherNotes = notes;

            // Calculate percentage (will be computed in database)
            Percentage = MaxScore > 0 ? score / MaxScore * 100 : 0;

            // Assign letter grade
            Grade = CalculateLetterGrade(Percentage.Value);

            // Determine result
            Result = DetermineResult(Percentage.Value);

            if (gradedBy.HasValue)
                UpdatedBy = gradedBy.Value;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Calculate letter grade from percentage
        /// </summary>
        /// <param name="percentage">Percentage score</param>
        /// <returns>Letter grade (A+, A, B+, etc.)</returns>
        private string CalculateLetterGrade(decimal percentage)
        {
            return percentage switch
            {
                >= 97 => "A+",
                >= 93 => "A",
                >= 90 => "A-",
                >= 87 => "B+",
                >= 83 => "B",
                >= 80 => "B-",
                >= 77 => "C+",
                >= 73 => "C",
                >= 70 => "C-",
                >= 67 => "D+",
                >= 65 => "D",
                _ => "F"
            };
        }

        /// <summary>
        /// Determine pass/fail result from percentage
        /// </summary>
        /// <param name="percentage">Percentage score</param>
        /// <returns>Result (Pass, Fail, Need Retake)</returns>
        private string DetermineResult(decimal percentage)
        {
            return percentage switch
            {
                >= 70 => "Pass",
                >= 50 => "Need Retake",
                _ => "Fail"
            };
        }

        /// <summary>
        /// Check if student is eligible for certificate
        /// </summary>
        /// <returns>True if eligible for certificate</returns>
        public bool IsEligibleForCertificate()
        {
            return IsPassed && WasAttended && Certificate == null;
        }

        /// <summary>
        /// Issue certificate for this examination
        /// </summary>
        /// <param name="certificateNumber">Certificate number</param>
        /// <param name="issuedBy">User who issued certificate</param>
        /// <returns>New certificate instance</returns>
        public Certificate IssueCertificate(string certificateNumber, int issuedBy)
        {
            if (!IsEligibleForCertificate())
                throw new InvalidOperationException("Student is not eligible for certificate");

            var certificate = new Certificate
            {
                StudentId = StudentId,
                StudentExaminationId = StudentExaminationId,
                CertificateNumber = certificateNumber,
                GradeId = Examination?.GradeId ?? 0,
                IssueDate = DateTime.Today,
                CertificateTitle = $"{Examination?.ExamName} Certificate",
                IssuedBy = Examination?.Company?.CompanyName ?? "KMSI",
                CreatedBy = issuedBy
            };

            return certificate;
        }

        /// <summary>
        /// Schedule retake examination
        /// </summary>
        /// <param name="newExaminationId">New examination ID for retake</param>
        /// <param name="scheduledBy">User who scheduled retake</param>
        /// <returns>New student examination for retake</returns>
        public StudentExamination ScheduleRetake(int newExaminationId, int scheduledBy)
        {
            if (Result != "Need Retake" && Result != "Fail")
                throw new InvalidOperationException("Only failed examinations can be retaken");

            var retake = new StudentExamination
            {
                ExaminationId = newExaminationId,
                StudentId = StudentId,
                MaxScore = MaxScore,
                CreatedBy = scheduledBy
            };

            return retake;
        }

        /// <summary>
        /// Get examination performance summary
        /// </summary>
        /// <returns>Performance summary data</returns>
        public Dictionary<string, object> GetPerformanceSummary()
        {
            return new Dictionary<string, object>
            {
                {"BasicInfo", new
                {
                    StudentExaminationId = StudentExaminationId,
                    Student = Student?.FullName,
                    Examination = Examination?.ExamName,
                    ExamDate = ExamDate,
                    Grade = Examination?.Grade?.GradeName
                }},
                {"Attendance", new
                {
                    AttendanceStatus = AttendanceStatus,
                    WasAttended = WasAttended,
                    ExamTime = ExamTimeDisplay,
                    Duration = DurationDisplay,
                    ActualDuration = ActualDuration
                }},
                {"Performance", new
                {
                    Score = Score,
                    MaxScore = MaxScore,
                    Percentage = Percentage,
                    Grade = Grade,
                    Result = Result,
                    PerformanceLevel = PerformanceLevel,
                    IsPassed = IsPassed,
                    NeedsRetake = NeedsRetake
                }},
                {"Certificate", new
                {
                    Status = CertificateStatus,
                    IsEligible = IsEligibleForCertificate(),
                    CertificateNumber = Certificate?.CertificateNumber
                }},
                {"Notes", TeacherNotes}
            };
        }

        /// <summary>
        /// Get examination statistics for a group of student examinations
        /// </summary>
        /// <param name="examinations">Student examinations to analyze</param>
        /// <returns>Statistical summary</returns>
        public static Dictionary<string, object> GetExaminationStatistics(IEnumerable<StudentExamination> examinations)
        {
            var completedExams = examinations.Where(e => e.IsCompleted).ToList();
            var attendedExams = examinations.Where(e => e.WasAttended).ToList();

            return new Dictionary<string, object>
            {
                {"Total", examinations.Count()},
                {"Attended", attendedExams.Count},
                {"Completed", completedExams.Count},
                {"AttendanceRate", examinations.Count() > 0 ? (double)attendedExams.Count / examinations.Count() : 0},
                {"Results", new
                {
                    Passed = completedExams.Count(e => e.IsPassed),
                    Failed = completedExams.Count(e => e.Result == "Fail"),
                    NeedRetake = completedExams.Count(e => e.NeedsRetake),
                    PassRate = completedExams.Count > 0 ? (double)completedExams.Count(e => e.IsPassed) / completedExams.Count : 0
                }},
                {"Scores", completedExams.Any() ? new
                {
                    Average = completedExams.Average(e => (double)(e.Percentage ?? 0)),
                    Highest = completedExams.Max(e => e.Percentage ?? 0),
                    Lowest = completedExams.Min(e => e.Percentage ?? 0),
                    Median = GetMedianPercentage(completedExams)
                } : null},
                {"GradeDistribution", completedExams.GroupBy(e => e.Grade).ToDictionary(g => g.Key ?? "Ungraded", g => g.Count())},
                {"PerformanceLevels", completedExams.GroupBy(e => e.PerformanceLevel).ToDictionary(g => g.Key, g => g.Count())}
            };
        }

        /// <summary>
        /// Calculate median percentage for completed examinations
        /// </summary>
        /// <param name="completedExaminations">Completed examinations</param>
        /// <returns>Median percentage</returns>
        private static decimal GetMedianPercentage(List<StudentExamination> completedExaminations)
        {
            if (!completedExaminations.Any()) return 0;

            var sortedPercentages = completedExaminations
                .Where(e => e.Percentage.HasValue)
                .Select(e => e.Percentage!.Value)
                .OrderBy(p => p)
                .ToList();

            if (!sortedPercentages.Any()) return 0;

            var count = sortedPercentages.Count;
            if (count % 2 == 0)
                return (sortedPercentages[count / 2 - 1] + sortedPercentages[count / 2]) / 2;
            else
                return sortedPercentages[count / 2];
        }

        /// <summary>
        /// Get student's examination history
        /// </summary>
        /// <param name="studentExaminations">All examinations for a student</param>
        /// <returns>Examination history summary</returns>
        public static Dictionary<string, object> GetStudentExaminationHistory(IEnumerable<StudentExamination> studentExaminations)
        {
            var exams = studentExaminations.OrderByDescending(e => e.ExamDate).ToList();
            var passedExams = exams.Where(e => e.IsPassed).ToList();

            return new Dictionary<string, object>
            {
                {"TotalExaminations", exams.Count},
                {"PassedExaminations", passedExams.Count},
                {"FailedExaminations", exams.Count(e => e.Result == "Fail")},
                {"RetakeExaminations", exams.Count(e => e.NeedsRetake)},
                {"AverageScore", exams.Where(e => e.IsCompleted).Average(e => (double)(e.Percentage ?? 0))},
                {"CertificatesEarned", exams.Count(e => e.Certificate != null)},
                {"RecentExaminations", exams.Take(5).Select(e => new
                {
                    ExamName = e.Examination?.ExamName,
                    ExamDate = e.ExamDate,
                    Grade = e.Grade,
                    Result = e.Result,
                    Percentage = e.Percentage
                }).ToList()},
                {"PerformanceTrend", AnalyzePerformanceTrend(exams)}
            };
        }

        /// <summary>
        /// Analyze performance trend over multiple examinations
        /// </summary>
        /// <param name="examinations">Student's examinations ordered by date</param>
        /// <returns>Performance trend analysis</returns>
        private static string AnalyzePerformanceTrend(List<StudentExamination> examinations)
        {
            var completed = examinations.Where(e => e.IsCompleted).OrderBy(e => e.ExamDate).ToList();
            if (completed.Count < 3) return "Insufficient Data";

            var recentAvg = completed.TakeLast(3).Average(e => (double)(e.Percentage ?? 0));
            var earlierAvg = completed.Take(completed.Count - 3).Average(e => (double)(e.Percentage ?? 0));

            if (recentAvg > earlierAvg + 5) return "Improving";
            if (recentAvg < earlierAvg - 5) return "Declining";
            return "Stable";
        }

        /// <summary>
        /// Validate student examination business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateStudentExaminationRules()
        {
            var errors = new List<string>();

            // End time must be after start time
            if (StartTime.HasValue && EndTime.HasValue && EndTime.Value <= StartTime.Value)
            {
                errors.Add("End time must be after start time");
            }

            // Score cannot exceed maximum score
            if (Score.HasValue && Score.Value > MaxScore)
            {
                errors.Add($"Score ({Score}) cannot exceed maximum score ({MaxScore})");
            }

            // Completed exams should have scores
            if (AttendanceStatus == "Present" && !Score.HasValue)
            {
                errors.Add("Present students should have scores recorded");
            }

            // Absent students should not have scores
            if (AttendanceStatus == "Absent" && Score.HasValue)
            {
                errors.Add("Absent students should not have scores");
            }

            // Passed exams should be eligible for certificates
            if (Result == "Pass" && !IsEligibleForCertificate() && Certificate == null)
            {
                errors.Add("Passed examinations should be eligible for certificates");
            }

            return errors;
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the student examination</returns>
        public override string ToString() => DisplayName;
    }
}
