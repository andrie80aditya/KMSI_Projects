using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents teacher payroll record in the KMSI Course Management System
    /// Tracks teacher compensation based on teaching hours and other factors
    /// </summary>
    /// [Table("TeacherPayrolls")]
    public class TeacherPayroll
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayrollId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Billing period is required")]
        [Display(Name = "Billing Period")]
        [ForeignKey("BillingPeriod")]
        public int BillingPeriodId { get; set; }

        [Required(ErrorMessage = "Teacher is required")]
        [Display(Name = "Teacher")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Payroll number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Payroll number must be between 5 and 20 characters")]
        [Display(Name = "Payroll Number")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Payroll number must contain only uppercase letters, numbers, and hyphens")]
        public string PayrollNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payroll date is required")]
        [Display(Name = "Payroll Date")]
        [DataType(DataType.Date)]
        public DateTime PayrollDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Total teaching hours is required")]
        [Range(0, 999.99, ErrorMessage = "Total teaching hours must be between 0 and 999.99")]
        [Display(Name = "Total Teaching Hours")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal TotalTeachingHours { get; set; } = 0;

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, 9999999.99, ErrorMessage = "Hourly rate must be between 0 and 9,999,999.99")]
        [Display(Name = "Hourly Rate")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal HourlyRate { get; set; } = 0;

        [Required(ErrorMessage = "Basic salary is required")]
        [Range(0, 999999999.99, ErrorMessage = "Basic salary must be between 0 and 999,999,999.99")]
        [Display(Name = "Basic Salary")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BasicSalary { get; set; } = 0;

        [Range(0, 999999999.99, ErrorMessage = "Allowances must be between 0 and 999,999,999.99")]
        [Display(Name = "Allowances")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Allowances { get; set; } = 0;

        [Range(0, 999999999.99, ErrorMessage = "Deductions must be between 0 and 999,999,999.99")]
        [Display(Name = "Deductions")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Deductions { get; set; } = 0;

        [Range(0, 999999999.99, ErrorMessage = "Tax must be between 0 and 999,999,999.99")]
        [Display(Name = "Tax")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Tax { get; set; } = 0;

        [Required(ErrorMessage = "Net salary is required")]
        [Range(0, 999999999.99, ErrorMessage = "Net salary must be between 0 and 999,999,999.99")]
        [Display(Name = "Net Salary")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal NetSalary { get; set; } = 0;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Draft"; // Draft, Approved, Paid

        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        [StringLength(20, ErrorMessage = "Payment method cannot exceed 20 characters")]
        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [StringLength(50, ErrorMessage = "Payment reference cannot exceed 50 characters")]
        [Display(Name = "Payment Reference")]
        public string? PaymentReference { get; set; }

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

        [Display(Name = "Updated Date")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Updated By")]
        [ForeignKey("UpdatedByUser")]
        public int? UpdatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Company that owns this payroll record
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where this payroll record belongs
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Billing period for this payroll
        /// </summary>
        [Required]
        [Display(Name = "Billing Period")]
        public virtual BillingPeriod BillingPeriod { get; set; } = null!;

        /// <summary>
        /// Teacher this payroll belongs to
        /// </summary>
        [Required]
        [Display(Name = "Teacher")]
        public virtual Teacher Teacher { get; set; } = null!;

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this payroll record
        /// </summary>
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this payroll record
        /// </summary>
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining payroll number and teacher name
        /// </summary>
        [NotMapped]
        [Display(Name = "Payroll")]
        public string DisplayName => $"{PayrollNumber} - {Teacher?.TeacherName ?? "Unknown"}";

        /// <summary>
        /// Full display name with period information
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Payroll Info")]
        public string FullDisplayName => $"{PayrollNumber} - {Teacher?.TeacherName} ({BillingPeriod?.PeriodName})";

        /// <summary>
        /// Status display with color coding information
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => Status switch
        {
            "Draft" => "Draft",
            "Approved" => "Approved",
            "Paid" => "Paid",
            _ => "Unknown"
        };

        /// <summary>
        /// Status color for UI styling
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Color")]
        public string StatusColor => Status switch
        {
            "Draft" => "warning",
            "Approved" => "info",
            "Paid" => "success",
            _ => "secondary"
        };

        /// <summary>
        /// Payment method display
        /// </summary>
        [NotMapped]
        [Display(Name = "Payment Method")]
        public string PaymentMethodDisplay => PaymentMethod ?? "Not Specified";

        /// <summary>
        /// Days since payroll was created
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Created")]
        public int DaysSinceCreated => (DateTime.Now - CreatedDate).Days;

        /// <summary>
        /// Days since payment (if paid)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Payment")]
        public int? DaysSincePayment => PaymentDate.HasValue ? (DateTime.Now - PaymentDate.Value).Days : null;

        /// <summary>
        /// Gross salary (before deductions and tax)
        /// </summary>
        [NotMapped]
        [Display(Name = "Gross Salary")]
        [DataType(DataType.Currency)]
        public decimal GrossSalary => BasicSalary + Allowances;

        /// <summary>
        /// Total deductions including tax
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Deductions")]
        [DataType(DataType.Currency)]
        public decimal TotalDeductions => Deductions + Tax;

        /// <summary>
        /// Effective hourly rate including allowances
        /// </summary>
        [NotMapped]
        [Display(Name = "Effective Hourly Rate")]
        [DataType(DataType.Currency)]
        public decimal EffectiveHourlyRate => TotalTeachingHours > 0 ? GrossSalary / TotalTeachingHours : 0;

        /// <summary>
        /// Net hourly rate after all deductions
        /// </summary>
        [NotMapped]
        [Display(Name = "Net Hourly Rate")]
        [DataType(DataType.Currency)]
        public decimal NetHourlyRate => TotalTeachingHours > 0 ? NetSalary / TotalTeachingHours : 0;

        /// <summary>
        /// Tax percentage of gross salary
        /// </summary>
        [NotMapped]
        [Display(Name = "Tax Percentage")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal TaxPercentage => GrossSalary > 0 ? Tax / GrossSalary : 0;

        /// <summary>
        /// Deduction percentage of gross salary
        /// </summary>
        [NotMapped]
        [Display(Name = "Deduction Percentage")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal DeductionPercentage => GrossSalary > 0 ? Deductions / GrossSalary : 0;

        /// <summary>
        /// Total deduction percentage of gross salary
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Deduction Percentage")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal TotalDeductionPercentage => GrossSalary > 0 ? TotalDeductions / GrossSalary : 0;

        /// <summary>
        /// Check if payroll is overdue for payment
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Overdue")]
        public bool IsOverdue => Status == "Approved" && BillingPeriod?.EndDate < DateTime.Today.AddDays(-30);

        /// <summary>
        /// Check if payroll is ready for payment
        /// </summary>
        [NotMapped]
        [Display(Name = "Ready for Payment")]
        public bool IsReadyForPayment => Status == "Approved" && !PaymentDate.HasValue;

        /// <summary>
        /// Check if payroll has been paid
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Paid")]
        public bool IsPaid => Status == "Paid" && PaymentDate.HasValue;

        /// <summary>
        /// Working days in the billing period
        /// </summary>
        [NotMapped]
        [Display(Name = "Working Days")]
        public int WorkingDays
        {
            get
            {
                if (BillingPeriod == null) return 0;
                return (BillingPeriod.EndDate - BillingPeriod.StartDate).Days + 1;
            }
        }

        /// <summary>
        /// Average hours per day
        /// </summary>
        [NotMapped]
        [Display(Name = "Avg Hours Per Day")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public decimal AverageHoursPerDay => WorkingDays > 0 ? TotalTeachingHours / WorkingDays : 0;

        /// <summary>
        /// Performance indicator based on expected vs actual hours
        /// </summary>
        [NotMapped]
        [Display(Name = "Performance Indicator")]
        public string PerformanceIndicator
        {
            get
            {
                // Assuming 8 hours per day as standard
                var expectedHours = WorkingDays * 8;
                if (expectedHours == 0) return "No Data";

                var performanceRatio = (double)TotalTeachingHours / expectedHours;
                return performanceRatio switch
                {
                    >= 0.95 => "Excellent",
                    >= 0.85 => "Very Good",
                    >= 0.75 => "Good",
                    >= 0.60 => "Fair",
                    _ => "Below Standard"
                };
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Check if payroll number is unique within the same billing period and site
        /// </summary>
        /// <param name="otherPayrolls">Other payrolls to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsPayrollNumberUnique(IEnumerable<TeacherPayroll> otherPayrolls)
        {
            return !otherPayrolls.Any(p =>
                p.PayrollId != PayrollId &&
                p.PayrollNumber.ToUpper() == PayrollNumber.ToUpper() &&
                p.BillingPeriodId == BillingPeriodId &&
                p.SiteId == SiteId);
        }

        /// <summary>
        /// Calculate basic salary from hours and rate
        /// </summary>
        public void CalculateBasicSalary()
        {
            BasicSalary = TotalTeachingHours * HourlyRate;
        }

        /// <summary>
        /// Calculate net salary from all components
        /// </summary>
        public void CalculateNetSalary()
        {
            NetSalary = BasicSalary + Allowances - Deductions - Tax;
        }

        /// <summary>
        /// Recalculate all salary components
        /// </summary>
        public void RecalculateAllSalaryComponents()
        {
            CalculateBasicSalary();
            CalculateNetSalary();
        }

        /// <summary>
        /// Apply standard tax calculation (configurable percentage)
        /// </summary>
        /// <param name="taxPercentage">Tax percentage (e.g., 0.1 for 10%)</param>
        public void ApplyTaxCalculation(decimal taxPercentage = 0.05m) // Default 5%
        {
            Tax = Math.Round(GrossSalary * taxPercentage, 2);
            CalculateNetSalary();
        }

        /// <summary>
        /// Apply standard deductions (insurance, etc.)
        /// </summary>
        /// <param name="insuranceAmount">Insurance deduction amount</param>
        /// <param name="otherDeductions">Other deduction amounts</param>
        public void ApplyStandardDeductions(decimal insuranceAmount = 0, decimal otherDeductions = 0)
        {
            Deductions = insuranceAmount + otherDeductions;
            CalculateNetSalary();
        }

        /// <summary>
        /// Approve payroll for payment
        /// </summary>
        /// <param name="approvedBy">User ID who approved</param>
        public void Approve(int approvedBy)
        {
            if (Status != "Draft")
                throw new InvalidOperationException("Only draft payrolls can be approved");

            Status = "Approved";
            UpdatedBy = approvedBy;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Mark payroll as paid
        /// </summary>
        /// <param name="paymentDate">Date of payment</param>
        /// <param name="paymentMethod">Method of payment</param>
        /// <param name="paymentReference">Payment reference number</param>
        /// <param name="processedBy">User ID who processed payment</param>
        public void MarkAsPaid(DateTime paymentDate, string paymentMethod, string? paymentReference, int processedBy)
        {
            if (Status != "Approved")
                throw new InvalidOperationException("Only approved payrolls can be marked as paid");

            Status = "Paid";
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
            PaymentReference = paymentReference;
            UpdatedBy = processedBy;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Revert payroll to draft status (if not paid)
        /// </summary>
        /// <param name="revertedBy">User ID who reverted</param>
        public void RevertToDraft(int revertedBy)
        {
            if (Status == "Paid")
                throw new InvalidOperationException("Paid payrolls cannot be reverted to draft");

            Status = "Draft";
            PaymentDate = null;
            PaymentMethod = null;
            PaymentReference = null;
            UpdatedBy = revertedBy;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Generate payroll summary for reports
        /// </summary>
        /// <returns>Payroll summary data</returns>
        public Dictionary<string, object> GetPayrollSummary()
        {
            return new Dictionary<string, object>
            {
                {"PayrollId", PayrollId},
                {"PayrollNumber", PayrollNumber},
                {"Teacher", new { Teacher?.TeacherId, Teacher?.TeacherCode, Teacher?.TeacherName }},
                {"Site", Site?.SiteName},
                {"Period", BillingPeriod?.PeriodName},
                {"Status", Status},
                {"Hours", new
                {
                    Total = TotalTeachingHours,
                    Average = AverageHoursPerDay,
                    WorkingDays = WorkingDays
                }},
                {"Salary", new
                {
                    HourlyRate = HourlyRate,
                    BasicSalary = BasicSalary,
                    Allowances = Allowances,
                    GrossSalary = GrossSalary,
                    Deductions = Deductions,
                    Tax = Tax,
                    NetSalary = NetSalary,
                    EffectiveHourlyRate = EffectiveHourlyRate
                }},
                {"Payment", new
                {
                    PaymentDate = PaymentDate,
                    PaymentMethod = PaymentMethod,
                    PaymentReference = PaymentReference,
                    IsPaid = IsPaid,
                    IsOverdue = IsOverdue
                }},
                {"Performance", PerformanceIndicator}
            };
        }

        /// <summary>
        /// Generate detailed payslip data
        /// </summary>
        /// <returns>Payslip data for printing/display</returns>
        public Dictionary<string, object> GeneratePayslip()
        {
            return new Dictionary<string, object>
            {
                {"Header", new
                {
                    Company = Company?.CompanyName,
                    Site = Site?.SiteName,
                    PayrollNumber = PayrollNumber,
                    PayrollDate = PayrollDate,
                    PeriodName = BillingPeriod?.PeriodName,
                    PeriodStart = BillingPeriod?.StartDate,
                    PeriodEnd = BillingPeriod?.EndDate
                }},
                {"Employee", new
                {
                    TeacherId = Teacher?.TeacherId,
                    TeacherCode = Teacher?.TeacherCode,
                    TeacherName = Teacher?.TeacherName,
                    Email = Teacher?.User?.Email,
                    Phone = Teacher?.User?.Phone
                }},
                {"WorkDetails", new
                {
                    TotalTeachingHours = TotalTeachingHours,
                    HourlyRate = HourlyRate,
                    WorkingDays = WorkingDays,
                    AverageHoursPerDay = AverageHoursPerDay,
                    Performance = PerformanceIndicator
                }},
                {"Earnings", new
                {
                    BasicSalary = BasicSalary,
                    Allowances = Allowances,
                    GrossSalary = GrossSalary
                }},
                {"Deductions", new
                {
                    Deductions = Deductions,
                    Tax = Tax,
                    TotalDeductions = TotalDeductions,
                    TaxPercentage = TaxPercentage
                }},
                {"Summary", new
                {
                    GrossSalary = GrossSalary,
                    TotalDeductions = TotalDeductions,
                    NetSalary = NetSalary,
                    NetHourlyRate = NetHourlyRate
                }},
                {"PaymentInfo", new
                {
                    Status = Status,
                    PaymentDate = PaymentDate,
                    PaymentMethod = PaymentMethod,
                    PaymentReference = PaymentReference
                }},
                {"Notes", Notes},
                {"GeneratedDate", DateTime.Now}
            };
        }

        /// <summary>
        /// Validate payroll business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidatePayrollRules()
        {
            var errors = new List<string>();

            // Basic salary should match calculation
            var calculatedBasicSalary = TotalTeachingHours * HourlyRate;
            if (Math.Abs(BasicSalary - calculatedBasicSalary) > 0.01m)
            {
                errors.Add($"Basic salary ({BasicSalary:C}) doesn't match calculated amount ({calculatedBasicSalary:C})");
            }

            // Net salary should match calculation
            var calculatedNetSalary = BasicSalary + Allowances - Deductions - Tax;
            if (Math.Abs(NetSalary - calculatedNetSalary) > 0.01m)
            {
                errors.Add($"Net salary ({NetSalary:C}) doesn't match calculated amount ({calculatedNetSalary:C})");
            }

            // Paid payrolls must have payment date
            if (Status == "Paid" && !PaymentDate.HasValue)
            {
                errors.Add("Paid payrolls must have payment date");
            }

            // Payment date should not be in the future
            if (PaymentDate.HasValue && PaymentDate.Value.Date > DateTime.Today)
            {
                errors.Add("Payment date cannot be in the future");
            }

            // Teaching hours should be reasonable
            if (TotalTeachingHours > WorkingDays * 12) // More than 12 hours per day
            {
                errors.Add($"Teaching hours ({TotalTeachingHours}) seems excessive for {WorkingDays} working days");
            }

            // Net salary should not be negative
            if (NetSalary < 0)
            {
                errors.Add("Net salary cannot be negative");
            }

            return errors;
        }

        /// <summary>
        /// Generate next payroll number for a teacher and period
        /// </summary>
        /// <param name="teacherCode">Teacher code</param>
        /// <param name="billingPeriod">Billing period</param>
        /// <param name="existingPayrolls">Existing payroll numbers</param>
        /// <returns>Next payroll number</returns>
        public static string GeneratePayrollNumber(string teacherCode, BillingPeriod billingPeriod, IEnumerable<string> existingPayrolls)
        {
            var periodCode = billingPeriod.StartDate.ToString("yyyyMM");
            var prefix = $"PAY-{teacherCode}-{periodCode}-";

            var existingNumbers = existingPayrolls
                .Where(p => p.StartsWith(prefix))
                .Select(p =>
                {
                    var parts = p.Split('-');
                    return parts.Length == 4 && int.TryParse(parts[3], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingNumbers.Max() + 1;
            return $"{prefix}{nextNumber:000}";
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the payroll</returns>
        public override string ToString() => DisplayName;
    }
}
