using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a student registration/trial record in the KMSI Course Management System
    /// Handles the registration process from initial sign-up through trial to confirmation
    /// </summary>
    [Table("Registrations")]
    public class Registration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RegistrationId { get; set; }

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

        [Required(ErrorMessage = "Registration code is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Registration code must be between 3 and 20 characters")]
        [Display(Name = "Registration Code")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Registration code must contain only uppercase letters, numbers, and hyphens")]
        public string RegistrationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Registration date is required")]
        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        [Display(Name = "Trial Date")]
        [DataType(DataType.Date)]
        public DateTime? TrialDate { get; set; }

        [Display(Name = "Trial Time")]
        [DataType(DataType.Time)]
        public TimeSpan? TrialTime { get; set; }

        [Display(Name = "Assigned Teacher")]
        [ForeignKey("AssignedTeacher")]
        public int? AssignedTeacherId { get; set; }

        [Required(ErrorMessage = "Requested grade is required")]
        [Display(Name = "Requested Grade")]
        [ForeignKey("RequestedGrade")]
        public int RequestedGradeId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Trial Scheduled, Trial Done, Confirmed, Cancelled

        [StringLength(20, ErrorMessage = "Trial result cannot exceed 20 characters")]
        [Display(Name = "Trial Result")]
        public string? TrialResult { get; set; } // Pass, Fail, Need Assessment

        [Display(Name = "Confirmation Date")]
        [DataType(DataType.Date)]
        public DateTime? ConfirmationDate { get; set; }

        [Required(ErrorMessage = "Payment status is required")]
        [StringLength(20, ErrorMessage = "Payment status cannot exceed 20 characters")]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Cancelled

        [Range(0, 999999999.99, ErrorMessage = "Payment amount must be between 0 and 999,999,999.99")]
        [Display(Name = "Payment Amount")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? PaymentAmount { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
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
        /// Company that handles this registration
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where the registration is processed
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Student who is registering
        /// </summary>
        [Required]
        [Display(Name = "Student")]
        public virtual Student Student { get; set; } = null!;

        /// <summary>
        /// Teacher assigned for the trial/class
        /// </summary>
        [Display(Name = "Assigned Teacher")]
        public virtual Teacher? AssignedTeacher { get; set; }

        /// <summary>
        /// Grade level requested by the student
        /// </summary>
        [Required]
        [Display(Name = "Requested Grade")]
        public virtual Grade RequestedGrade { get; set; } = null!;

        /// <summary>
        /// User who created this registration record
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this registration record
        /// </summary>
        [Display(Name = "Updated By")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI combining student name and registration code
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{Student?.FullName} - {RegistrationCode}";

        /// <summary>
        /// Formatted trial date and time for display
        /// </summary>
        [NotMapped]
        [Display(Name = "Trial Schedule")]
        public string TrialScheduleDisplay
        {
            get
            {
                if (!TrialDate.HasValue) return "Not Scheduled";

                var dateStr = TrialDate.Value.ToString("dd/MM/yyyy");
                var timeStr = TrialTime?.ToString(@"hh\:mm") ?? "Time TBD";
                return $"{dateStr} at {timeStr}";
            }
        }

        /// <summary>
        /// Status display with color coding information
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => Status switch
        {
            "Pending" => "⏳ Pending",
            "Trial Scheduled" => "📅 Trial Scheduled",
            "Trial Done" => "✅ Trial Completed",
            "Confirmed" => "🎉 Confirmed",
            "Cancelled" => "❌ Cancelled",
            _ => Status
        };

        /// <summary>
        /// Payment status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Payment Status Display")]
        public string PaymentStatusDisplay => PaymentStatus switch
        {
            "Pending" => "⏳ Payment Pending",
            "Paid" => "💰 Paid",
            "Cancelled" => "❌ Cancelled",
            _ => PaymentStatus
        };

        /// <summary>
        /// Trial result display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Trial Result Display")]
        public string TrialResultDisplay => TrialResult switch
        {
            "Pass" => "✅ Pass",
            "Fail" => "❌ Fail",
            "Need Assessment" => "⚠️ Need Assessment",
            _ => TrialResult ?? "Not Evaluated"
        };

        /// <summary>
        /// Indicates if the registration is still active (not cancelled)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Active")]
        public bool IsActive => Status != "Cancelled";

        /// <summary>
        /// Indicates if trial has been completed
        /// </summary>
        [NotMapped]
        [Display(Name = "Trial Completed")]
        public bool IsTrialCompleted => Status == "Trial Done" || Status == "Confirmed";

        /// <summary>
        /// Indicates if registration is confirmed and student can start classes
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Confirmed")]
        public bool IsConfirmed => Status == "Confirmed";

        /// <summary>
        /// Days since registration
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Registration")]
        public int DaysSinceRegistration => (DateTime.Today - RegistrationDate.Date).Days;

        // Business Logic Methods

        /// <summary>
        /// Generate next registration number for a site
        /// </summary>
        /// <param name="siteCode">Site code</param>
        /// <param name="registrationDate">Registration date</param>
        /// <param name="existingRegistrations">Existing registration codes</param>
        /// <returns>Next registration code</returns>
        public static string GenerateRegistrationCode(string siteCode, DateTime registrationDate, IEnumerable<string> existingRegistrations)
        {
            var dateCode = registrationDate.ToString("yyMM");
            var prefix = $"REG-{siteCode}-{dateCode}-";

            var existingNumbers = existingRegistrations
                .Where(r => r.StartsWith(prefix))
                .Select(r =>
                {
                    var parts = r.Split('-');
                    return parts.Length == 4 && int.TryParse(parts[3], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingNumbers.Max() + 1;
            return $"{prefix}{nextNumber:000}";
        }

        /// <summary>
        /// Validate registration business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateRegistrationRules()
        {
            var errors = new List<string>();

            // Registration date should not be in the future
            if (RegistrationDate.Date > DateTime.Today)
            {
                errors.Add("Registration date cannot be in the future");
            }

            // Trial date should not be before registration date
            if (TrialDate.HasValue && TrialDate.Value.Date < RegistrationDate.Date)
            {
                errors.Add("Trial date cannot be before registration date");
            }

            // Confirmation date should not be before trial date
            if (ConfirmationDate.HasValue && TrialDate.HasValue &&
                ConfirmationDate.Value.Date < TrialDate.Value.Date)
            {
                errors.Add("Confirmation date cannot be before trial date");
            }

            // Confirmed registrations must have payment amount
            if (Status == "Confirmed" && (!PaymentAmount.HasValue || PaymentAmount.Value <= 0))
            {
                errors.Add("Confirmed registrations must have a valid payment amount");
            }

            // Paid registrations must have confirmation date
            if (PaymentStatus == "Paid" && !ConfirmationDate.HasValue)
            {
                errors.Add("Paid registrations must have confirmation date");
            }

            // Trial result validation
            if (Status == "Trial Done" && string.IsNullOrEmpty(TrialResult))
            {
                errors.Add("Trial result must be specified for completed trials");
            }

            // Assigned teacher required for scheduled trials
            if (Status == "Trial Scheduled" && !AssignedTeacherId.HasValue)
            {
                errors.Add("Assigned teacher is required for scheduled trials");
            }

            return errors;
        }

        /// <summary>
        /// Get available status transitions based on current status
        /// </summary>
        /// <returns>List of valid next statuses</returns>
        public List<string> GetAvailableStatusTransitions()
        {
            return Status switch
            {
                "Pending" => new List<string> { "Trial Scheduled", "Cancelled" },
                "Trial Scheduled" => new List<string> { "Trial Done", "Cancelled" },
                "Trial Done" => new List<string> { "Confirmed", "Cancelled" },
                "Confirmed" => new List<string> { "Cancelled" },
                "Cancelled" => new List<string>(), // Terminal status
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Update status with validation
        /// </summary>
        /// <param name="newStatus">New status to set</param>
        /// <returns>True if status was updated successfully</returns>
        public bool UpdateStatus(string newStatus)
        {
            var availableTransitions = GetAvailableStatusTransitions();
            if (!availableTransitions.Contains(newStatus))
            {
                return false;
            }

            Status = newStatus;
            UpdatedDate = DateTime.Now;

            // Auto-set related fields based on status
            switch (newStatus)
            {
                case "Confirmed":
                    if (!ConfirmationDate.HasValue)
                        ConfirmationDate = DateTime.Today;
                    break;
                case "Cancelled":
                    PaymentStatus = "Cancelled";
                    break;
            }

            return true;
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the registration</returns>
        public override string ToString() => DisplayName;
    }
}
