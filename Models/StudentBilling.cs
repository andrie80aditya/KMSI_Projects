using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents student billing record in the KMSI Course Management System
    /// Tracks tuition fees, book costs, and payment details for students
    /// </summary>
    [Table("StudentBillings")]
    public class StudentBilling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillingId { get; set; }

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

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Billing number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Billing number must be between 5 and 20 characters")]
        [Display(Name = "Billing Number")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Billing number must contain only uppercase letters, numbers, and hyphens")]
        public string BillingNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Billing date is required")]
        [Display(Name = "Billing Date")]
        [DataType(DataType.Date)]
        public DateTime BillingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        [Display(Name = "Grade")]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Tuition fee is required")]
        [Range(0, 999999999.99, ErrorMessage = "Tuition fee must be between 0 and 999,999,999.99")]
        [Display(Name = "Tuition Fee")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal TuitionFee { get; set; } = 0;

        [Range(0, 999999999.99, ErrorMessage = "Book fees must be between 0 and 999,999,999.99")]
        [Display(Name = "Book Fees")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal BookFees { get; set; } = 0;

        [Range(0, 999999999.99, ErrorMessage = "Other fees must be between 0 and 999,999,999.99")]
        [Display(Name = "Other Fees")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal OtherFees { get; set; } = 0;

        [Range(0, 999999999.99, ErrorMessage = "Discount must be between 0 and 999,999,999.99")]
        [Display(Name = "Discount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Discount { get; set; } = 0;

        [Range(0, 999999999.99, ErrorMessage = "Tax must be between 0 and 999,999,999.99")]
        [Display(Name = "Tax")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Tax { get; set; } = 0;

        /// <summary>
        /// Total amount computed column (TuitionFee + BookFees + OtherFees - Discount + Tax)
        /// </summary>
        [Range(0, 999999999.99, ErrorMessage = "Total amount must be between 0 and 999,999,999.99")]
        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Outstanding"; // Outstanding, Paid, Overdue, Cancelled

        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Payment amount must be between 0 and 999,999,999.99")]
        [Display(Name = "Payment Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? PaymentAmount { get; set; }

        [StringLength(20, ErrorMessage = "Payment method cannot exceed 20 characters")]
        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [StringLength(50, ErrorMessage = "Payment reference cannot exceed 50 characters")]
        [Display(Name = "Payment Reference")]
        public string? PaymentReference { get; set; }

        [Display(Name = "Receipt Printed")]
        public bool ReceiptPrinted { get; set; } = false;

        [Display(Name = "Receipt Print Count")]
        public int ReceiptPrintCount { get; set; } = 0;

        [Display(Name = "Last Print Date")]
        [DataType(DataType.DateTime)]
        public DateTime? LastPrintDate { get; set; }

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
        /// Company that owns this billing record
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where this billing record belongs
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Billing period for this bill
        /// </summary>
        [Required]
        [Display(Name = "Billing Period")]
        public virtual BillingPeriod BillingPeriod { get; set; } = null!;

        /// <summary>
        /// Student this bill belongs to
        /// </summary>
        [Required]
        [Display(Name = "Student")]
        public virtual Student Student { get; set; } = null!;

        /// <summary>
        /// Grade/level the student was in during this billing period
        /// </summary>
        [Required]
        [Display(Name = "Grade")]
        public virtual Grade Grade { get; set; } = null!;

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this billing record
        /// </summary>
        [InverseProperty("CreatedStudentBillings")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this billing record
        /// </summary>
        [InverseProperty("UpdatedStudentBillings")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining billing number and student name
        /// </summary>
        [NotMapped]
        [Display(Name = "Billing")]
        public string DisplayName => $"{BillingNumber} - {Student?.FullName ?? "Unknown"}";

        /// <summary>
        /// Full display name with period information
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Billing Info")]
        public string FullDisplayName => $"{BillingNumber} - {Student?.FullName} ({BillingPeriod?.PeriodName})";

        /// <summary>
        /// Status display with color coding information
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
            "Outstanding" => "warning",
            "Paid" => "success",
            "Overdue" => "danger",
            "Cancelled" => "secondary",
            _ => "info"
        };

        /// <summary>
        /// Payment method display
        /// </summary>
        [NotMapped]
        [Display(Name = "Payment Method")]
        public string PaymentMethodDisplay => PaymentMethod ?? "Not Specified";

        /// <summary>
        /// Days since billing was created
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Billing")]
        public int DaysSinceBilling => (DateTime.Now - CreatedDate).Days;

        /// <summary>
        /// Days until due date (negative if overdue)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Due")]
        public int DaysUntilDue => (DueDate.Date - DateTime.Today).Days;

        /// <summary>
        /// Days since payment (if paid)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Payment")]
        public int? DaysSincePayment => PaymentDate.HasValue ? (DateTime.Now.Date - PaymentDate.Value.Date).Days : null;

        /// <summary>
        /// Subtotal before discount and tax
        /// </summary>
        [NotMapped]
        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal => TuitionFee + BookFees + OtherFees;

        /// <summary>
        /// Amount after discount but before tax
        /// </summary>
        [NotMapped]
        [Display(Name = "Amount After Discount")]
        [DataType(DataType.Currency)]
        public decimal AmountAfterDiscount => Subtotal - Discount;

        /// <summary>
        /// Remaining balance (if partially paid)
        /// </summary>
        [NotMapped]
        [Display(Name = "Remaining Balance")]
        [DataType(DataType.Currency)]
        public decimal RemainingBalance => TotalAmount - (PaymentAmount ?? 0);

        /// <summary>
        /// Payment percentage of total amount
        /// </summary>
        [NotMapped]
        [Display(Name = "Payment Percentage")]
        [DisplayFormat(DataFormatString = "{0:P1}", ApplyFormatInEditMode = true)]
        public decimal PaymentPercentage => TotalAmount > 0 ? (PaymentAmount ?? 0) / TotalAmount : 0;

        /// <summary>
        /// Discount percentage of subtotal
        /// </summary>
        [NotMapped]
        [Display(Name = "Discount Percentage")]
        [DisplayFormat(DataFormatString = "{0:P1}", ApplyFormatInEditMode = true)]
        public decimal DiscountPercentage => Subtotal > 0 ? Discount / Subtotal : 0;

        /// <summary>
        /// Tax percentage of discounted amount
        /// </summary>
        [NotMapped]
        [Display(Name = "Tax Percentage")]
        [DisplayFormat(DataFormatString = "{0:P1}", ApplyFormatInEditMode = true)]
        public decimal TaxPercentage => AmountAfterDiscount > 0 ? Tax / AmountAfterDiscount : 0;

        /// <summary>
        /// Check if billing is overdue
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Overdue")]
        public bool IsOverdue => Status == "Overdue" || (Status == "Outstanding" && DateTime.Today > DueDate.Date);

        /// <summary>
        /// Check if billing is paid in full
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Paid In Full")]
        public bool IsPaidInFull => Status == "Paid" && PaymentAmount >= TotalAmount;

        /// <summary>
        /// Check if billing is partially paid
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Partially Paid")]
        public bool IsPartiallyPaid => PaymentAmount.HasValue && PaymentAmount > 0 && PaymentAmount < TotalAmount;

        /// <summary>
        /// Check if payment is due soon (within 3 days)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Due Soon")]
        public bool IsDueSoon => Status == "Outstanding" && DaysUntilDue >= 0 && DaysUntilDue <= 3;

        /// <summary>
        /// Priority level for collections (1 = highest priority)
        /// </summary>
        [NotMapped]
        [Display(Name = "Collection Priority")]
        public int CollectionPriority
        {
            get
            {
                if (IsOverdue && TotalAmount > 1000) return 1; // High amount overdue
                if (IsOverdue) return 2; // Regular overdue
                if (IsDueSoon && TotalAmount > 1000) return 3; // High amount due soon
                if (IsDueSoon) return 4; // Due soon
                if (IsPartiallyPaid) return 5; // Partially paid
                return 6; // Outstanding but not urgent
            }
        }

        /// <summary>
        /// Collection priority display
        /// </summary>
        [NotMapped]
        [Display(Name = "Collection Priority")]
        public string CollectionPriorityDisplay => CollectionPriority switch
        {
            1 => "Critical",
            2 => "High",
            3 => "Medium-High",
            4 => "Medium",
            5 => "Low-Medium",
            _ => "Low"
        };

        /// <summary>
        /// Due date status display
        /// </summary>
        [NotMapped]
        [Display(Name = "Due Status")]
        public string DueStatusDisplay
        {
            get
            {
                if (Status == "Paid") return "Paid";
                if (IsOverdue) return $"Overdue by {Math.Abs(DaysUntilDue)} day{(Math.Abs(DaysUntilDue) != 1 ? "s" : "")}";
                if (DaysUntilDue == 0) return "Due Today";
                if (IsDueSoon) return $"Due in {DaysUntilDue} day{(DaysUntilDue != 1 ? "s" : "")}";
                return $"Due in {DaysUntilDue} day{(DaysUntilDue != 1 ? "s" : "")}";
            }
        }

        /// <summary>
        /// Payment summary display
        /// </summary>
        [NotMapped]
        [Display(Name = "Payment Summary")]
        public string PaymentSummaryDisplay
        {
            get
            {
                if (IsPaidInFull) return $"Paid in Full ({PaymentAmount:C})";
                if (IsPartiallyPaid) return $"Partial Payment ({PaymentAmount:C} of {TotalAmount:C})";
                return $"Unpaid ({TotalAmount:C})";
            }
        }

        /// <summary>
        /// Academic period display
        /// </summary>
        [NotMapped]
        [Display(Name = "Academic Period")]
        public string AcademicPeriodDisplay => BillingPeriod?.PeriodName ?? "Unknown Period";

        /// <summary>
        /// Fee breakdown display
        /// </summary>
        [NotMapped]
        [Display(Name = "Fee Breakdown")]
        public string FeeBreakdownDisplay
        {
            get
            {
                var parts = new List<string>();
                if (TuitionFee > 0) parts.Add($"Tuition: {TuitionFee:C}");
                if (BookFees > 0) parts.Add($"Books: {BookFees:C}");
                if (OtherFees > 0) parts.Add($"Other: {OtherFees:C}");
                if (Discount > 0) parts.Add($"Discount: -{Discount:C}");
                if (Tax > 0) parts.Add($"Tax: {Tax:C}");
                return string.Join(", ", parts);
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Check if billing number is unique within the same billing period and site
        /// </summary>
        /// <param name="otherBillings">Other billings to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsBillingNumberUnique(IEnumerable<StudentBilling> otherBillings)
        {
            return !otherBillings.Any(b =>
                b.BillingId != BillingId &&
                b.BillingNumber.ToUpper() == BillingNumber.ToUpper() &&
                b.BillingPeriodId == BillingPeriodId &&
                b.SiteId == SiteId);
        }

        /// <summary>
        /// Calculate total amount from all components
        /// </summary>
        /// <returns>Calculated total amount</returns>
        public decimal CalculateTotalAmount()
        {
            return TuitionFee + BookFees + OtherFees - Discount + Tax;
        }

        /// <summary>
        /// Apply discount to billing
        /// </summary>
        /// <param name="discountAmount">Discount amount</param>
        /// <param name="discountReason">Reason for discount</param>
        /// <param name="appliedBy">User who applied discount</param>
        public void ApplyDiscount(decimal discountAmount, string discountReason, int appliedBy)
        {
            if (discountAmount < 0 || discountAmount > Subtotal)
                throw new ArgumentException("Invalid discount amount");

            Discount = discountAmount;
            Notes = string.IsNullOrEmpty(Notes)
                ? $"Discount applied: {discountReason}"
                : $"{Notes}; Discount: {discountReason}";

            UpdatedBy = appliedBy;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Process payment for this billing
        /// </summary>
        /// <param name="paymentAmount">Amount paid</param>
        /// <param name="paymentMethod">Method of payment</param>
        /// <param name="paymentReference">Payment reference/transaction ID</param>
        /// <param name="processedBy">User who processed payment</param>
        public void ProcessPayment(decimal paymentAmount, string paymentMethod, string? paymentReference, int processedBy)
        {
            if (paymentAmount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero");

            if (Status == "Paid")
                throw new InvalidOperationException("Billing is already paid");

            PaymentAmount = (PaymentAmount ?? 0) + paymentAmount;
            PaymentDate = DateTime.Today;
            PaymentMethod = paymentMethod;
            PaymentReference = paymentReference;

            // Update status based on payment amount
            if (PaymentAmount >= TotalAmount)
            {
                Status = "Paid";
            }
            else
            {
                Status = "Outstanding"; // Partial payment
            }

            UpdatedBy = processedBy;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Mark billing as overdue
        /// </summary>
        /// <param name="updatedBy">User who marked as overdue</param>
        public void MarkAsOverdue(int updatedBy)
        {
            if (Status == "Outstanding" && DateTime.Today > DueDate.Date)
            {
                Status = "Overdue";
                UpdatedBy = updatedBy;
                UpdatedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Cancel billing
        /// </summary>
        /// <param name="reason">Reason for cancellation</param>
        /// <param name="cancelledBy">User who cancelled</param>
        public void CancelBilling(string reason, int cancelledBy)
        {
            if (Status == "Paid")
                throw new InvalidOperationException("Cannot cancel paid billing");

            Status = "Cancelled";
            Notes = string.IsNullOrEmpty(Notes)
                ? $"Cancelled: {reason}"
                : $"{Notes}; Cancelled: {reason}";

            UpdatedBy = cancelledBy;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Print receipt
        /// </summary>
        /// <param name="printedBy">User who printed receipt</param>
        public void PrintReceipt(int printedBy)
        {
            ReceiptPrinted = true;
            ReceiptPrintCount++;
            LastPrintDate = DateTime.Now;
            UpdatedBy = printedBy;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Send reminder email
        /// </summary>
        /// <param name="reminderType">Type of reminder (Due Soon, Overdue, etc.)</param>
        /// <returns>True if reminder should be sent</returns>
        public bool ShouldSendReminder(string reminderType)
        {
            return reminderType.ToLower() switch
            {
                "due_soon" => IsDueSoon && Status == "Outstanding",
                "overdue" => IsOverdue,
                "payment_received" => Status == "Paid" && PaymentDate >= DateTime.Today.AddDays(-1),
                _ => false
            };
        }

        /// <summary>
        /// Generate billing statement data
        /// </summary>
        /// <returns>Complete billing statement information</returns>
        public Dictionary<string, object> GenerateBillingStatement()
        {
            return new Dictionary<string, object>
            {
                {"Header", new
                {
                    BillingNumber = BillingNumber,
                    BillingDate = BillingDate,
                    DueDate = DueDate,
                    Company = Company?.CompanyName,
                    Site = Site?.SiteName
                }},
                {"Student", new
                {
                    StudentCode = Student?.StudentCode,
                    StudentName = Student?.FullName,
                    Grade = Grade?.GradeName,
                    ParentName = Student?.ParentName,
                    Address = Student?.FullAddress
                }},
                {"Period", new
                {
                    PeriodName = BillingPeriod?.PeriodName,
                    StartDate = BillingPeriod?.StartDate,
                    EndDate = BillingPeriod?.EndDate
                }},
                {"Charges", new
                {
                    TuitionFee = TuitionFee,
                    BookFees = BookFees,
                    OtherFees = OtherFees,
                    Subtotal = Subtotal,
                    Discount = Discount,
                    DiscountPercentage = DiscountPercentage,
                    AmountAfterDiscount = AmountAfterDiscount,
                    Tax = Tax,
                    TaxPercentage = TaxPercentage,
                    TotalAmount = TotalAmount
                }},
                {"Payment", new
                {
                    Status = Status,
                    PaymentAmount = PaymentAmount,
                    PaymentDate = PaymentDate,
                    PaymentMethod = PaymentMethod,
                    PaymentReference = PaymentReference,
                    RemainingBalance = RemainingBalance,
                    PaymentPercentage = PaymentPercentage
                }},
                {"Notes", Notes},
                {"GeneratedDate", DateTime.Now}
            };
        }

        /// <summary>
        /// Generate billing summary for dashboard
        /// </summary>
        /// <returns>Billing summary data</returns>
        public Dictionary<string, object> GetBillingSummary()
        {
            return new Dictionary<string, object>
            {
                {"BillingId", BillingId},
                {"BillingNumber", BillingNumber},
                {"Student", new { Student?.StudentId, Student?.StudentCode, Student?.FullName }},
                {"Period", BillingPeriod?.PeriodName},
                {"Grade", Grade?.GradeName},
                {"Amount", new
                {
                    TotalAmount = TotalAmount,
                    PaymentAmount = PaymentAmount,
                    RemainingBalance = RemainingBalance
                }},
                {"Dates", new
                {
                    BillingDate = BillingDate,
                    DueDate = DueDate,
                    PaymentDate = PaymentDate,
                    DaysUntilDue = DaysUntilDue,
                    DaysSincePayment = DaysSincePayment
                }},
                {"Status", new
                {
                    Status = Status,
                    StatusColor = StatusColor,
                    IsOverdue = IsOverdue,
                    IsDueSoon = IsDueSoon,
                    IsPaidInFull = IsPaidInFull,
                    IsPartiallyPaid = IsPartiallyPaid,
                    CollectionPriority = CollectionPriority,
                    DueStatusDisplay = DueStatusDisplay
                }}
            };
        }

        /// <summary>
        /// Get billing statistics for a collection of billings
        /// </summary>
        /// <param name="billings">Billings to analyze</param>
        /// <returns>Statistical summary</returns>
        public static Dictionary<string, object> GetBillingStatistics(IEnumerable<StudentBilling> billings)
        {
            var billingList = billings.ToList();
            var paidBillings = billingList.Where(b => b.Status == "Paid").ToList();
            var outstandingBillings = billingList.Where(b => b.Status == "Outstanding").ToList();
            var overdueBillings = billingList.Where(b => b.IsOverdue).ToList();

            return new Dictionary<string, object>
            {
                {"Total", billingList.Count},
                {"TotalAmount", billingList.Sum(b => b.TotalAmount)},
                {"Paid", new
                {
                    Count = paidBillings.Count,
                    Amount = paidBillings.Sum(b => b.PaymentAmount ?? 0),
                    Percentage = billingList.Count > 0 ? (double)paidBillings.Count / billingList.Count : 0
                }},
                {"Outstanding", new
                {
                    Count = outstandingBillings.Count,
                    Amount = outstandingBillings.Sum(b => b.TotalAmount),
                    Percentage = billingList.Count > 0 ? (double)outstandingBillings.Count / billingList.Count : 0
                }},
                {"Overdue", new
                {
                    Count = overdueBillings.Count,
                    Amount = overdueBillings.Sum(b => b.TotalAmount),
                    Percentage = billingList.Count > 0 ? (double)overdueBillings.Count / billingList.Count : 0
                }},
                {"AverageAmount", billingList.Any() ? billingList.Average(b => b.TotalAmount) : 0},
                {"CollectionMetrics", new
                {
                    CollectionRate = billingList.Count > 0 ? (double)paidBillings.Count / billingList.Count : 0,
                    AveragePaymentDays = paidBillings.Where(b => b.PaymentDate.HasValue)
                        .Average(b => (b.PaymentDate!.Value - b.BillingDate).Days),
                    HighPriorityCount = billingList.Count(b => b.CollectionPriority <= 2)
                }}
            };
        }

        /// <summary>
        /// Get overdue billings that need attention
        /// </summary>
        /// <param name="billings">All billings to check</param>
        /// <param name="priorityThreshold">Priority threshold (1-6)</param>
        /// <returns>Overdue billings requiring attention</returns>
        public static List<StudentBilling> GetOverdueBillingsRequiringAttention(IEnumerable<StudentBilling> billings,
            int priorityThreshold = 3)
        {
            return billings
                .Where(b => b.IsOverdue && b.CollectionPriority <= priorityThreshold)
                .OrderBy(b => b.CollectionPriority)
                .ThenByDescending(b => b.TotalAmount)
                .ToList();
        }

        /// <summary>
        /// Generate revenue report for a period
        /// </summary>
        /// <param name="billings">Billings to analyze</param>
        /// <param name="periodStart">Period start date</param>
        /// <param name="periodEnd">Period end date</param>
        /// <returns>Revenue report data</returns>
        public static Dictionary<string, object> GenerateRevenueReport(IEnumerable<StudentBilling> billings,
            DateTime periodStart, DateTime periodEnd)
        {
            var periodBillings = billings
                .Where(b => b.PaymentDate >= periodStart && b.PaymentDate <= periodEnd)
                .ToList();

            var revenueBySource = new Dictionary<string, decimal>
            {
                {"TuitionRevenue", periodBillings.Sum(b => b.TuitionFee * (b.PaymentPercentage / 100))},
                {"BookRevenue", periodBillings.Sum(b => b.BookFees * (b.PaymentPercentage / 100))},
                {"OtherRevenue", periodBillings.Sum(b => b.OtherFees * (b.PaymentPercentage / 100))}
            };

            return new Dictionary<string, object>
            {
                {"Period", new { Start = periodStart, End = periodEnd }},
                {"TotalRevenue", periodBillings.Sum(b => b.PaymentAmount ?? 0)},
                {"BillingsCount", periodBillings.Count},
                {"AveragePayment", periodBillings.Any() ? periodBillings.Average(b => b.PaymentAmount ?? 0) : 0},
                {"RevenueBySource", revenueBySource},
                {"PaymentMethods", periodBillings.GroupBy(b => b.PaymentMethod ?? "Unknown")
                    .ToDictionary(g => g.Key, g => new { Count = g.Count(), Amount = g.Sum(b => b.PaymentAmount ?? 0) })},
                {"MonthlyBreakdown", periodBillings.GroupBy(b => new { b.PaymentDate!.Value.Year, b.PaymentDate.Value.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:00}",
                        g => new { Count = g.Count(), Amount = g.Sum(b => b.PaymentAmount ?? 0) })}
            };
        }

        /// <summary>
        /// Get aging report for outstanding billings
        /// </summary>
        /// <param name="billings">Outstanding billings</param>
        /// <returns>Aging report data</returns>
        public static Dictionary<string, object> GetAgingReport(IEnumerable<StudentBilling> billings)
        {
            var outstandingBillings = billings.Where(b => b.Status == "Outstanding" || b.IsOverdue).ToList();

            var agingBuckets = new Dictionary<string, List<StudentBilling>>
            {
                {"Current", outstandingBillings.Where(b => b.DaysUntilDue >= 0).ToList()},
                {"1-30 Days", outstandingBillings.Where(b => b.DaysUntilDue >= -30 && b.DaysUntilDue < 0).ToList()},
                {"31-60 Days", outstandingBillings.Where(b => b.DaysUntilDue >= -60 && b.DaysUntilDue < -30).ToList()},
                {"61-90 Days", outstandingBillings.Where(b => b.DaysUntilDue >= -90 && b.DaysUntilDue < -60).ToList()},
                {"Over 90 Days", outstandingBillings.Where(b => b.DaysUntilDue < -90).ToList()}
            };

            return new Dictionary<string, object>
            {
                {"TotalOutstanding", outstandingBillings.Sum(b => b.RemainingBalance)},
                {"TotalCount", outstandingBillings.Count},
                {"AgingBuckets", agingBuckets.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new
                    {
                        Count = kvp.Value.Count,
                        Amount = kvp.Value.Sum(b => b.RemainingBalance),
                        Percentage = outstandingBillings.Count > 0 ? (double)kvp.Value.Count / outstandingBillings.Count : 0
                    }
                )},
                {"CriticalAccounts", outstandingBillings.Where(b => b.CollectionPriority <= 2)
                    .Select(b => new
                    {
                        b.BillingNumber,
                        Student = b.Student?.FullName,
                        b.TotalAmount,
                        b.DaysUntilDue,
                        b.CollectionPriorityDisplay
                    }).ToList()}
            };
        }

        /// <summary>
        /// Generate next billing number for a student and period
        /// </summary>
        /// <param name="studentCode">Student code</param>
        /// <param name="billingPeriod">Billing period</param>
        /// <param name="siteCode">Site code</param>
        /// <param name="existingBillings">Existing billing numbers</param>
        /// <returns>Next billing number</returns>
        public static string GenerateBillingNumber(string studentCode, BillingPeriod billingPeriod,
            string siteCode, IEnumerable<string> existingBillings)
        {
            var periodCode = billingPeriod.StartDate.ToString("yyyyMM");
            var prefix = $"BILL-{siteCode}-{periodCode}-";

            var existingNumbers = existingBillings
                .Where(b => b.StartsWith(prefix))
                .Select(b =>
                {
                    var parts = b.Split('-');
                    return parts.Length == 4 && int.TryParse(parts[3], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingNumbers.Max() + 1;
            return $"{prefix}{nextNumber:0000}";
        }

        /// <summary>
        /// Calculate late fees for overdue billing
        /// </summary>
        /// <param name="lateFeePercentage">Late fee percentage (e.g., 0.05 for 5%)</param>
        /// <param name="maxLateFee">Maximum late fee amount</param>
        /// <returns>Calculated late fee amount</returns>
        public decimal CalculateLateFee(decimal lateFeePercentage = 0.05m, decimal maxLateFee = 100m)
        {
            if (!IsOverdue) return 0;

            var lateFee = TotalAmount * lateFeePercentage;
            return Math.Min(lateFee, maxLateFee);
        }

        /// <summary>
        /// Get payment plan options for this billing
        /// </summary>
        /// <param name="numberOfInstallments">Number of installments (2-12)</param>
        /// <returns>Payment plan options</returns>
        public Dictionary<string, object> GetPaymentPlanOptions(int numberOfInstallments = 3)
        {
            if (numberOfInstallments < 2 || numberOfInstallments > 12)
                throw new ArgumentException("Number of installments must be between 2 and 12");

            var remainingAmount = RemainingBalance;
            var installmentAmount = Math.Ceiling(remainingAmount / numberOfInstallments);
            var lastInstallmentAmount = remainingAmount - (installmentAmount * (numberOfInstallments - 1));

            var installments = new List<object>();
            for (int i = 1; i <= numberOfInstallments; i++)
            {
                var dueDate = DueDate.AddMonths(i - 1);
                var amount = i == numberOfInstallments ? lastInstallmentAmount : installmentAmount;

                installments.Add(new
                {
                    InstallmentNumber = i,
                    DueDate = dueDate,
                    Amount = amount,
                    IsOverdue = dueDate < DateTime.Today
                });
            }

            return new Dictionary<string, object>
            {
                {"TotalAmount", remainingAmount},
                {"NumberOfInstallments", numberOfInstallments},
                {"RegularInstallmentAmount", installmentAmount},
                {"LastInstallmentAmount", lastInstallmentAmount},
                {"Installments", installments},
                {"TotalPlanDuration", $"{numberOfInstallments} months"},
                {"FirstPaymentDue", DueDate},
                {"LastPaymentDue", DueDate.AddMonths(numberOfInstallments - 1)}
            };
        }

        /// <summary>
        /// Send automated reminders based on billing status
        /// </summary>
        /// <param name="billings">Billings to check for reminders</param>
        /// <returns>List of billings requiring reminders</returns>
        public static List<object> GetBillingsRequiringReminders(IEnumerable<StudentBilling> billings)
        {
            var reminders = new List<object>();

            foreach (var billing in billings.Where(b => b.Status == "Outstanding" || b.IsOverdue))
            {
                var reminderTypes = new List<string>();
                var priority = "Normal";

                if (billing.IsOverdue)
                {
                    reminderTypes.Add("Overdue Payment");
                    priority = billing.CollectionPriority <= 2 ? "High" : "Medium";
                }
                else if (billing.IsDueSoon)
                {
                    reminderTypes.Add("Payment Due Soon");
                }

                if (billing.IsPartiallyPaid)
                {
                    reminderTypes.Add("Partial Payment Received");
                }

                if (reminderTypes.Any())
                {
                    reminders.Add(new
                    {
                        BillingId = billing.BillingId,
                        BillingNumber = billing.BillingNumber,
                        Student = new { billing.Student?.StudentId, billing.Student?.FullName, billing.Student?.ParentEmail },
                        Amount = billing.TotalAmount,
                        RemainingBalance = billing.RemainingBalance,
                        DueDate = billing.DueDate,
                        DaysUntilDue = billing.DaysUntilDue,
                        ReminderTypes = reminderTypes,
                        Priority = priority,
                        ContactPreference = billing.Student?.ContactPreference
                    });
                }
            }

            return reminders.OrderBy(r => ((dynamic)r).Priority == "High" ? 1 :
                                          ((dynamic)r).Priority == "Medium" ? 2 : 3)
                           .ThenBy(r => ((dynamic)r).DaysUntilDue)
                           .ToList();
        }

        /// <summary>
        /// Validate student billing business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateStudentBillingRules()
        {
            var errors = new List<string>();

            // Due date must be after billing date
            if (DueDate <= BillingDate)
            {
                errors.Add("Due date must be after billing date");
            }

            // Payment date should not be before billing date
            if (PaymentDate.HasValue && PaymentDate.Value.Date < BillingDate.Date)
            {
                errors.Add("Payment date cannot be before billing date");
            }

            // Payment amount validation
            if (PaymentAmount.HasValue && PaymentAmount.Value > TotalAmount * 1.1m) // Allow 10% overpayment
            {
                errors.Add($"Payment amount ({PaymentAmount:C}) significantly exceeds total amount ({TotalAmount:C})");
            }

            // Status consistency
            if (Status == "Paid" && (!PaymentAmount.HasValue || PaymentAmount < TotalAmount * 0.95m)) // Allow 5% underpayment
            {
                errors.Add("Paid billings must have payment amount close to total amount");
            }

            if (Status == "Paid" && !PaymentDate.HasValue)
            {
                errors.Add("Paid billings must have payment date");
            }

            // Discount validation
            if (Discount > Subtotal)
            {
                errors.Add($"Discount ({Discount:C}) cannot exceed subtotal ({Subtotal:C})");
            }

            // Fee validation
            if (TuitionFee <= 0 && BookFees <= 0 && OtherFees <= 0)
            {
                errors.Add("At least one fee component must be greater than zero");
            }

            return errors;
        }

        /// <summary>
        /// Get billing health score (0-100)
        /// </summary>
        /// <returns>Health score based on payment history and status</returns>
        public int GetBillingHealthScore()
        {
            var score = 100;

            // Deduct points for overdue status
            if (IsOverdue)
            {
                score -= Math.Min(50, Math.Abs(DaysUntilDue) * 2); // Max 50 points deduction
            }

            // Deduct points for partial payment
            if (IsPartiallyPaid)
            {
                score -= 20;
            }

            // Add points for early payment
            if (Status == "Paid" && PaymentDate < DueDate)
            {
                score += 10;
            }

            // Deduct points for high amount outstanding
            if (TotalAmount > 1000)
            {
                score -= 10;
            }

            return Math.Max(0, Math.Min(100, score));
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the billing</returns>
        public override string ToString() => DisplayName;
    }
}
