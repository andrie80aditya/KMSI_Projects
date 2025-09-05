using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a certificate in the KMSI Course Management System
    /// Manages academic certificates issued to students after successful examination completion
    /// </summary>
    [Table("Certificates")]
    public class Certificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CertificateId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Student examination is required")]
        [Display(Name = "Student Examination")]
        [ForeignKey("StudentExamination")]
        public int StudentExaminationId { get; set; }

        [Required(ErrorMessage = "Certificate number is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Certificate number must be between 5 and 50 characters")]
        [Display(Name = "Certificate Number")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Certificate number must contain only uppercase letters, numbers, and hyphens")]
        public string CertificateNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade is required")]
        [Display(Name = "Grade")]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Issue date is required")]
        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Certificate title is required")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Certificate title must be between 10 and 200 characters")]
        [Display(Name = "Certificate Title")]
        public string CertificateTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Issued by is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Issued by must be between 3 and 100 characters")]
        [Display(Name = "Issued By")]
        public string IssuedBy { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Signed by cannot exceed 100 characters")]
        [Display(Name = "Signed By")]
        public string? SignedBy { get; set; }

        [StringLength(255, ErrorMessage = "Certificate path cannot exceed 255 characters")]
        [Display(Name = "Certificate Path")]
        public string? CertificatePath { get; set; } // PDF file path

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Issued"; // Issued, Revoked, Replaced

        [Range(0, 999, ErrorMessage = "Print count must be between 0 and 999")]
        [Display(Name = "Print Count")]
        public int PrintCount { get; set; } = 0;

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

        // Navigation Properties

        /// <summary>
        /// Student who received this certificate
        /// </summary>
        [Required]
        [Display(Name = "Student")]
        public virtual Student Student { get; set; } = null!;

        /// <summary>
        /// Student examination that led to this certificate
        /// </summary>
        [Required]
        [Display(Name = "Student Examination")]
        public virtual StudentExamination StudentExamination { get; set; } = null!;

        /// <summary>
        /// Grade level for this certificate
        /// </summary>
        [Required]
        [Display(Name = "Grade")]
        public virtual Grade Grade { get; set; } = null!;

        /// <summary>
        /// User who created this certificate
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{CertificateNumber} - {Student?.FullName}";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplayName => $"{CertificateNumber} - {Grade?.GradeName}";

        /// <summary>
        /// Status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => Status switch
        {
            "Issued" => "🏆 Issued",
            "Revoked" => "❌ Revoked",
            "Replaced" => "🔄 Replaced",
            _ => Status
        };

        /// <summary>
        /// Certificate type based on grade or content
        /// </summary>
        [NotMapped]
        [Display(Name = "Certificate Type")]
        public string CertificateType
        {
            get
            {
                var title = CertificateTitle?.ToLower() ?? "";
                return title switch
                {
                    var t when t.Contains("completion") => "Completion Certificate",
                    var t when t.Contains("achievement") => "Achievement Certificate",
                    var t when t.Contains("participation") => "Participation Certificate",
                    var t when t.Contains("excellence") => "Excellence Certificate",
                    var t when t.Contains("grade") || t.Contains("level") => "Grade Certificate",
                    _ => "Academic Certificate"
                };
            }
        }

        /// <summary>
        /// Certificate validity status
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Valid")]
        public bool IsValid => Status == "Issued";

        /// <summary>
        /// Indicates if certificate was revoked
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Revoked")]
        public bool IsRevoked => Status == "Revoked";

        /// <summary>
        /// Indicates if certificate was replaced
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Replaced")]
        public bool IsReplaced => Status == "Replaced";

        /// <summary>
        /// Indicates if certificate has been printed
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Printed")]
        public bool IsPrinted => PrintCount > 0;

        /// <summary>
        /// Days since certificate was issued
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Issued")]
        public int DaysSinceIssued => (DateTime.Today - IssueDate.Date).Days;

        /// <summary>
        /// Age category of certificate
        /// </summary>
        [NotMapped]
        [Display(Name = "Age Category")]
        public string AgeCategory
        {
            get
            {
                var days = DaysSinceIssued;
                return days switch
                {
                    <= 7 => "New",
                    <= 30 => "Recent",
                    <= 365 => "Current Year",
                    <= 1095 => "Within 3 Years",
                    _ => "Historical"
                };
            }
        }

        /// <summary>
        /// Exam score from student examination
        /// </summary>
        [NotMapped]
        [Display(Name = "Exam Score")]
        public decimal? ExamScore => StudentExamination?.Score;

        /// <summary>
        /// Exam percentage from student examination
        /// </summary>
        [NotMapped]
        [Display(Name = "Exam Percentage")]
        public decimal? ExamPercentage => StudentExamination?.Percentage;

        /// <summary>
        /// Letter grade from student examination
        /// </summary>
        [NotMapped]
        [Display(Name = "Letter Grade")]
        public string? LetterGrade => StudentExamination?.Grade;

        /// <summary>
        /// Achievement level based on exam score
        /// </summary>
        [NotMapped]
        [Display(Name = "Achievement Level")]
        public string AchievementLevel
        {
            get
            {
                if (!ExamPercentage.HasValue) return "Not Available";

                return ExamPercentage.Value switch
                {
                    >= 95 => "Outstanding",
                    >= 90 => "Excellent",
                    >= 85 => "Very Good",
                    >= 80 => "Good",
                    >= 75 => "Satisfactory",
                    >= 70 => "Acceptable",
                    _ => "Pass"
                };
            }
        }

        /// <summary>
        /// Certificate priority based on achievement and grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority")]
        public string Priority
        {
            get
            {
                if (AchievementLevel == "Outstanding" || AchievementLevel == "Excellent")
                    return "High";
                if (Grade?.SortOrder >= 5) // Higher grades
                    return "High";
                return "Normal";
            }
        }

        /// <summary>
        /// Has PDF file available
        /// </summary>
        [NotMapped]
        [Display(Name = "Has PDF")]
        public bool HasPdf => !string.IsNullOrEmpty(CertificatePath);

        /// <summary>
        /// File name from path
        /// </summary>
        [NotMapped]
        [Display(Name = "File Name")]
        public string? FileName => !string.IsNullOrEmpty(CertificatePath) ?
            System.IO.Path.GetFileName(CertificatePath) : null;

        /// <summary>
        /// Print frequency category
        /// </summary>
        [NotMapped]
        [Display(Name = "Print Frequency")]
        public string PrintFrequency => PrintCount switch
        {
            0 => "Never Printed",
            1 => "Printed Once",
            <= 3 => "Printed Few Times",
            <= 10 => "Frequently Printed",
            _ => "Extensively Printed"
        };

        // Static Constants for Status
        public static class CertificateStatus
        {
            public const string Issued = "Issued";
            public const string Revoked = "Revoked";
            public const string Replaced = "Replaced";
        }

        // Business Logic Methods

        /// <summary>
        /// Validate certificate business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateCertificateRules()
        {
            var errors = new List<string>();

            // Issue date validation
            if (IssueDate.Date > DateTime.Today)
            {
                errors.Add("Issue date cannot be in the future");
            }

            // Certificate number format validation
            if (string.IsNullOrWhiteSpace(CertificateNumber))
            {
                errors.Add("Certificate number is required");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(CertificateNumber, @"^[A-Z0-9-]+$"))
            {
                errors.Add("Certificate number must contain only uppercase letters, numbers, and hyphens");
            }

            // Title validation
            if (string.IsNullOrWhiteSpace(CertificateTitle))
            {
                errors.Add("Certificate title is required");
            }

            // Status validation
            var validStatuses = new[] { CertificateStatus.Issued, CertificateStatus.Revoked, CertificateStatus.Replaced };
            if (!validStatuses.Contains(Status))
            {
                errors.Add($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}");
            }

            // File path validation
            if (!string.IsNullOrEmpty(CertificatePath))
            {
                var allowedExtensions = new[] { ".pdf", ".PDF" };
                var extension = System.IO.Path.GetExtension(CertificatePath);
                if (!allowedExtensions.Contains(extension))
                {
                    errors.Add("Certificate file must be a PDF");
                }
            }

            // Print count validation
            if (PrintCount < 0)
            {
                errors.Add("Print count cannot be negative");
            }

            // Exam result validation (must have passed to get certificate)
            if (StudentExamination?.Result != "Pass")
            {
                errors.Add("Certificate can only be issued for passed examinations");
            }

            return errors;
        }

        /// <summary>
        /// Generate unique certificate number
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="gradeCode">Grade code</param>
        /// <param name="issueDate">Issue date</param>
        /// <param name="existingNumbers">Existing certificate numbers</param>
        /// <returns>Unique certificate number</returns>
        public static string GenerateCertificateNumber(string companyCode, string gradeCode,
            DateTime issueDate, IEnumerable<string> existingNumbers)
        {
            var dateCode = issueDate.ToString("yyyyMM");
            var prefix = $"CERT-{companyCode}-{gradeCode}-{dateCode}-";

            var existingSequences = existingNumbers
                .Where(n => n.StartsWith(prefix))
                .Select(n =>
                {
                    var parts = n.Split('-');
                    return parts.Length == 5 && int.TryParse(parts[4], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingSequences.Max() + 1;
            return $"{prefix}{nextNumber:0000}";
        }

        /// <summary>
        /// Update certificate status with validation
        /// </summary>
        /// <param name="newStatus">New status</param>
        /// <param name="notes">Optional notes for status change</param>
        /// <returns>True if status was updated successfully</returns>
        public bool UpdateStatus(string newStatus, string? notes = null)
        {
            var validTransitions = GetValidStatusTransitions();
            if (!validTransitions.Contains(newStatus))
                return false;

            Status = newStatus;
            if (!string.IsNullOrEmpty(notes))
                Notes = notes;

            return true;
        }

        /// <summary>
        /// Get valid status transitions based on current status
        /// </summary>
        /// <returns>List of valid next statuses</returns>
        public List<string> GetValidStatusTransitions()
        {
            return Status switch
            {
                CertificateStatus.Issued => new List<string> { CertificateStatus.Revoked, CertificateStatus.Replaced },
                CertificateStatus.Revoked => new List<string>(), // Terminal status
                CertificateStatus.Replaced => new List<string>(), // Terminal status
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Revoke certificate with reason
        /// </summary>
        /// <param name="reason">Reason for revocation</param>
        /// <returns>True if revoked successfully</returns>
        public bool RevokeCertificate(string reason)
        {
            if (Status != CertificateStatus.Issued)
                return false;

            Status = CertificateStatus.Revoked;
            Notes = $"Revoked: {reason}";
            return true;
        }

        /// <summary>
        /// Mark certificate as replaced
        /// </summary>
        /// <param name="replacementCertificateNumber">New certificate number that replaces this one</param>
        /// <returns>True if marked as replaced successfully</returns>
        public bool MarkAsReplaced(string replacementCertificateNumber)
        {
            if (Status != CertificateStatus.Issued)
                return false;

            Status = CertificateStatus.Replaced;
            Notes = $"Replaced by certificate: {replacementCertificateNumber}";
            return true;
        }

        /// <summary>
        /// Record certificate printing
        /// </summary>
        /// <param name="printDate">Date when printed (optional, defaults to now)</param>
        /// <returns>True if recorded successfully</returns>
        public bool RecordPrint(DateTime? printDate = null)
        {
            if (Status != CertificateStatus.Issued)
                return false;

            PrintCount++;
            LastPrintDate = printDate ?? DateTime.Now;
            return true;
        }

        /// <summary>
        /// Get certificate summary for reporting
        /// </summary>
        /// <returns>Dictionary with certificate summary</returns>
        public Dictionary<string, object> GetCertificateSummary()
        {
            return new Dictionary<string, object>
            {
                {"CertificateId", CertificateId},
                {"CertificateNumber", CertificateNumber},
                {"StudentName", Student?.FullName},
                {"StudentCode", Student?.StudentCode},
                {"GradeName", Grade?.GradeName},
                {"CertificateTitle", CertificateTitle},
                {"IssueDate", IssueDate},
                {"Status", Status},
                {"IsValid", IsValid},
                {"ExamScore", ExamScore},
                {"ExamPercentage", ExamPercentage},
                {"LetterGrade", LetterGrade},
                {"AchievementLevel", AchievementLevel},
                {"CertificateType", CertificateType},
                {"Priority", Priority},
                {"PrintCount", PrintCount},
                {"LastPrintDate", LastPrintDate},
                {"HasPdf", HasPdf},
                {"DaysSinceIssued", DaysSinceIssued},
                {"AgeCategory", AgeCategory},
                {"IssuedBy", IssuedBy},
                {"SignedBy", SignedBy}
            };
        }

        /// <summary>
        /// Create a new certificate
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="studentExaminationId">Student examination ID</param>
        /// <param name="gradeId">Grade ID</param>
        /// <param name="certificateNumber">Certificate number</param>
        /// <param name="certificateTitle">Certificate title</param>
        /// <param name="issuedBy">Issued by organization</param>
        /// <param name="signedBy">Signed by person (optional)</param>
        /// <param name="issueDate">Issue date (optional, defaults to today)</param>
        /// <param name="createdBy">Creator user ID</param>
        /// <returns>New certificate instance</returns>
        public static Certificate CreateCertificate(int studentId, int studentExaminationId, int gradeId,
            string certificateNumber, string certificateTitle, string issuedBy,
            string? signedBy = null, DateTime? issueDate = null, int? createdBy = null)
        {
            return new Certificate
            {
                StudentId = studentId,
                StudentExaminationId = studentExaminationId,
                GradeId = gradeId,
                CertificateNumber = certificateNumber,
                CertificateTitle = certificateTitle,
                IssuedBy = issuedBy,
                SignedBy = signedBy,
                IssueDate = issueDate ?? DateTime.Today,
                Status = CertificateStatus.Issued,
                PrintCount = 0,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get certificates by status
        /// </summary>
        /// <param name="certificates">Collection of certificates</param>
        /// <param name="status">Status to filter by</param>
        /// <returns>Filtered certificates</returns>
        public static IEnumerable<Certificate> GetByStatus(IEnumerable<Certificate> certificates, string status)
        {
            return certificates.Where(c => c.Status == status);
        }

        /// <summary>
        /// Get certificates by date range
        /// </summary>
        /// <param name="certificates">Collection of certificates</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Certificates issued within date range</returns>
        public static IEnumerable<Certificate> GetByDateRange(IEnumerable<Certificate> certificates,
            DateTime startDate, DateTime endDate)
        {
            return certificates.Where(c => c.IssueDate.Date >= startDate.Date &&
                                          c.IssueDate.Date <= endDate.Date);
        }

        /// <summary>
        /// Calculate certificate statistics
        /// </summary>
        /// <param name="certificates">Collection of certificates</param>
        /// <returns>Dictionary with statistics</returns>
        public static Dictionary<string, object> CalculateStatistics(IEnumerable<Certificate> certificates)
        {
            var certList = certificates.ToList();
            var totalCerts = certList.Count;

            if (totalCerts == 0)
            {
                return new Dictionary<string, object>
                {
                    {"TotalCertificates", 0},
                    {"ValidCertificates", 0},
                    {"RevokedCertificates", 0},
                    {"ReplacedCertificates", 0}
                };
            }

            var validCerts = certList.Count(c => c.IsValid);
            var revokedCerts = certList.Count(c => c.IsRevoked);
            var replacedCerts = certList.Count(c => c.IsReplaced);
            var printedCerts = certList.Count(c => c.IsPrinted);

            var avgPrintCount = certList.Average(c => c.PrintCount);
            var totalPrints = certList.Sum(c => c.PrintCount);

            return new Dictionary<string, object>
            {
                {"TotalCertificates", totalCerts},
                {"ValidCertificates", validCerts},
                {"RevokedCertificates", revokedCerts},
                {"ReplacedCertificates", replacedCerts},
                {"PrintedCertificates", printedCerts},
                {"ValidityRate", Math.Round((decimal)validCerts / totalCerts * 100, 2)},
                {"PrintRate", Math.Round((decimal)printedCerts / totalCerts * 100, 2)},
                {"AveragePrintCount", Math.Round(avgPrintCount, 2)},
                {"TotalPrints", totalPrints},
                {"HighAchievers", certList.Count(c => c.AchievementLevel == "Outstanding" || c.AchievementLevel == "Excellent")},
                {"RecentCertificates", certList.Count(c => c.AgeCategory == "New" || c.AgeCategory == "Recent")}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the certificate</returns>
        public override string ToString() => DisplayName;
    }
}
