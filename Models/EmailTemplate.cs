using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents an email template in the KMSI Course Management System
    /// Manages reusable email templates for various notifications and communications
    /// </summary>
    [Table("EmailTemplates")]
    public class EmailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailTemplateId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Template name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Template name must be between 3 and 100 characters")]
        [Display(Name = "Template Name")]
        public string TemplateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Template type is required")]
        [StringLength(50, ErrorMessage = "Template type cannot exceed 50 characters")]
        [Display(Name = "Template Type")]
        public string TemplateType { get; set; } = string.Empty; // Billing Reminder, Trial Confirmation, etc

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Subject must be between 5 and 255 characters")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Body is required")]
        [Display(Name = "Body")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; } = string.Empty;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

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
        /// Company that owns this email template
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// User who created this email template
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this email template
        /// </summary>
        [Display(Name = "Updated By")]
        public virtual User? UpdatedByUser { get; set; }

        /// <summary>
        /// Email logs using this template
        /// </summary>
        [Display(Name = "Email Logs")]
        public virtual ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{TemplateName} ({TemplateType})";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplayName => TemplateName;

        /// <summary>
        /// Status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => IsActive ? "🟢 Active" : "🔴 Inactive";

        /// <summary>
        /// Template type display with formatting
        /// </summary>
        [NotMapped]
        [Display(Name = "Template Type Display")]
        public string TemplateTypeDisplay => TemplateType switch
        {
            "Billing Reminder" => "💳 Billing Reminder",
            "Trial Confirmation" => "🎵 Trial Confirmation",
            "Registration Welcome" => "🎉 Registration Welcome",
            "Exam Notification" => "📝 Exam Notification",
            "Grade Progression" => "📈 Grade Progression",
            "Payment Confirmation" => "💰 Payment Confirmation",
            "Class Schedule" => "📅 Class Schedule",
            "Certificate Issued" => "🏆 Certificate Issued",
            _ => $"📧 {TemplateType}"
        };

        /// <summary>
        /// Template category based on type
        /// </summary>
        [NotMapped]
        [Display(Name = "Category")]
        public string Category
        {
            get
            {
                return TemplateType.ToLower() switch
                {
                    var type when type.Contains("billing") || type.Contains("payment") => "Financial",
                    var type when type.Contains("trial") || type.Contains("registration") => "Enrollment",
                    var type when type.Contains("exam") || type.Contains("certificate") => "Assessment",
                    var type when type.Contains("class") || type.Contains("schedule") => "Scheduling",
                    var type when type.Contains("grade") || type.Contains("progress") => "Academic",
                    _ => "General"
                };
            }
        }

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

        /// <summary>
        /// Available placeholders/variables in the template
        /// </summary>
        [NotMapped]
        [Display(Name = "Placeholders")]
        public List<string> Placeholders
        {
            get
            {
                var placeholders = new List<string>();
                if (!string.IsNullOrEmpty(Subject))
                {
                    placeholders.AddRange(ExtractPlaceholders(Subject));
                }
                if (!string.IsNullOrEmpty(Body))
                {
                    placeholders.AddRange(ExtractPlaceholders(Body));
                }
                return placeholders.Distinct().OrderBy(p => p).ToList();
            }
        }

        /// <summary>
        /// Count of emails sent using this template
        /// </summary>
        [NotMapped]
        [Display(Name = "Emails Sent Count")]
        public int EmailsSentCount => EmailLogs?.Count(el => el.Status == "Sent") ?? 0;

        /// <summary>
        /// Last time this template was used
        /// </summary>
        [NotMapped]
        [Display(Name = "Last Used")]
        public DateTime? LastUsed => EmailLogs?.Where(el => el.SentDate.HasValue).Max(el => el.SentDate);

        /// <summary>
        /// Template usage frequency indicator
        /// </summary>
        [NotMapped]
        [Display(Name = "Usage Frequency")]
        public string UsageFrequency
        {
            get
            {
                var count = EmailsSentCount;
                return count switch
                {
                    0 => "Never Used",
                    <= 5 => "Rarely Used",
                    <= 20 => "Occasionally Used",
                    <= 50 => "Frequently Used",
                    _ => "Very Frequently Used"
                };
            }
        }

        /// <summary>
        /// Indicates if template has placeholders
        /// </summary>
        [NotMapped]
        [Display(Name = "Has Placeholders")]
        public bool HasPlaceholders => Placeholders.Any();

        /// <summary>
        /// Template complexity based on length and placeholders
        /// </summary>
        [NotMapped]
        [Display(Name = "Complexity")]
        public string Complexity
        {
            get
            {
                var score = 0;
                score += WordCount > 100 ? 2 : 1;
                score += HasPlaceholders ? 2 : 0;
                score += Body.Contains("<html>") || Body.Contains("<div>") ? 2 : 0;

                return score switch
                {
                    <= 2 => "Simple",
                    <= 4 => "Moderate",
                    _ => "Complex"
                };
            }
        }

        // Static Template Type Constants
        public static class TemplateTypes
        {
            public const string BillingReminder = "Billing Reminder";
            public const string TrialConfirmation = "Trial Confirmation";
            public const string RegistrationWelcome = "Registration Welcome";
            public const string ExamNotification = "Exam Notification";
            public const string GradeProgression = "Grade Progression";
            public const string PaymentConfirmation = "Payment Confirmation";
            public const string ClassSchedule = "Class Schedule";
            public const string CertificateIssued = "Certificate Issued";
            public const string OverduePayment = "Overdue Payment";
            public const string TrialReminder = "Trial Reminder";
            public const string AttendanceAlert = "Attendance Alert";
            public const string InventoryAlert = "Inventory Alert";
        }

        // Business Logic Methods

        /// <summary>
        /// Validate email template business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateTemplateRules()
        {
            var errors = new List<string>();

            // Subject validation
            if (string.IsNullOrWhiteSpace(Subject))
            {
                errors.Add("Subject is required");
            }
            else if (Subject.Length > 255)
            {
                errors.Add("Subject cannot exceed 255 characters");
            }

            // Body validation
            if (string.IsNullOrWhiteSpace(Body))
            {
                errors.Add("Body is required");
            }
            else if (Body.Length > 50000) // Reasonable limit for email body
            {
                errors.Add("Body is too long (limit: 50,000 characters)");
            }

            // Template name validation
            if (string.IsNullOrWhiteSpace(TemplateName))
            {
                errors.Add("Template name is required");
            }

            // Placeholder validation
            var subjectPlaceholders = ExtractPlaceholders(Subject);
            var bodyPlaceholders = ExtractPlaceholders(Body);

            foreach (var placeholder in subjectPlaceholders.Concat(bodyPlaceholders))
            {
                if (!IsValidPlaceholder(placeholder))
                {
                    errors.Add($"Invalid placeholder format: {placeholder}");
                }
            }

            return errors;
        }

        /// <summary>
        /// Extract placeholders from text (format: {{placeholder}})
        /// </summary>
        /// <param name="text">Text to search for placeholders</param>
        /// <returns>List of found placeholders</returns>
        private List<string> ExtractPlaceholders(string text)
        {
            if (string.IsNullOrEmpty(text)) return new List<string>();

            var regex = new Regex(@"\{\{([^}]+)\}\}", RegexOptions.IgnoreCase);
            return regex.Matches(text)
                       .Cast<Match>()
                       .Select(m => m.Groups[1].Value.Trim())
                       .ToList();
        }

        /// <summary>
        /// Validate placeholder format
        /// </summary>
        /// <param name="placeholder">Placeholder to validate</param>
        /// <returns>True if valid</returns>
        private bool IsValidPlaceholder(string placeholder)
        {
            // Valid placeholders should contain only letters, numbers, dots, and underscores
            return Regex.IsMatch(placeholder, @"^[a-zA-Z][a-zA-Z0-9_\.]*$");
        }

        /// <summary>
        /// Process template with actual values
        /// </summary>
        /// <param name="values">Dictionary of placeholder values</param>
        /// <returns>Processed template with subject and body</returns>
        public (string processedSubject, string processedBody) ProcessTemplate(Dictionary<string, object> values)
        {
            var processedSubject = ProcessText(Subject, values);
            var processedBody = ProcessText(Body, values);

            return (processedSubject, processedBody);
        }

        /// <summary>
        /// Process text by replacing placeholders with actual values
        /// </summary>
        /// <param name="text">Text with placeholders</param>
        /// <param name="values">Dictionary of values</param>
        /// <returns>Processed text</returns>
        private string ProcessText(string text, Dictionary<string, object> values)
        {
            if (string.IsNullOrEmpty(text) || values == null) return text;

            var result = text;
            var regex = new Regex(@"\{\{([^}]+)\}\}", RegexOptions.IgnoreCase);

            return regex.Replace(result, match =>
            {
                var placeholder = match.Groups[1].Value.Trim();
                if (values.TryGetValue(placeholder, out var value))
                {
                    return value?.ToString() ?? "";
                }
                return match.Value; // Keep original if not found
            });
        }

        /// <summary>
        /// Get standard placeholders for different template types
        /// </summary>
        /// <param name="templateType">Template type</param>
        /// <returns>List of standard placeholders</returns>
        public static List<string> GetStandardPlaceholders(string templateType)
        {
            var common = new List<string>
            {
                "StudentName", "CompanyName", "SiteName", "CurrentDate", "CurrentTime"
            };

            var specific = templateType switch
            {
                TemplateTypes.BillingReminder => new[] { "BillingNumber", "Amount", "DueDate", "PaymentMethod" },
                TemplateTypes.TrialConfirmation => new[] { "TrialDate", "TrialTime", "TeacherName", "Location" },
                TemplateTypes.ExamNotification => new[] { "ExamName", "ExamDate", "ExamTime", "Location", "ExaminerName" },
                TemplateTypes.PaymentConfirmation => new[] { "PaymentAmount", "PaymentDate", "PaymentMethod", "ReceiptNumber" },
                TemplateTypes.ClassSchedule => new[] { "ScheduleDate", "StartTime", "EndTime", "TeacherName", "Room" },
                TemplateTypes.CertificateIssued => new[] { "CertificateNumber", "GradeName", "IssueDate", "Achievement" },
                _ => Array.Empty<string>()
            };

            return common.Concat(specific).ToList();
        }

        /// <summary>
        /// Create a new email template
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="templateName">Template name</param>
        /// <param name="templateType">Template type</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body</param>
        /// <param name="createdBy">Creator user ID</param>
        /// <returns>New email template instance</returns>
        public static EmailTemplate CreateTemplate(int companyId, string templateName, string templateType,
            string subject, string body, int? createdBy = null)
        {
            return new EmailTemplate
            {
                CompanyId = companyId,
                TemplateName = templateName,
                TemplateType = templateType,
                Subject = subject,
                Body = body,
                IsActive = true,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Clone template with modifications
        /// </summary>
        /// <param name="newName">New template name</param>
        /// <param name="newType">New template type (optional)</param>
        /// <param name="createdBy">Creator user ID</param>
        /// <returns>Cloned template</returns>
        public EmailTemplate Clone(string newName, string? newType = null, int? createdBy = null)
        {
            return new EmailTemplate
            {
                CompanyId = CompanyId,
                TemplateName = newName,
                TemplateType = newType ?? TemplateType,
                Subject = Subject,
                Body = Body,
                IsActive = true,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get template usage statistics
        /// </summary>
        /// <returns>Dictionary with usage statistics</returns>
        public Dictionary<string, object> GetUsageStatistics()
        {
            var logs = EmailLogs?.ToList() ?? new List<EmailLog>();
            var sentLogs = logs.Where(l => l.Status == "Sent").ToList();

            return new Dictionary<string, object>
            {
                {"TemplateId", EmailTemplateId},
                {"TemplateName", TemplateName},
                {"TemplateType", TemplateType},
                {"TotalEmailsCreated", logs.Count},
                {"EmailsSent", sentLogs.Count},
                {"EmailsFailed", logs.Count(l => l.Status == "Failed")},
                {"EmailsPending", logs.Count(l => l.Status == "Pending")},
                {"SuccessRate", logs.Any() ? Math.Round((decimal)sentLogs.Count / logs.Count * 100, 2) : 0},
                {"LastUsed", LastUsed},
                {"UsageFrequency", UsageFrequency},
                {"WordCount", WordCount},
                {"PlaceholderCount", Placeholders.Count},
                {"Complexity", Complexity},
                {"IsActive", IsActive}
            };
        }

        /// <summary>
        /// Generate template preview with sample data
        /// </summary>
        /// <returns>Tuple with preview subject and body</returns>
        public (string previewSubject, string previewBody) GeneratePreview()
        {
            var sampleData = GetSampleData();
            return ProcessTemplate(sampleData);
        }

        /// <summary>
        /// Get sample data for template preview
        /// </summary>
        /// <returns>Dictionary with sample values</returns>
        private Dictionary<string, object> GetSampleData()
        {
            return new Dictionary<string, object>
            {
                {"StudentName", "John Doe"},
                {"ParentName", "Jane Doe"},
                {"CompanyName", "Kawai Music School Indonesia"},
                {"SiteName", "Jakarta Central"},
                {"TeacherName", "Ms. Sarah"},
                {"CurrentDate", DateTime.Today.ToString("dd/MM/yyyy")},
                {"CurrentTime", DateTime.Now.ToString("HH:mm")},
                {"BillingNumber", "INV-2501-001"},
                {"Amount", "Rp 500,000"},
                {"DueDate", DateTime.Today.AddDays(7).ToString("dd/MM/yyyy")},
                {"TrialDate", DateTime.Today.AddDays(3).ToString("dd/MM/yyyy")},
                {"TrialTime", "10:00 AM"},
                {"ExamName", "Grade 1 Piano Examination"},
                {"Location", "Room A"},
                {"GradeName", "Grade 1 Piano"}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the email template</returns>
        public override string ToString() => DisplayName;
    }
}
