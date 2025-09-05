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
        public int CompanyId { get; set; }

        public int? ParentCompanyId { get; set; }

        [Required(ErrorMessage = "Company code is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Company code must be between 2 and 10 characters")]
        [Display(Name = "Company Code")]
        public string CompanyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

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
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Is Head Office")]
        public bool IsHeadOffice { get; set; } = false;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }

        [Display(Name = "Updated Date")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }

        // Navigation Properties - Let EF handle relationships
        public virtual Company? ParentCompany { get; set; }
        public virtual ICollection<Company> ChildCompanies { get; set; } = new List<Company>();
        public virtual ICollection<Site> Sites { get; set; } = new List<Site>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        // Computed Properties
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{CompanyCode} - {CompanyName}";

        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        [NotMapped]
        [Display(Name = "Company Type")]
        public string CompanyTypeDisplay => IsHeadOffice ? "Head Office" : "Branch";

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

        [NotMapped]
        [Display(Name = "Total Sites")]
        public int TotalSites => Sites?.Count ?? 0;

        [NotMapped]
        [Display(Name = "Total Users")]
        public int TotalUsers => Users?.Count ?? 0;

        [NotMapped]
        [Display(Name = "Active Users")]
        public int ActiveUsers => Users?.Count(u => u.IsActive) ?? 0;
    }
}
