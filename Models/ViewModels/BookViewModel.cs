using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    /// <summary>
    /// View model for Book management operations
    /// Used for Create and Edit operations in Book Master - follows same pattern as GradeViewModel
    /// Uses the same validation rules as the existing Book model
    /// </summary>
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Book code is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Book code must be between 2 and 20 characters")]
        [Display(Name = "Book Code")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Book code must contain only uppercase letters, numbers, and hyphens")]
        public string BookCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Book title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Book title must be between 3 and 200 characters")]
        [Display(Name = "Book Title")]
        public string BookTitle { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters")]
        [Display(Name = "Author")]
        public string? Author { get; set; }

        [StringLength(100, ErrorMessage = "Publisher cannot exceed 100 characters")]
        [Display(Name = "Publisher")]
        public string? Publisher { get; set; }

        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
        [Display(Name = "ISBN")]
        //[RegularExpression(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$",
        //    ErrorMessage = "Invalid ISBN format")]
        public string? ISBN { get; set; }

        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Read-only properties for display (same pattern as GradeViewModel)
        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Additional properties for business logic and display
        public int GradesCount { get; set; }
        public int StockQuantity { get; set; }
        public int CoursesCount { get; set; }

        // Computed properties for display
        public string DisplayName => $"{BookCode} - {BookTitle}";

        public string ShortDisplayName => $"{BookCode} - {(BookTitle.Length > 30 ? BookTitle.Substring(0, 30) + "..." : BookTitle)}";

        public string CategoryDisplay => Category?.ToLower() switch
        {
            "piano" => "🎹 Piano",
            "violin" => "🎻 Violin",
            "guitar" => "🎸 Guitar",
            "theory" => "📚 Theory",
            "songbook" => "🎵 Songbook",
            "exercise" => "💪 Exercise",
            "technique" => "🎯 Technique",
            "method" => "📖 Method",
            _ => $"📘 {Category ?? "General"}"
        };

        public string StatusDisplay => IsActive ? "🟢 Active" : "🔴 Inactive";

        public bool HasISBN => !string.IsNullOrEmpty(ISBN);

        public string FullBookInfo
        {
            get
            {
                var info = BookTitle;
                if (!string.IsNullOrEmpty(Author)) info += $" by {Author}";
                if (!string.IsNullOrEmpty(Publisher)) info += $" ({Publisher})";
                return info;
            }
        }
    }
}