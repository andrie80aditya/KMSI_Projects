using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a company/branch in the KMSI Course Management System
    /// Supports multi-tenancy with parent-child company relationships
    /// </summary>
    [Table("Companies")]
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Company code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Company code must be exactly 3 characters")]
        [Display(Name = "Company Code")]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Company code must be 3 uppercase letters (e.g., KMI, KMJ)")]
        public string CompanyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Parent Company")]
        [ForeignKey("ParentCompany")]
        public int? ParentCompanyId { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "Province name cannot exceed 50 characters")]
        [Display(Name = "Province")]
        public string? Province { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [Display(Name = "Is Head Office")]
        public bool IsHeadOffice { get; set; } = false;

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
        /// Parent company (for group company structure)
        /// </summary>
        [Display(Name = "Parent Company")]
        public virtual Company? ParentCompany { get; set; }

        /// <summary>
        /// Child companies (subsidiaries or branches)
        /// </summary>
        [Display(Name = "Child Companies")]
        public virtual ICollection<Company> ChildCompanies { get; set; } = new List<Company>();

        /// <summary>
        /// Sites/branches under this company
        /// </summary>
        [Display(Name = "Sites")]
        public virtual ICollection<Site> Sites { get; set; } = new List<Site>();

        /// <summary>
        /// Grades offered by this company
        /// </summary>
        [Display(Name = "Grades")]
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

        /// <summary>
        /// Books managed by this company
        /// </summary>
        [Display(Name = "Books")]
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        /// <summary>
        /// Users belonging to this company
        /// </summary>
        [Display(Name = "Users")]
        public virtual ICollection<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// Students registered under this company
        /// </summary>
        [Display(Name = "Students")]
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();

        /// <summary>
        /// Teachers working for this company
        /// </summary>
        [Display(Name = "Teachers")]
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        /// <summary>
        /// Billing periods for this company
        /// </summary>
        [Display(Name = "Billing Periods")]
        public virtual ICollection<BillingPeriod> BillingPeriods { get; set; } = new List<BillingPeriod>();

        /// <summary>
        /// Examinations conducted by this company
        /// </summary>
        [Display(Name = "Examinations")]
        public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

        /// <summary>
        /// Email templates for this company
        /// </summary>
        [Display(Name = "Email Templates")]
        public virtual ICollection<EmailTemplate> EmailTemplates { get; set; } = new List<EmailTemplate>();

        /// <summary>
        /// System settings specific to this company
        /// </summary>
        [Display(Name = "System Settings")]
        public virtual ICollection<SystemSetting> SystemSettings { get; set; } = new List<SystemSetting>();

        /// <summary>
        /// Audit logs for this company
        /// </summary>
        [Display(Name = "Audit Logs")]
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        // Audit Navigation Properties (for CreatedBy/UpdatedBy)

        /// <summary>
        /// User who created this company record
        /// </summary>
        [InverseProperty("CreatedCompanies")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this company record
        /// </summary>
        [InverseProperty("UpdatedCompanies")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining code and name
        /// </summary>
        [NotMapped]
        [Display(Name = "Company")]
        public string DisplayName => $"{CompanyCode} - {CompanyName}";

        /// <summary>
        /// Full address combining all address fields
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Address")]
        public string FullAddress
        {
            get
            {
                var addressParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Address)) addressParts.Add(Address);
                if (!string.IsNullOrWhiteSpace(City)) addressParts.Add(City);
                if (!string.IsNullOrWhiteSpace(Province)) addressParts.Add(Province);
                return string.Join(", ", addressParts);
            }
        }

        /// <summary>
        /// Status display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Company type display
        /// </summary>
        [NotMapped]
        [Display(Name = "Company Type")]
        public string CompanyTypeDisplay => IsHeadOffice ? "Head Office" : "Branch";

        /// <summary>
        /// Total active sites count
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Sites Count")]
        public int ActiveSitesCount => Sites?.Count(s => s.IsActive) ?? 0;

        /// <summary>
        /// Total active students count across all sites
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Students Count")]
        public int ActiveStudentsCount => Students?.Count(s => s.IsActive && s.Status == "Active") ?? 0;

        /// <summary>
        /// Total active teachers count across all sites
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Teachers Count")]
        public int ActiveTeachersCount => Teachers?.Count(t => t.IsActive) ?? 0;

        // Business Logic Methods

        /// <summary>
        /// Check if this company is a subsidiary of another company
        /// </summary>
        /// <returns>True if this company has a parent company</returns>
        public bool IsSubsidiary() => ParentCompanyId.HasValue;

        /// <summary>
        /// Check if this company has child companies
        /// </summary>
        /// <returns>True if this company has child companies</returns>
        public bool HasChildCompanies() => ChildCompanies?.Any() == true;

        /// <summary>
        /// Get all sites under this company that are active
        /// </summary>
        /// <returns>Collection of active sites</returns>
        public IEnumerable<Site> GetActiveSites() => Sites?.Where(s => s.IsActive) ?? Enumerable.Empty<Site>();

        /// <summary>
        /// Get company hierarchy level (0 = root, 1 = child, etc.)
        /// </summary>
        /// <returns>Hierarchy level</returns>
        public int GetHierarchyLevel()
        {
            int level = 0;
            var current = this;
            while (current?.ParentCompany != null)
            {
                level++;
                current = current.ParentCompany;
            }
            return level;
        }

        /// <summary>
        /// Generate next student code for this company
        /// Format: {CompanyCode}-{YYMM}-{5-digit-sequence}
        /// Example: KMI-2401-01530
        /// </summary>
        /// <param name="existingCodes">Existing student codes for this period</param>
        /// <returns>Next student code</returns>
        public string GenerateNextStudentCode(IEnumerable<string> existingCodes)
        {
            var now = DateTime.Now;
            var yearMonth = $"{now.Year % 100:00}{now.Month:00}";
            var prefix = $"{CompanyCode}-{yearMonth}-";

            var existingNumbers = existingCodes
                .Where(code => code.StartsWith(prefix))
                .Select(code =>
                {
                    var parts = code.Split('-');
                    return parts.Length == 3 && int.TryParse(parts[2], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingNumbers.Max() + 1;
            return $"{prefix}{nextNumber:00000}";
        }

        /// <summary>
        /// Validate if company code is unique within the same parent company
        /// </summary>
        /// <param name="otherCompanies">Other companies to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsCompanyCodeUnique(IEnumerable<Company> otherCompanies)
        {
            return !otherCompanies.Any(c =>
                c.CompanyId != CompanyId &&
                c.CompanyCode == CompanyCode &&
                c.ParentCompanyId == ParentCompanyId);
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the company</returns>
        public override string ToString() => DisplayName;
    }
}
