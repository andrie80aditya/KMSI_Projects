using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    /// <summary>
    /// View model for Student management operations
    /// Used for Create and Edit operations in module Student Master
    /// </summary>
    public class StudentViewModel
    {
        public int StudentId { get; set; }

        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Student code is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Student code must be between 3 and 20 characters")]
        [Display(Name = "Student Code")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Student code must contain only uppercase letters, numbers, and hyphens")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(1, ErrorMessage = "Gender must be M or F")]
        [RegularExpression("^[MF]$", ErrorMessage = "Gender must be M (Male) or F (Female)")]
        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "Parent/Guardian name cannot exceed 100 characters")]
        [Display(Name = "Parent/Guardian Name")]
        public string? ParentName { get; set; }

        [StringLength(20, ErrorMessage = "Parent phone cannot exceed 20 characters")]
        [Display(Name = "Parent Phone")]
        [Phone(ErrorMessage = "Invalid parent phone number format")]
        public string? ParentPhone { get; set; }

        [StringLength(100, ErrorMessage = "Parent email cannot exceed 100 characters")]
        [Display(Name = "Parent Email")]
        [EmailAddress(ErrorMessage = "Invalid parent email format")]
        public string? ParentEmail { get; set; }

        [Display(Name = "Current Grade")]
        public int? CurrentGradeId { get; set; }

        [Display(Name = "Assigned Teacher")]
        public int? AssignedTeacherId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Registration date is required")]
        [Display(Name = "Registration Date")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Read-only properties for display
        public string? CompanyName { get; set; }
        public string? SiteName { get; set; }
        public string? CurrentGradeName { get; set; }
        public string? AssignedTeacherName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Computed properties
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string DisplayName => $"{StudentCode} - {FullName}";
        public string StatusDisplay => Status;
        public string GenderDisplay => Gender switch
        {
            "M" => "Male",
            "F" => "Female",
            _ => "Not Specified"
        };
    }

    /// <summary>
    /// View model for Student list operations
    /// Used for displaying students in the index view
    /// </summary>
    public class StudentListViewModel
    {
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public int SiteId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? CurrentGradeName { get; set; }
        public string? AssignedTeacherName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Display properties
        public string StatusDisplay => IsActive ? Status : "Inactive";
        public string DisplayName => $"{StudentCode} - {FullName}";
        public string GenderDisplay => Gender switch
        {
            "M" => "Male",
            "F" => "Female",
            _ => "Not Specified"
        };
        public string ContactInfo
        {
            get
            {
                var contacts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Phone)) contacts.Add($"Student: {Phone}");
                if (!string.IsNullOrWhiteSpace(ParentPhone)) contacts.Add($"Parent: {ParentPhone}");
                return string.Join(" | ", contacts);
            }
        }
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue) return null;
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }

    /// <summary>
    /// View model for Student details modal
    /// Used for displaying detailed student information
    /// </summary>
    public class StudentDetailsViewModel
    {
        public int StudentId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
        public string? CurrentGradeName { get; set; }
        public string? AssignedTeacherName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Display properties
        public string GenderDisplay => Gender switch
        {
            "M" => "Male",
            "F" => "Female",
            _ => "Not Specified"
        };
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue) return null;
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public string ContactInfo
        {
            get
            {
                var info = new List<string>();
                if (!string.IsNullOrWhiteSpace(Phone)) info.Add($"Phone: {Phone}");
                if (!string.IsNullOrWhiteSpace(Email)) info.Add($"Email: {Email}");
                return string.Join(" | ", info);
            }
        }
        public string ParentContactInfo
        {
            get
            {
                var info = new List<string>();
                if (!string.IsNullOrWhiteSpace(ParentPhone)) info.Add($"Phone: {ParentPhone}");
                if (!string.IsNullOrWhiteSpace(ParentEmail)) info.Add($"Email: {ParentEmail}");
                return string.Join(" | ", info);
            }
        }
    }
}