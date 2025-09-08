using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    /// <summary>
    /// View model for Teacher management operations
    /// Used for Create and Edit operations in module Teacher Master
    /// </summary>
    public class TeacherViewModel
    {
        public int TeacherId { get; set; }

        [Display(Name = "User")]
        public int UserId { get; set; }

        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Teacher code is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Teacher code must be between 3 and 20 characters")]
        [Display(Name = "Teacher Code")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Teacher code must contain only uppercase letters, numbers, and hyphens")]
        public string TeacherCode { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
        [Display(Name = "Specialization")]
        public string? Specialization { get; set; }

        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
        [Display(Name = "Experience (Years)")]
        public int? ExperienceYears { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Hourly rate must be between 0 and 9,999,999.99")]
        [Display(Name = "Hourly Rate")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? HourlyRate { get; set; }

        [Range(1, 20, ErrorMessage = "Maximum students per day must be between 1 and 20")]
        [Display(Name = "Max Students Per Day")]
        public int MaxStudentsPerDay { get; set; } = 8;

        [Display(Name = "Available for Trial")]
        public bool IsAvailableForTrial { get; set; } = true;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Read-only properties for display
        public string? UserName { get; set; }
        public string? CompanyName { get; set; }
        public string? SiteName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StudentsCount { get; set; }
        public int SchedulesCount { get; set; }
    }

    /// <summary>
    /// View model for Teacher list operations
    /// Used for displaying teachers in the index view
    /// </summary>
    public class TeacherListViewModel
    {
        public int TeacherId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public int SiteId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string TeacherCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? HourlyRate { get; set; }
        public int MaxStudentsPerDay { get; set; }
        public bool IsAvailableForTrial { get; set; }
        public bool IsActive { get; set; }
        public int StudentsCount { get; set; }
        public int SchedulesCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string DisplayName => $"{TeacherCode} - {UserName}";
        public string ExperienceLevelDisplay
        {
            get
            {
                if (!ExperienceYears.HasValue) return "Not Specified";

                return ExperienceYears.Value switch
                {
                    0 => "Fresh Graduate",
                    >= 1 and <= 2 => "Junior Teacher",
                    >= 3 and <= 5 => "Experienced Teacher",
                    >= 6 and <= 10 => "Senior Teacher",
                    > 10 => "Expert Teacher",
                    _ => "Unknown"
                };
            }
        }
        public string TrialAvailabilityDisplay => IsAvailableForTrial ? "Available" : "Not Available";
        public string HourlyRateDisplay => HourlyRate?.ToString("C") ?? "Not Set";
    }

    /// <summary>
    /// View model for Teacher details modal
    /// Used for displaying detailed teacher information
    /// </summary>
    public class TeacherDetailsViewModel
    {
        public int TeacherId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string TeacherCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal? HourlyRate { get; set; }
        public int MaxStudentsPerDay { get; set; }
        public bool IsAvailableForTrial { get; set; }
        public bool IsActive { get; set; }
        public int StudentsCount { get; set; }
        public int SchedulesCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public string? UpdatedByName { get; set; }

        // Display properties
        public string StatusDisplay => IsActive ? "Active" : "Inactive";
        public string ExperienceLevelDisplay
        {
            get
            {
                if (!ExperienceYears.HasValue) return "Not Specified";

                return ExperienceYears.Value switch
                {
                    0 => "Fresh Graduate",
                    >= 1 and <= 2 => "Junior Teacher",
                    >= 3 and <= 5 => "Experienced Teacher",
                    >= 6 and <= 10 => "Senior Teacher",
                    > 10 => "Expert Teacher",
                    _ => "Unknown"
                };
            }
        }
        public string TrialAvailabilityDisplay => IsAvailableForTrial ? "Available for Trial" : "Not Available for Trial";
        public string HourlyRateDisplay => HourlyRate?.ToString("C") ?? "Not Set";
    }
}
