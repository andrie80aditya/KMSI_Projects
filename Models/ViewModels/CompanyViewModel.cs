using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    /// <summary>
    /// View model for Company management operations
    /// Used for Create and Edit operations in Company Master
    /// </summary>
    public class CompanyViewModel
    {
        public int CompanyId { get; set; }

        [Display(Name = "Parent Company")]
        public int? ParentCompanyId { get; set; }

        [Required(ErrorMessage = "Company code is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Company code must be between 2 and 10 characters")]
        [Display(Name = "Company Code")]
        [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "Company code must contain only uppercase letters and numbers")]
        public string CompanyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
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

        // Read-only properties for display
        public string? ParentCompanyName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int SitesCount { get; set; }
    }

    /// <summary>
    /// View model for Company list operations
    /// Used for displaying companies in the index view
    /// </summary>
    public class CompanyListViewModel
    {
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? ParentCompanyName { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsHeadOffice { get; set; }
        public bool IsActive { get; set; }
        public int SitesCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string HeadOfficeDisplay => IsHeadOffice ? "Yes" : "No";
        public string DisplayName => $"{CompanyCode} - {CompanyName}";
    }
}
