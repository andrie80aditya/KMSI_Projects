using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents billing periods for managing recurring billing cycles
    /// Controls the billing workflow from draft to finalized state
    /// Used for both student billings and teacher payrolls
    /// </summary>
    [Table("BillingPeriods")]

    public class BillingPeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillingPeriodId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Period name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Period name must be between 3 and 50 characters")]
        [Display(Name = "Period Name")]
        public string PeriodName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Draft"; // Draft, Generated, Finalized, Closed

        [Display(Name = "Generated Date")]
        [DataType(DataType.DateTime)]
        public DateTime? GeneratedDate { get; set; }

        [Display(Name = "Finalized Date")]
        [DataType(DataType.DateTime)]
        public DateTime? FinalizedDate { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Company that owns this billing period
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// User who created this billing period
        /// </summary>
        [Display(Name = "Created By User")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// Student billings generated for this period
        /// </summary>
        [Display(Name = "Student Billings")]
        public virtual ICollection<StudentBilling> StudentBillings { get; set; } = new List<StudentBilling>();

        /// <summary>
        /// Teacher payrolls generated for this period
        /// </summary>
        [Display(Name = "Teacher Payrolls")]
        public virtual ICollection<TeacherPayroll> TeacherPayrolls { get; set; } = new List<TeacherPayroll>();

        // Computed Properties (Not Mapped - calculated properties)

        /// <summary>
        /// Duration of the billing period in days
        /// </summary>
        [NotMapped]
        [Display(Name = "Period Duration (Days)")]
        public int PeriodDuration => (EndDate - StartDate).Days + 1;

        /// <summary>
        /// Number of days from end date to due date (grace period)
        /// </summary>
        [NotMapped]
        [Display(Name = "Grace Period (Days)")]
        public int GracePeriod => (DueDate - EndDate).Days;

        /// <summary>
        /// Check if the period is currently active
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Active Period")]
        public bool IsActivePeriod
        {
            get
            {
                var today = DateTime.Today;
                return StartDate <= today && today <= EndDate;
            }
        }

        /// <summary>
        /// Check if the period has ended
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Past Period")]
        public bool IsPastPeriod => EndDate < DateTime.Today;

        /// <summary>
        /// Check if the period is in the future
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Future Period")]
        public bool IsFuturePeriod => StartDate > DateTime.Today;

        /// <summary>
        /// Check if the due date has passed
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Overdue")]
        public bool IsOverdue => DueDate < DateTime.Today && Status != "Closed";

        /// <summary>
        /// Days until start date (negative if already started)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Start")]
        public int DaysUntilStart => (StartDate - DateTime.Today).Days;

        /// <summary>
        /// Days until end date (negative if already ended)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until End")]
        public int DaysUntilEnd => (EndDate - DateTime.Today).Days;

        /// <summary>
        /// Days until due date (negative if overdue)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Due")]
        public int DaysUntilDue => (DueDate - DateTime.Today).Days;

        /// <summary>
        /// Progress percentage of the billing period (0-100)
        /// </summary>
        [NotMapped]
        [Display(Name = "Period Progress")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal PeriodProgress
        {
            get
            {
                var today = DateTime.Today;
                if (today < StartDate) return 0;
                if (today > EndDate) return 1;

                var totalDays = PeriodDuration;
                var elapsedDays = (today - StartDate).Days + 1;
                return totalDays > 0 ? (decimal)elapsedDays / totalDays : 0;
            }
        }

        /// <summary>
        /// Total number of student billings
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Student Billings")]
        public int TotalStudentBillings => StudentBillings?.Count ?? 0;

        /// <summary>
        /// Total number of teacher payrolls
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Teacher Payrolls")]
        public int TotalTeacherPayrolls => TeacherPayrolls?.Count ?? 0;

        /// <summary>
        /// Total amount of student billings
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Billing Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalBillingAmount => StudentBillings?.Sum(b => b.TotalAmount) ?? 0;

        /// <summary>
        /// Total amount of teacher payrolls
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Payroll Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalPayrollAmount => TeacherPayrolls?.Sum(p => p.NetSalary) ?? 0;

        /// <summary>
        /// Display name for dropdowns and lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{PeriodName} ({Status})";

        /// <summary>
        /// Short description for quick identification
        /// </summary>
        [NotMapped]
        [Display(Name = "Description")]
        public string Description => $"{PeriodName} - {StartDate:MMM dd} to {EndDate:MMM dd, yyyy}";

        /// <summary>
        /// Status with additional context
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Description")]
        public string StatusDescription
        {
            get
            {
                return Status switch
                {
                    "Draft" => $"Draft - Not yet generated",
                    "Generated" => $"Generated - Ready for review",
                    "Finalized" => $"Finalized - Bills sent out",
                    "Closed" => $"Closed - Period completed",
                    _ => Status
                };
            }
        }

        /// <summary>
        /// Period type based on duration
        /// </summary>
        [NotMapped]
        [Display(Name = "Period Type")]
        public string PeriodType
        {
            get
            {
                return PeriodDuration switch
                {
                    <= 7 => "Weekly",
                    <= 14 => "Bi-weekly",
                    <= 31 => "Monthly",
                    <= 93 => "Quarterly",
                    <= 186 => "Semi-annual",
                    <= 366 => "Annual",
                    _ => "Custom"
                };
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Validate business rules for billing period
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateBusinessRules()
        {
            var errors = new List<string>();

            // End date must be after start date
            if (EndDate <= StartDate)
            {
                errors.Add("End date must be after start date");
            }

            // Due date should be after end date (grace period)
            if (DueDate < EndDate)
            {
                errors.Add("Due date should be on or after end date");
            }

            // Period duration should be reasonable (at least 1 day, max 1 year)
            if (PeriodDuration < 1)
            {
                errors.Add("Billing period must be at least 1 day");
            }
            else if (PeriodDuration > 366)
            {
                errors.Add("Billing period cannot exceed 1 year");
            }

            // Grace period should be reasonable (max 90 days)
            if (GracePeriod > 90)
            {
                errors.Add("Grace period should not exceed 90 days");
            }

            // Status validation
            var validStatuses = new[] { "Draft", "Generated", "Finalized", "Closed" };
            if (!validStatuses.Contains(Status))
            {
                errors.Add($"Status must be one of: {string.Join(", ", validStatuses)}");
            }

            // Date consistency with status
            if (Status == "Generated" && !GeneratedDate.HasValue)
            {
                errors.Add("Generated date is required when status is 'Generated'");
            }

            if (Status == "Finalized" && !FinalizedDate.HasValue)
            {
                errors.Add("Finalized date is required when status is 'Finalized'");
            }

            // Generated date should be after created date
            if (GeneratedDate.HasValue && GeneratedDate.Value < CreatedDate)
            {
                errors.Add("Generated date cannot be before created date");
            }

            // Finalized date should be after generated date
            if (FinalizedDate.HasValue && GeneratedDate.HasValue && FinalizedDate.Value < GeneratedDate.Value)
            {
                errors.Add("Finalized date cannot be before generated date");
            }

            return errors;
        }

        /// <summary>
        /// Check if the period can be moved to the next status
        /// </summary>
        /// <returns>True if status can be advanced</returns>
        public bool CanAdvanceStatus()
        {
            return Status switch
            {
                "Draft" => true, // Can generate
                "Generated" => true, // Can finalize
                "Finalized" => IsOverdue || IsPastPeriod, // Can close if overdue or past
                "Closed" => false, // Cannot advance further
                _ => false
            };
        }

        /// <summary>
        /// Advance to the next status
        /// </summary>
        /// <returns>True if successful</returns>
        public bool AdvanceStatus()
        {
            if (!CanAdvanceStatus()) return false;

            switch (Status)
            {
                case "Draft":
                    Status = "Generated";
                    GeneratedDate = DateTime.Now;
                    break;
                case "Generated":
                    Status = "Finalized";
                    FinalizedDate = DateTime.Now;
                    break;
                case "Finalized":
                    Status = "Closed";
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if bills can be generated for this period
        /// </summary>
        /// <returns>True if bills can be generated</returns>
        public bool CanGenerateBills()
        {
            return Status == "Draft" &&
                   !IsFuturePeriod &&
                   ValidateBusinessRules().Count == 0;
        }

        /// <summary>
        /// Check if bills can be finalized for this period
        /// </summary>
        /// <returns>True if bills can be finalized</returns>
        public bool CanFinalizeBills()
        {
            return Status == "Generated" &&
                   GeneratedDate.HasValue;
        }

        /// <summary>
        /// Check if the period can be closed
        /// </summary>
        /// <returns>True if period can be closed</returns>
        public bool CanClosePeriod()
        {
            return Status == "Finalized" &&
                   FinalizedDate.HasValue &&
                   (IsOverdue || IsPastPeriod);
        }

        /// <summary>
        /// Generate period name based on start date
        /// </summary>
        /// <param name="startDate">Start date of the period</param>
        /// <returns>Generated period name</returns>
        public static string GeneratePeriodName(DateTime startDate)
        {
            return $"{startDate:MMMM yyyy}";
        }

        /// <summary>
        /// Generate period name for a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Generated period name</returns>
        public static string GeneratePeriodName(DateTime startDate, DateTime endDate)
        {
            if (startDate.Year == endDate.Year)
            {
                if (startDate.Month == endDate.Month)
                {
                    return $"{startDate:MMMM yyyy}";
                }
                return $"{startDate:MMM} - {endDate:MMM yyyy}";
            }
            return $"{startDate:MMM yyyy} - {endDate:MMM yyyy}";
        }

        /// <summary>
        /// Create next billing period based on this one
        /// </summary>
        /// <returns>New billing period for next cycle</returns>
        public BillingPeriod CreateNextPeriod()
        {
            var nextStart = EndDate.AddDays(1);
            var nextEnd = nextStart.AddDays(PeriodDuration - 1);
            var nextDue = nextEnd.AddDays(GracePeriod);

            return new BillingPeriod
            {
                CompanyId = CompanyId,
                PeriodName = GeneratePeriodName(nextStart, nextEnd),
                StartDate = nextStart,
                EndDate = nextEnd,
                DueDate = nextDue,
                Status = "Draft",
                CreatedBy = CreatedBy
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the billing period</returns>
        public override string ToString() => DisplayName;
    }
}
