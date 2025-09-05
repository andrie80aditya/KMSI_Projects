using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents an email log entry in the KMSI Course Management System
    /// Tracks all email communications sent through the system with delivery status and error handling
    /// </summary>
    [Table("EmailLogs")]
    public class EmailLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailLogId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Email Template")]
        [ForeignKey("EmailTemplate")]
        public int? EmailTemplateId { get; set; }

        [Required(ErrorMessage = "Recipient type is required")]
        [StringLength(20, ErrorMessage = "Recipient type cannot exceed 20 characters")]
        [Display(Name = "Recipient Type")]
        public string RecipientType { get; set; } = string.Empty; // Student, Parent, Teacher, Staff

        [Required(ErrorMessage = "Recipient ID is required")]
        [Display(Name = "Recipient ID")]
        public int RecipientId { get; set; }

        [Required(ErrorMessage = "Recipient email is required")]
        [StringLength(100, ErrorMessage = "Recipient email cannot exceed 100 characters")]
        [Display(Name = "Recipient Email")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string RecipientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(255, ErrorMessage = "Subject cannot exceed 255 characters")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "Body")]
        [DataType(DataType.MultilineText)]
        public string? Body { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Sent, Failed, Bounced

        [Display(Name = "Sent Date")]
        [DataType(DataType.DateTime)]
        public DateTime? SentDate { get; set; }

        [StringLength(500, ErrorMessage = "Error message cannot exceed 500 characters")]
        [Display(Name = "Error Message")]
        [DataType(DataType.MultilineText)]
        public string? ErrorMessage { get; set; }

        [Range(0, 10, ErrorMessage = "Retry count must be between 0 and 10")]
        [Display(Name = "Retry Count")]
        public int RetryCount { get; set; } = 0;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties

        /// <summary>
        /// Company that sent this email
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Email template used for this email (if any)
        /// </summary>
        [Display(Name = "Email Template")]
        public virtual EmailTemplate? EmailTemplate { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{Subject} → {RecipientEmail}";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplayName => $"{RecipientType}: {RecipientEmail}";

        /// <summary>
        /// Status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => Status switch
        {
            "Pending" => "⏳ Pending",
            "Sent" => "✅ Sent",
            "Failed" => "❌ Failed",
            "Bounced" => "📧 Bounced",
            _ => Status
        };

        /// <summary>
        /// Recipient type display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Recipient Type Display")]
        public string RecipientTypeDisplay => RecipientType switch
        {
            "Student" => "🎓 Student",
            "Parent" => "👨‍👩‍👧‍👦 Parent",
            "Teacher" => "👩‍🏫 Teacher",
            "Staff" => "👤 Staff",
            _ => $"👥 {RecipientType}"
        };

        /// <summary>
        /// Priority level based on recipient type and content
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority")]
        public string Priority
        {
            get
            {
                if (Subject.ToLower().Contains("urgent") || Subject.ToLower().Contains("overdue"))
                    return "High";

                if (RecipientType == "Teacher" || RecipientType == "Staff")
                    return "High";

                if (Subject.ToLower().Contains("reminder") || Subject.ToLower().Contains("billing"))
                    return "Medium";

                return "Normal";
            }
        }

        /// <summary>
        /// Indicates if email was successfully delivered
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Delivered")]
        public bool IsDelivered => Status == "Sent";

        /// <summary>
        /// Indicates if email failed to send
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Failed")]
        public bool IsFailed => Status == "Failed" || Status == "Bounced";

        /// <summary>
        /// Indicates if email is still pending
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Pending")]
        public bool IsPending => Status == "Pending";

        /// <summary>
        /// Indicates if email needs retry
        /// </summary>
        [NotMapped]
        [Display(Name = "Needs Retry")]
        public bool NeedsRetry => Status == "Failed" && RetryCount < 3;

        /// <summary>
        /// Days since email was created
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Created")]
        public int DaysSinceCreated => (DateTime.Today - CreatedDate.Date).Days;

        /// <summary>
        /// Hours since email was sent (if sent)
        /// </summary>
        [NotMapped]
        [Display(Name = "Hours Since Sent")]
        public int? HoursSinceSent => SentDate.HasValue ? (int)(DateTime.Now - SentDate.Value).TotalHours : null;

        /// <summary>
        /// Delivery time in minutes (from created to sent)
        /// </summary>
        [NotMapped]
        [Display(Name = "Delivery Time (Minutes)")]
        public int? DeliveryTimeMinutes => SentDate.HasValue ? (int)(SentDate.Value - CreatedDate).TotalMinutes : null;

        /// <summary>
        /// Email category based on subject content
        /// </summary>
        [NotMapped]
        [Display(Name = "Category")]
        public string Category
        {
            get
            {
                var subjectLower = Subject.ToLower();
                return subjectLower switch
                {
                    var s when s.Contains("billing") || s.Contains("payment") || s.Contains("invoice") => "Financial",
                    var s when s.Contains("trial") || s.Contains("registration") => "Enrollment",
                    var s when s.Contains("exam") || s.Contains("certificate") => "Assessment",
                    var s when s.Contains("schedule") || s.Contains("class") => "Scheduling",
                    var s when s.Contains("reminder") || s.Contains("overdue") => "Reminder",
                    var s when s.Contains("welcome") || s.Contains("confirmation") => "Welcome",
                    _ => "General"
                };
            }
        }

        /// <summary>
        /// Template name (if template was used)
        /// </summary>
        [NotMapped]
        [Display(Name = "Template Name")]
        public string? TemplateName => EmailTemplate?.TemplateName;

        /// <summary>
        /// Word count in body
        /// </summary>
        [NotMapped]
        [Display(Name = "Word Count")]
        public int WordCount => string.IsNullOrWhiteSpace(Body) ? 0 : Body.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

        /// <summary>
        /// Character count in body
        /// </summary>
        [NotMapped]
        [Display(Name = "Character Count")]
        public int CharacterCount => Body?.Length ?? 0;

        // Static Constants for Status and Recipient Types
        public static class EmailStatus
        {
            public const string Pending = "Pending";
            public const string Sent = "Sent";
            public const string Failed = "Failed";
            public const string Bounced = "Bounced";
        }

        public static class RecipientTypes
        {
            public const string Student = "Student";
            public const string Parent = "Parent";
            public const string Teacher = "Teacher";
            public const string Staff = "Staff";
        }

        // Business Logic Methods

        /// <summary>
        /// Validate email log business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateEmailLogRules()
        {
            var errors = new List<string>();

            // Email address validation
            if (string.IsNullOrWhiteSpace(RecipientEmail))
            {
                errors.Add("Recipient email is required");
            }
            else if (!IsValidEmail(RecipientEmail))
            {
                errors.Add("Invalid email address format");
            }

            // Subject validation
            if (string.IsNullOrWhiteSpace(Subject))
            {
                errors.Add("Subject is required");
            }
            else if (Subject.Length > 255)
            {
                errors.Add("Subject cannot exceed 255 characters");
            }

            // Status validation
            var validStatuses = new[] { EmailStatus.Pending, EmailStatus.Sent, EmailStatus.Failed, EmailStatus.Bounced };
            if (!validStatuses.Contains(Status))
            {
                errors.Add($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}");
            }

            // Recipient type validation
            var validRecipientTypes = new[] { RecipientTypes.Student, RecipientTypes.Parent, RecipientTypes.Teacher, RecipientTypes.Staff };
            if (!validRecipientTypes.Contains(RecipientType))
            {
                errors.Add($"Invalid recipient type. Must be one of: {string.Join(", ", validRecipientTypes)}");
            }

            // Sent date validation
            if (Status == EmailStatus.Sent && !SentDate.HasValue)
            {
                errors.Add("Sent emails must have a sent date");
            }

            if (SentDate.HasValue && SentDate.Value > DateTime.Now)
            {
                errors.Add("Sent date cannot be in the future");
            }

            // Retry count validation
            if (RetryCount < 0 || RetryCount > 10)
            {
                errors.Add("Retry count must be between 0 and 10");
            }

            return errors;
        }

        /// <summary>
        /// Validate email address format
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if valid</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Mark email as sent
        /// </summary>
        /// <param name="sentDate">Date when email was sent (optional, defaults to now)</param>
        /// <returns>True if successful</returns>
        public bool MarkAsSent(DateTime? sentDate = null)
        {
            if (Status != EmailStatus.Pending)
                return false;

            Status = EmailStatus.Sent;
            SentDate = sentDate ?? DateTime.Now;
            ErrorMessage = null; // Clear any previous error

            return true;
        }

        /// <summary>
        /// Mark email as failed
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="incrementRetry">Whether to increment retry count</param>
        /// <returns>True if successful</returns>
        public bool MarkAsFailed(string errorMessage, bool incrementRetry = true)
        {
            Status = EmailStatus.Failed;
            ErrorMessage = errorMessage?.Length > 500 ? errorMessage.Substring(0, 500) : errorMessage;

            if (incrementRetry)
            {
                RetryCount++;
            }

            return true;
        }

        /// <summary>
        /// Mark email as bounced
        /// </summary>
        /// <param name="bounceReason">Reason for bounce</param>
        /// <returns>True if successful</returns>
        public bool MarkAsBounced(string bounceReason)
        {
            Status = EmailStatus.Bounced;
            ErrorMessage = bounceReason?.Length > 500 ? bounceReason.Substring(0, 500) : bounceReason;

            return true;
        }

        /// <summary>
        /// Reset for retry
        /// </summary>
        /// <returns>True if email can be retried</returns>
        public bool ResetForRetry()
        {
            if (!NeedsRetry)
                return false;

            Status = EmailStatus.Pending;
            SentDate = null;
            // Keep error message for history but allow retry

            return true;
        }

        /// <summary>
        /// Get delivery status summary
        /// </summary>
        /// <returns>Dictionary with delivery status information</returns>
        public Dictionary<string, object> GetDeliveryStatus()
        {
            return new Dictionary<string, object>
            {
                {"EmailLogId", EmailLogId},
                {"Subject", Subject},
                {"RecipientEmail", RecipientEmail},
                {"RecipientType", RecipientType},
                {"Status", Status},
                {"IsDelivered", IsDelivered},
                {"IsFailed", IsFailed},
                {"IsPending", IsPending},
                {"NeedsRetry", NeedsRetry},
                {"RetryCount", RetryCount},
                {"CreatedDate", CreatedDate},
                {"SentDate", SentDate},
                {"DeliveryTimeMinutes", DeliveryTimeMinutes},
                {"ErrorMessage", ErrorMessage},
                {"TemplateName", TemplateName},
                {"Category", Category},
                {"Priority", Priority}
            };
        }

        /// <summary>
        /// Create a new email log entry
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="recipientType">Type of recipient</param>
        /// <param name="recipientId">Recipient ID</param>
        /// <param name="recipientEmail">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="templateId">Email template ID (optional)</param>
        /// <returns>New email log instance</returns>
        public static EmailLog CreateEmailLog(int companyId, string recipientType, int recipientId,
            string recipientEmail, string subject, string? body = null, int? templateId = null)
        {
            return new EmailLog
            {
                CompanyId = companyId,
                EmailTemplateId = templateId,
                RecipientType = recipientType,
                RecipientId = recipientId,
                RecipientEmail = recipientEmail,
                Subject = subject,
                Body = body,
                Status = EmailStatus.Pending,
                RetryCount = 0,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get email logs by status
        /// </summary>
        /// <param name="emailLogs">Collection of email logs</param>
        /// <param name="status">Status to filter by</param>
        /// <returns>Filtered email logs</returns>
        public static IEnumerable<EmailLog> GetByStatus(IEnumerable<EmailLog> emailLogs, string status)
        {
            return emailLogs.Where(el => el.Status == status);
        }

        /// <summary>
        /// Get email logs that need retry
        /// </summary>
        /// <param name="emailLogs">Collection of email logs</param>
        /// <returns>Email logs that need retry</returns>
        public static IEnumerable<EmailLog> GetRetryableEmails(IEnumerable<EmailLog> emailLogs)
        {
            return emailLogs.Where(el => el.NeedsRetry);
        }

        /// <summary>
        /// Calculate delivery statistics for a collection of emails
        /// </summary>
        /// <param name="emailLogs">Collection of email logs</param>
        /// <returns>Dictionary with statistics</returns>
        public static Dictionary<string, object> CalculateDeliveryStatistics(IEnumerable<EmailLog> emailLogs)
        {
            var logs = emailLogs.ToList();
            var totalEmails = logs.Count;

            if (totalEmails == 0)
            {
                return new Dictionary<string, object>
                {
                    {"TotalEmails", 0},
                    {"DeliveryRate", 0},
                    {"FailureRate", 0},
                    {"BounceRate", 0}
                };
            }

            var sentEmails = logs.Count(l => l.Status == EmailStatus.Sent);
            var failedEmails = logs.Count(l => l.Status == EmailStatus.Failed);
            var bouncedEmails = logs.Count(l => l.Status == EmailStatus.Bounced);
            var pendingEmails = logs.Count(l => l.Status == EmailStatus.Pending);

            var avgDeliveryTime = logs.Where(l => l.DeliveryTimeMinutes.HasValue)
                                     .Select(l => l.DeliveryTimeMinutes!.Value)
                                     .DefaultIfEmpty(0)
                                     .Average();

            return new Dictionary<string, object>
            {
                {"TotalEmails", totalEmails},
                {"SentEmails", sentEmails},
                {"FailedEmails", failedEmails},
                {"BouncedEmails", bouncedEmails},
                {"PendingEmails", pendingEmails},
                {"DeliveryRate", Math.Round((decimal)sentEmails / totalEmails * 100, 2)},
                {"FailureRate", Math.Round((decimal)failedEmails / totalEmails * 100, 2)},
                {"BounceRate", Math.Round((decimal)bouncedEmails / totalEmails * 100, 2)},
                {"AverageDeliveryTimeMinutes", Math.Round(avgDeliveryTime, 2)},
                {"RetryableEmails", logs.Count(l => l.NeedsRetry)}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the email log</returns>
        public override string ToString() => DisplayName;
    }
}
