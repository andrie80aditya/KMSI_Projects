using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    /// <summary>
    /// View model for Site management operations
    /// Used for Create and Edit operations in Site Master
    /// </summary>
    public class SiteViewModel
    {
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site code is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Site code must be between 2 and 10 characters")]
        [Display(Name = "Site Code")]
        [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "Site code must contain only uppercase letters and numbers")]
        public string SiteCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Site name is required")]
        [StringLength(100, ErrorMessage = "Site name cannot exceed 100 characters")]
        [Display(Name = "Site Name")]
        public string SiteName { get; set; } = string.Empty;

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
        //[Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        //[EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters")]
        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Read-only properties for display
        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int UsersCount { get; set; }
    }

    /// <summary>
    /// View model for Site list operations
    /// Used for displaying sites in the index view
    /// </summary>
    public class SiteListViewModel
    {
        public int SiteId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;
        public string SiteCode { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ManagerName { get; set; }
        public bool IsActive { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int UsersCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string DisplayName => $"{SiteCode} - {SiteName}";
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
    }

    /// <summary>
    /// View model for Site details modal
    /// Used for displaying detailed site information
    /// </summary>
    public class SiteDetailsViewModel
    {
        public int SiteId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;
        public string SiteCode { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ManagerName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int UsersCount { get; set; }
        public int ActiveStudents { get; set; }
        public int ActiveTeachers { get; set; }
        public int ActiveUsers { get; set; }

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string DisplayName => $"{SiteCode} - {SiteName}";
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
    }
}
