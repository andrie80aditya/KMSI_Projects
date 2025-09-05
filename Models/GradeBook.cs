using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents the mapping between grades and books in the KMSI Course Management System
    /// Defines which books are required or optional for each grade level
    /// </summary>
    [Table("GradeBooks")]
    [Index(nameof(GradeId), nameof(BookId), IsUnique = true, Name = "IX_GradeBooks_GradeId_BookId")]
    public class GradeBook
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GradeBookId { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        [Display(Name = "Grade")]
        [ForeignKey("Grade")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Book is required")]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Display(Name = "Is Required")]
        public bool IsRequired { get; set; } = true;

        [Range(1, 999, ErrorMessage = "Quantity must be between 1 and 999")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; } = 1;

        [Range(0, 9999, ErrorMessage = "Sort order must be between 0 and 9999")]
        [Display(Name = "Sort Order")]
        public int? SortOrder { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Grade that requires this book
        /// </summary>
        [Required]
        [Display(Name = "Grade")]
        public virtual Grade Grade { get; set; } = null!;

        /// <summary>
        /// Book that is required for the grade
        /// </summary>
        [Required]
        [Display(Name = "Book")]
        public virtual Book Book { get; set; } = null!;

        /// <summary>
        /// User who created this grade-book mapping
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining grade and book information
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{Grade?.GradeName} - {Book?.BookTitle}";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display Name")]
        public string ShortDisplayName => $"{Grade?.GradeCode} - {Book?.BookCode}";

        /// <summary>
        /// Book type display with requirement indicator
        /// </summary>
        [NotMapped]
        [Display(Name = "Book Type")]
        public string BookTypeDisplay => IsRequired ? "📚 Required" : "📖 Optional";

        /// <summary>
        /// Requirement status for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Requirement Status")]
        public string RequirementStatus => IsRequired ? "Required" : "Optional";

        /// <summary>
        /// Complete description including quantity
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Description")]
        public string FullDescription
        {
            get
            {
                var requirement = IsRequired ? "Required" : "Optional";
                var quantityText = Quantity > 1 ? $" (Qty: {Quantity})" : "";
                return $"{Book?.BookTitle} - {requirement}{quantityText}";
            }
        }

        /// <summary>
        /// Indicates if this is a primary/main book (lower sort order)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Primary Book")]
        public bool IsPrimaryBook => SortOrder.HasValue && SortOrder <= 3;

        /// <summary>
        /// Book category from the associated book
        /// </summary>
        [NotMapped]
        [Display(Name = "Book Category")]
        public string BookCategory => Book?.Category ?? "Uncategorized";

        /// <summary>
        /// Total quantity needed (for reporting)
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Quantity Needed")]
        public int TotalQuantityNeeded => Quantity;

        /// <summary>
        /// Indicates if book has multiple copies required
        /// </summary>
        [NotMapped]
        [Display(Name = "Multiple Copies")]
        public bool HasMultipleCopies => Quantity > 1;

        // Business Logic Methods

        /// <summary>
        /// Validate grade-book mapping business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateGradeBookRules()
        {
            var errors = new List<string>();

            // Quantity validation
            if (Quantity <= 0)
            {
                errors.Add("Quantity must be greater than zero");
            }

            if (Quantity > 10)
            {
                errors.Add("Quantity seems excessive (>10), please verify");
            }

            // Sort order validation
            if (SortOrder.HasValue && SortOrder < 0)
            {
                errors.Add("Sort order cannot be negative");
            }

            // Business logic: Required books should generally have lower sort order
            if (IsRequired && SortOrder.HasValue && SortOrder > 100)
            {
                errors.Add("Required books should typically have lower sort order for better organization");
            }

            return errors;
        }

        /// <summary>
        /// Calculate total cost for this book mapping at a specific site
        /// </summary>
        /// <param name="siteId">Site ID for pricing lookup</param>
        /// <returns>Total cost (price × quantity)</returns>
        public decimal CalculateTotalCost(int siteId)
        {
            var currentPrice = Book?.BookPrices?
                .Where(bp => bp.SiteId == siteId && bp.IsActive)
                .Where(bp => bp.EffectiveDate <= DateTime.Today)
                .Where(bp => bp.ExpiryDate == null || bp.ExpiryDate >= DateTime.Today)
                .OrderByDescending(bp => bp.EffectiveDate)
                .FirstOrDefault();

            return currentPrice?.Price * Quantity ?? 0;
        }

        /// <summary>
        /// Get the current active price for this book at a site
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <returns>Current price or null if not found</returns>
        public decimal? GetCurrentPrice(int siteId)
        {
            return Book?.BookPrices?
                .Where(bp => bp.SiteId == siteId && bp.IsActive)
                .Where(bp => bp.EffectiveDate <= DateTime.Today)
                .Where(bp => bp.ExpiryDate == null || bp.ExpiryDate >= DateTime.Today)
                .OrderByDescending(bp => bp.EffectiveDate)
                .FirstOrDefault()?.Price;
        }

        /// <summary>
        /// Check if this book is available in sufficient quantity at a site
        /// </summary>
        /// <param name="siteId">Site ID to check inventory</param>
        /// <param name="studentsCount">Number of students needing the book</param>
        /// <returns>True if sufficient inventory available</returns>
        public bool IsAvailableAtSite(int siteId, int studentsCount = 1)
        {
            var inventory = Book?.Inventories?.FirstOrDefault(i => i.SiteId == siteId);
            if (inventory == null) return false;

            var totalNeeded = Quantity * studentsCount;
            return inventory.HasSufficientStock(totalNeeded, true);
        }

        /// <summary>
        /// Get inventory status for this book at a site
        /// </summary>
        /// <param name="siteId">Site ID</param>
        /// <returns>Inventory status summary</returns>
        public string GetInventoryStatus(int siteId)
        {
            var inventory = Book?.Inventories?.FirstOrDefault(i => i.SiteId == siteId);
            if (inventory == null) return "No Inventory";

            return inventory.StockStatus;
        }

        /// <summary>
        /// Create a new grade-book mapping
        /// </summary>
        /// <param name="gradeId">Grade ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="isRequired">Whether the book is required</param>
        /// <param name="quantity">Quantity needed</param>
        /// <param name="sortOrder">Sort order</param>
        /// <param name="createdBy">User ID who created the mapping</param>
        /// <returns>New GradeBook instance</returns>
        public static GradeBook CreateMapping(int gradeId, int bookId, bool isRequired = true,
            int quantity = 1, int? sortOrder = null, int? createdBy = null)
        {
            return new GradeBook
            {
                GradeId = gradeId,
                BookId = bookId,
                IsRequired = isRequired,
                Quantity = quantity,
                SortOrder = sortOrder,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Update book requirement status
        /// </summary>
        /// <param name="isRequired">New requirement status</param>
        /// <param name="updateSortOrder">Whether to update sort order based on requirement</param>
        public void UpdateRequirement(bool isRequired, bool updateSortOrder = true)
        {
            IsRequired = isRequired;

            if (updateSortOrder)
            {
                // If making required, move to beginning; if making optional, move to end
                if (isRequired && (!SortOrder.HasValue || SortOrder > 50))
                {
                    SortOrder = 1;
                }
                else if (!isRequired && (!SortOrder.HasValue || SortOrder <= 50))
                {
                    SortOrder = 100;
                }
            }
        }

        /// <summary>
        /// Update quantity with validation
        /// </summary>
        /// <param name="newQuantity">New quantity</param>
        /// <returns>True if update was successful</returns>
        public bool UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0 || newQuantity > 999)
                return false;

            Quantity = newQuantity;
            return true;
        }

        /// <summary>
        /// Generate book requirement summary for a grade
        /// </summary>
        /// <returns>Dictionary with book requirement data</returns>
        public Dictionary<string, object> GenerateBookSummary()
        {
            return new Dictionary<string, object>
            {
                {"GradeBookId", GradeBookId},
                {"GradeName", Grade?.GradeName},
                {"GradeCode", Grade?.GradeCode},
                {"BookTitle", Book?.BookTitle},
                {"BookCode", Book?.BookCode},
                {"BookCategory", BookCategory},
                {"IsRequired", IsRequired},
                {"RequirementStatus", RequirementStatus},
                {"Quantity", Quantity},
                {"SortOrder", SortOrder},
                {"IsPrimaryBook", IsPrimaryBook},
                {"HasMultipleCopies", HasMultipleCopies},
                {"Author", Book?.Author},
                {"Publisher", Book?.Publisher},
                {"ISBN", Book?.ISBN},
                {"CreatedDate", CreatedDate}
            };
        }

        /// <summary>
        /// Get books grouped by requirement status for a grade
        /// </summary>
        /// <param name="gradeBooks">Collection of grade books</param>
        /// <returns>Grouped books by requirement</returns>
        public static Dictionary<string, List<GradeBook>> GroupByRequirement(IEnumerable<GradeBook> gradeBooks)
        {
            return gradeBooks.GroupBy(gb => gb.RequirementStatus)
                           .ToDictionary(g => g.Key, g => g.OrderBy(gb => gb.SortOrder ?? int.MaxValue)
                                                          .ThenBy(gb => gb.Book?.BookTitle)
                                                          .ToList());
        }

        /// <summary>
        /// Calculate total book cost for a grade at a site
        /// </summary>
        /// <param name="gradeBooks">Grade books for the grade</param>
        /// <param name="siteId">Site ID for pricing</param>
        /// <param name="includeOptional">Include optional books</param>
        /// <returns>Total cost</returns>
        public static decimal CalculateGradeTotalCost(IEnumerable<GradeBook> gradeBooks, int siteId, bool includeOptional = false)
        {
            var relevantBooks = includeOptional
                ? gradeBooks
                : gradeBooks.Where(gb => gb.IsRequired);

            return relevantBooks.Sum(gb => gb.CalculateTotalCost(siteId));
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the grade book mapping</returns>
        public override string ToString() => DisplayName;
    }
}
