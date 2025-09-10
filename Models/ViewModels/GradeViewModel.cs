using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    /// <summary>
    /// View model for Grade management operations
    /// Used for Create and Edit operations in Grade Master
    /// </summary>
    public class GradeViewModel
    {
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Grade code is required")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Grade code must be between 1 and 10 characters")]
        [Display(Name = "Grade Code")]
        [RegularExpression("^[A-Z0-9]+$", ErrorMessage = "Grade code must contain only uppercase letters and numbers")]
        public string GradeCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grade name is required")]
        [StringLength(50, ErrorMessage = "Grade name cannot exceed 50 characters")]
        [Display(Name = "Grade Name")]
        public string GradeName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Range(1, 104, ErrorMessage = "Duration must be between 1 and 104 weeks")]
        [Display(Name = "Duration (Weeks)")]
        public int? Duration { get; set; }

        [Range(1, 100, ErrorMessage = "Sort order must be between 1 and 100")]
        [Display(Name = "Sort Order")]
        public int? SortOrder { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Read-only properties for display
        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StudentsCount { get; set; }
        public int BooksCount { get; set; }
    }
}