using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a book in the KMSI Course Management System
    /// Manages music learning books used in courses with pricing, inventory, and grade associations
    /// </summary>
    [Table("Books")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
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
        [RegularExpression(@"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$",
            ErrorMessage = "Invalid ISBN format")]
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
        /// Company that manages this book
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// User who created this book record
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this book record
        /// </summary>
        [Display(Name = "Updated By")]
        public virtual User? UpdatedByUser { get; set; }

        /// <summary>
        /// Book prices for different sites
        /// </summary>
        [Display(Name = "Book Prices")]
        public virtual ICollection<BookPrice> BookPrices { get; set; } = new List<BookPrice>();

        /// <summary>
        /// Grade-book mappings (which grades use this book)
        /// </summary>
        [Display(Name = "Grade Books")]
        public virtual ICollection<GradeBook> GradeBooks { get; set; } = new List<GradeBook>();

        /// <summary>
        /// Inventory records for this book across different sites
        /// </summary>
        [Display(Name = "Inventories")]
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        /// <summary>
        /// Stock movements for this book
        /// </summary>
        [Display(Name = "Stock Movements")]
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

        /// <summary>
        /// Book requisition details that include this book
        /// </summary>
        [Display(Name = "Requisition Details")]
        public virtual ICollection<BookRequisitionDetail> BookRequisitionDetails { get; set; } = new List<BookRequisitionDetail>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{BookCode} - {BookTitle}";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplayName => $"{BookCode} - {BookTitle.Substring(0, Math.Min(BookTitle.Length, 30))}{(BookTitle.Length > 30 ? "..." : "")}";

        /// <summary>
        /// Status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => IsActive ? "🟢 Active" : "🔴 Inactive";

        /// <summary>
        /// Category display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Category Display")]
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

        /// <summary>
        /// Book type based on category and title
        /// </summary>
        [NotMapped]
        [Display(Name = "Book Type")]
        public string BookType
        {
            get
            {
                var title = BookTitle?.ToLower() ?? "";
                var category = Category?.ToLower() ?? "";

                if (category.Contains("method") || title.Contains("method"))
                    return "Method Book";
                if (category.Contains("theory") || title.Contains("theory"))
                    return "Theory Book";
                if (category.Contains("songbook") || title.Contains("songbook"))
                    return "Songbook";
                if (category.Contains("exercise") || title.Contains("exercise"))
                    return "Exercise Book";
                if (category.Contains("technique") || title.Contains("technique"))
                    return "Technique Book";
                if (title.Contains("grade") || title.Contains("level"))
                    return "Grade Book";

                return "Music Book";
            }
        }

        /// <summary>
        /// Indicates if book has ISBN
        /// </summary>
        [NotMapped]
        [Display(Name = "Has ISBN")]
        public bool HasISBN => !string.IsNullOrEmpty(ISBN);

        /// <summary>
        /// Full book information display
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Book Info")]
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

        /// <summary>
        /// Number of grades using this book
        /// </summary>
        [NotMapped]
        [Display(Name = "Grades Count")]
        public int GradesCount => GradeBooks?.Count ?? 0;

        /// <summary>
        /// Number of sites with this book in inventory
        /// </summary>
        [NotMapped]
        [Display(Name = "Sites Count")]
        public int SitesCount => Inventories?.Count ?? 0;

        /// <summary>
        /// Total stock across all sites
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Stock")]
        public int TotalStock => Inventories?.Sum(i => i.CurrentStock) ?? 0;

        /// <summary>
        /// Average price across all sites
        /// </summary>
        [NotMapped]
        [Display(Name = "Average Price")]
        public decimal AveragePrice
        {
            get
            {
                var activePrices = BookPrices?.Where(bp => bp.IsActive).ToList();
                return activePrices?.Any() == true ? activePrices.Average(bp => bp.Price) : 0;
            }
        }

        /// <summary>
        /// Price range display
        /// </summary>
        [NotMapped]
        [Display(Name = "Price Range")]
        public string PriceRange
        {
            get
            {
                var activePrices = BookPrices?.Where(bp => bp.IsActive).Select(bp => bp.Price).ToList();
                if (activePrices == null || !activePrices.Any()) return "No Price Set";

                var min = activePrices.Min();
                var max = activePrices.Max();

                if (min == max) return $"Rp {min:N0}";
                return $"Rp {min:N0} - Rp {max:N0}";
            }
        }

        /// <summary>
        /// Indicates if book is required for any grade
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Required")]
        public bool IsRequired => GradeBooks?.Any(gb => gb.IsRequired) ?? false;

        /// <summary>
        /// Indicates if book is optional for some grades
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Optional")]
        public bool IsOptional => GradeBooks?.Any(gb => !gb.IsRequired) ?? false;

        /// <summary>
        /// Book usage frequency
        /// </summary>
        [NotMapped]
        [Display(Name = "Usage Frequency")]
        public string UsageFrequency => GradesCount switch
        {
            0 => "Not Used",
            1 => "Single Grade",
            <= 3 => "Few Grades",
            <= 6 => "Multiple Grades",
            _ => "Widely Used"
        };

        /// <summary>
        /// Stock status across all sites
        /// </summary>
        [NotMapped]
        [Display(Name = "Overall Stock Status")]
        public string OverallStockStatus
        {
            get
            {
                if (Inventories == null || !Inventories.Any()) return "No Inventory";

                var lowStockSites = Inventories.Count(i => i.IsLowStock);
                var outOfStockSites = Inventories.Count(i => i.IsOutOfStock);
                var totalSites = Inventories.Count();

                if (outOfStockSites > 0) return $"🔴 {outOfStockSites}/{totalSites} Out of Stock";
                if (lowStockSites > 0) return $"🟡 {lowStockSites}/{totalSites} Low Stock";
                return "🟢 Stock OK";
            }
        }

        /// <summary>
        /// Publication year from creation or estimated
        /// </summary>
        [NotMapped]
        [Display(Name = "Publication Year")]
        public int? PublicationYear => CreatedDate.Year;

        /// <summary>
        /// Book age category
        /// </summary>
        [NotMapped]
        [Display(Name = "Age Category")]
        public string AgeCategory
        {
            get
            {
                var years = DateTime.Now.Year - CreatedDate.Year;
                return years switch
                {
                    0 => "New",
                    <= 2 => "Recent",
                    <= 5 => "Current",
                    <= 10 => "Established",
                    _ => "Classic"
                };
            }
        }

        /// <summary>
        /// Priority based on usage and stock
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority")]
        public string Priority
        {
            get
            {
                if (IsRequired && GradesCount > 3) return "High";
                if (IsRequired || GradesCount > 1) return "Medium";
                return "Normal";
            }
        }

        // Static Constants for Categories
        public static class BookCategories
        {
            public const string Piano = "Piano";
            public const string Violin = "Violin";
            public const string Guitar = "Guitar";
            public const string Theory = "Theory";
            public const string Songbook = "Songbook";
            public const string Exercise = "Exercise";
            public const string Technique = "Technique";
            public const string Method = "Method";
        }

        // Business Logic Methods

        /// <summary>
        /// Validate book business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateBookRules()
        {
            var errors = new List<string>();

            // Book code validation
            if (string.IsNullOrWhiteSpace(BookCode))
            {
                errors.Add("Book code is required");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(BookCode, @"^[A-Z0-9-]+$"))
            {
                errors.Add("Book code must contain only uppercase letters, numbers, and hyphens");
            }

            // Book title validation
            if (string.IsNullOrWhiteSpace(BookTitle))
            {
                errors.Add("Book title is required");
            }

            // ISBN validation (if provided)
            if (!string.IsNullOrEmpty(ISBN))
            {
                if (!IsValidISBN(ISBN))
                {
                    errors.Add("Invalid ISBN format");
                }
            }

            // Category validation
            if (!string.IsNullOrEmpty(Category) && Category.Length > 50)
            {
                errors.Add("Category cannot exceed 50 characters");
            }

            return errors;
        }

        /// <summary>
        /// Validate ISBN format
        /// </summary>
        /// <param name="isbn">ISBN to validate</param>
        /// <returns>True if valid</returns>
        private bool IsValidISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return false;

            // Remove all non-digit characters except X
            var cleaned = new string(isbn.Where(c => char.IsDigit(c) || c == 'X').ToArray());

            // Check for ISBN-10
            if (cleaned.Length == 10)
            {
                return ValidateISBN10(cleaned);
            }

            // Check for ISBN-13
            if (cleaned.Length == 13)
            {
                return ValidateISBN13(cleaned);
            }

            return false;
        }

        /// <summary>
        /// Validate ISBN-10 format
        /// </summary>
        private bool ValidateISBN10(string isbn)
        {
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (isbn[i] - '0') * (10 - i);
            }

            char checkDigit = isbn[9];
            int checkValue = checkDigit == 'X' ? 10 : checkDigit - '0';
            sum += checkValue;

            return sum % 11 == 0;
        }

        /// <summary>
        /// Validate ISBN-13 format
        /// </summary>
        private bool ValidateISBN13(string isbn)
        {
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                int digit = isbn[i] - '0';
                sum += i % 2 == 0 ? digit : digit * 3;
            }

            int checkDigit = (10 - (sum % 10)) % 10;
            return checkDigit == (isbn[12] - '0');
        }

        /// <summary>
        /// Get current price at specific site
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <returns>Current price or null if not found</returns>
        public decimal? GetCurrentPrice(int siteId)
        {
            return BookPrices?
                .Where(bp => bp.SiteId == siteId && bp.IsActive)
                .Where(bp => bp.EffectiveDate <= DateTime.Today)
                .Where(bp => bp.ExpiryDate == null || bp.ExpiryDate >= DateTime.Today)
                .OrderByDescending(bp => bp.EffectiveDate)
                .FirstOrDefault()?.Price;
        }

        /// <summary>
        /// Get inventory status at specific site
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <returns>Inventory or null if not found</returns>
        public Inventory? GetInventoryAtSite(int siteId)
        {
            return Inventories?.FirstOrDefault(i => i.SiteId == siteId);
        }

        /// <summary>
        /// Check if book is available at site
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <param name="quantity">Required quantity</param>
        /// <returns>True if available</returns>
        public bool IsAvailableAtSite(int siteId, int quantity = 1)
        {
            var inventory = GetInventoryAtSite(siteId);
            return inventory?.HasSufficientStock(quantity) ?? false;
        }

        /// <summary>
        /// Get grades that use this book
        /// </summary>
        /// <param name="isRequired">Filter by required status (optional)</param>
        /// <returns>List of grades</returns>
        public List<Grade> GetAssociatedGrades(bool? isRequired = null)
        {
            var gradeBooks = GradeBooks?.AsQueryable();

            if (isRequired.HasValue)
            {
                gradeBooks = gradeBooks?.Where(gb => gb.IsRequired == isRequired.Value);
            }

            return gradeBooks?.OrderBy(gb => gb.SortOrder ?? int.MaxValue)
                             .Select(gb => gb.Grade)
                             .ToList() ?? new List<Grade>();
        }

        /// <summary>
        /// Calculate total revenue from this book
        /// </summary>
        /// <returns>Total revenue across all sites</returns>
        public decimal CalculateTotalRevenue()
        {
            // This would typically involve looking at sales data
            // For now, we'll return 0 as sales data is not in the current schema
            return 0;
        }

        /// <summary>
        /// Get book statistics
        /// </summary>
        /// <returns>Dictionary with book statistics</returns>
        public Dictionary<string, object> GetBookStatistics()
        {
            return new Dictionary<string, object>
            {
                {"BookId", BookId},
                {"BookCode", BookCode},
                {"BookTitle", BookTitle},
                {"Author", Author},
                {"Publisher", Publisher},
                {"Category", Category},
                {"BookType", BookType},
                {"IsActive", IsActive},
                {"GradesCount", GradesCount},
                {"SitesCount", SitesCount},
                {"TotalStock", TotalStock},
                {"AveragePrice", AveragePrice},
                {"PriceRange", PriceRange},
                {"IsRequired", IsRequired},
                {"IsOptional", IsOptional},
                {"UsageFrequency", UsageFrequency},
                {"OverallStockStatus", OverallStockStatus},
                {"Priority", Priority},
                {"AgeCategory", AgeCategory},
                {"HasISBN", HasISBN},
                {"CreatedDate", CreatedDate}
            };
        }

        /// <summary>
        /// Generate unique book code
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="category">Book category</param>
        /// <param name="existingCodes">Existing book codes</param>
        /// <returns>Unique book code</returns>
        public static string GenerateBookCode(string companyCode, string? category, IEnumerable<string> existingCodes)
        {
            var categoryCode = category?.Substring(0, Math.Min(3, category.Length)).ToUpper() ?? "GEN";
            var prefix = $"{companyCode}-{categoryCode}-";

            var existingNumbers = existingCodes
                .Where(c => c.StartsWith(prefix))
                .Select(c =>
                {
                    var parts = c.Split('-');
                    return parts.Length == 3 && int.TryParse(parts[2], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingNumbers.Max() + 1;
            return $"{prefix}{nextNumber:000}";
        }

        /// <summary>
        /// Create a new book
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="bookCode">Book code</param>
        /// <param name="bookTitle">Book title</param>
        /// <param name="author">Author (optional)</param>
        /// <param name="publisher">Publisher (optional)</param>
        /// <param name="isbn">ISBN (optional)</param>
        /// <param name="category">Category (optional)</param>
        /// <param name="description">Description (optional)</param>
        /// <param name="createdBy">Creator user ID</param>
        /// <returns>New book instance</returns>
        public static Book CreateBook(int companyId, string bookCode, string bookTitle,
            string? author = null, string? publisher = null, string? isbn = null,
            string? category = null, string? description = null, int? createdBy = null)
        {
            return new Book
            {
                CompanyId = companyId,
                BookCode = bookCode,
                BookTitle = bookTitle,
                Author = author,
                Publisher = publisher,
                ISBN = isbn,
                Category = category,
                Description = description,
                IsActive = true,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get books by category
        /// </summary>
        /// <param name="books">Collection of books</param>
        /// <param name="category">Category to filter by</param>
        /// <returns>Filtered books</returns>
        public static IEnumerable<Book> GetByCategory(IEnumerable<Book> books, string category)
        {
            return books.Where(b => string.Equals(b.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get active books
        /// </summary>
        /// <param name="books">Collection of books</param>
        /// <returns>Active books only</returns>
        public static IEnumerable<Book> GetActiveBooks(IEnumerable<Book> books)
        {
            return books.Where(b => b.IsActive);
        }

        /// <summary>
        /// Search books by title or code
        /// </summary>
        /// <param name="books">Collection of books</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Matching books</returns>
        public static IEnumerable<Book> SearchBooks(IEnumerable<Book> books, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return books;

            var term = searchTerm.ToLower();
            return books.Where(b =>
                b.BookTitle.ToLower().Contains(term) ||
                b.BookCode.ToLower().Contains(term) ||
                (b.Author?.ToLower().Contains(term) ?? false) ||
                (b.Publisher?.ToLower().Contains(term) ?? false));
        }

        /// <summary>
        /// Calculate book statistics for a collection
        /// </summary>
        /// <param name="books">Collection of books</param>
        /// <returns>Dictionary with collection statistics</returns>
        public static Dictionary<string, object> CalculateCollectionStatistics(IEnumerable<Book> books)
        {
            var bookList = books.ToList();
            var totalBooks = bookList.Count;

            if (totalBooks == 0)
            {
                return new Dictionary<string, object>
                {
                    {"TotalBooks", 0},
                    {"ActiveBooks", 0},
                    {"CategoriesCount", 0}
                };
            }

            var activeBooks = bookList.Count(b => b.IsActive);
            var categoriesCount = bookList.Select(b => b.Category).Distinct().Count();
            var booksWithISBN = bookList.Count(b => b.HasISBN);
            var averageGradesPerBook = bookList.Average(b => b.GradesCount);
            var totalStock = bookList.Sum(b => b.TotalStock);

            var categoryDistribution = bookList
                .Where(b => !string.IsNullOrEmpty(b.Category))
                .GroupBy(b => b.Category)
                .ToDictionary(g => g.Key!, g => g.Count());

            return new Dictionary<string, object>
            {
                {"TotalBooks", totalBooks},
                {"ActiveBooks", activeBooks},
                {"InactiveBooks", totalBooks - activeBooks},
                {"CategoriesCount", categoriesCount},
                {"BooksWithISBN", booksWithISBN},
                {"AverageGradesPerBook", Math.Round(averageGradesPerBook, 2)},
                {"TotalStock", totalStock},
                {"CategoryDistribution", categoryDistribution},
                {"MostUsedBooks", bookList.Where(b => b.GradesCount > 0).OrderByDescending(b => b.GradesCount).Take(5).Select(b => b.DisplayName)},
                {"RequiredBooks", bookList.Count(b => b.IsRequired)},
                {"OptionalBooks", bookList.Count(b => b.IsOptional)}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the book</returns>
        public override string ToString() => DisplayName;
    }
}
