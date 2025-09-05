using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents book inventory/stock management in the KMSI Course Management System
    /// Tracks book quantities, stock levels, and reorder points per site
    /// </summary>
    [Table("Inventory")]
    [Index(nameof(SiteId), nameof(BookId), IsUnique = true, Name = "IX_Inventory_SiteId_BookId")]
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Book is required")]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Current stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Current stock cannot be negative")]
        [Display(Name = "Current Stock")]
        public int CurrentStock { get; set; } = 0;

        [Range(0, 9999, ErrorMessage = "Minimum stock must be between 0 and 9999")]
        [Display(Name = "Minimum Stock")]
        public int MinimumStock { get; set; } = 5;

        [Range(0, 9999, ErrorMessage = "Maximum stock must be between 0 and 9999")]
        [Display(Name = "Maximum Stock")]
        public int MaximumStock { get; set; } = 100;

        [Range(0, 9999, ErrorMessage = "Reorder level must be between 0 and 9999")]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; } = 10;

        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation Properties

        /// <summary>
        /// Company that owns this inventory
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site/location where the inventory is stored
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Book being tracked in inventory
        /// </summary>
        [Required]
        [Display(Name = "Book")]
        public virtual Book Book { get; set; } = null!;

        /// <summary>
        /// Stock movements related to this inventory
        /// </summary>
        [Display(Name = "Stock Movements")]
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining site and book information
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{Site?.SiteName} - {Book?.BookTitle}";

        /// <summary>
        /// Stock status indicator
        /// </summary>
        [NotMapped]
        [Display(Name = "Stock Status")]
        public string StockStatus
        {
            get
            {
                if (CurrentStock <= 0) return "Out of Stock";
                if (CurrentStock <= MinimumStock) return "Low Stock";
                if (CurrentStock <= ReorderLevel) return "Reorder Required";
                if (CurrentStock >= MaximumStock) return "Overstocked";
                return "Normal";
            }
        }

        /// <summary>
        /// Stock status with color coding for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Stock Status Display")]
        public string StockStatusDisplay => StockStatus switch
        {
            "Out of Stock" => "🔴 Out of Stock",
            "Low Stock" => "🟡 Low Stock",
            "Reorder Required" => "🟠 Reorder Required",
            "Overstocked" => "🔵 Overstocked",
            "Normal" => "🟢 Normal",
            _ => StockStatus
        };

        /// <summary>
        /// Stock level percentage (current vs maximum)
        /// </summary>
        [NotMapped]
        [Display(Name = "Stock Level Percentage")]
        public decimal StockLevelPercentage
        {
            get
            {
                if (MaximumStock <= 0) return 0;
                return Math.Round((decimal)CurrentStock / MaximumStock * 100, 2);
            }
        }

        /// <summary>
        /// Available quantity for allocation (above minimum stock)
        /// </summary>
        [NotMapped]
        [Display(Name = "Available Quantity")]
        public int AvailableQuantity => Math.Max(0, CurrentStock - MinimumStock);

        /// <summary>
        /// Quantity needed to reach maximum stock
        /// </summary>
        [NotMapped]
        [Display(Name = "Quantity to Maximum")]
        public int QuantityToMaximum => Math.Max(0, MaximumStock - CurrentStock);

        /// <summary>
        /// Quantity needed to reach reorder level
        /// </summary>
        [NotMapped]
        [Display(Name = "Quantity to Reorder Level")]
        public int QuantityToReorderLevel => Math.Max(0, ReorderLevel - CurrentStock);

        /// <summary>
        /// Indicates if stock is below reorder level
        /// </summary>
        [NotMapped]
        [Display(Name = "Needs Reorder")]
        public bool NeedsReorder => CurrentStock <= ReorderLevel;

        /// <summary>
        /// Indicates if stock is below minimum level
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Low Stock")]
        public bool IsLowStock => CurrentStock <= MinimumStock;

        /// <summary>
        /// Indicates if stock is out
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Out of Stock")]
        public bool IsOutOfStock => CurrentStock <= 0;

        /// <summary>
        /// Indicates if stock exceeds maximum
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Overstocked")]
        public bool IsOverstocked => CurrentStock > MaximumStock;

        /// <summary>
        /// Days since last stock update
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Last Update")]
        public int DaysSinceLastUpdate => (DateTime.Today - LastUpdated.Date).Days;

        /// <summary>
        /// Stock turnover indicator (updated frequently = high turnover)
        /// </summary>
        [NotMapped]
        [Display(Name = "Stock Activity")]
        public string StockActivity
        {
            get
            {
                var days = DaysSinceLastUpdate;
                return days switch
                {
                    0 => "Very Active",
                    <= 7 => "Active",
                    <= 30 => "Moderate",
                    <= 90 => "Slow",
                    _ => "Inactive"
                };
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Validate inventory business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateInventoryRules()
        {
            var errors = new List<string>();

            // Minimum stock should not exceed maximum stock
            if (MinimumStock > MaximumStock)
            {
                errors.Add("Minimum stock cannot exceed maximum stock");
            }

            // Reorder level should be between minimum and maximum
            if (ReorderLevel < MinimumStock)
            {
                errors.Add("Reorder level should not be less than minimum stock");
            }

            if (ReorderLevel > MaximumStock)
            {
                errors.Add("Reorder level should not exceed maximum stock");
            }

            // Current stock validation
            if (CurrentStock < 0)
            {
                errors.Add("Current stock cannot be negative");
            }

            // Reasonable stock level checks
            if (MaximumStock <= 0)
            {
                errors.Add("Maximum stock must be greater than zero");
            }

            if (MinimumStock < 0)
            {
                errors.Add("Minimum stock cannot be negative");
            }

            return errors;
        }

        /// <summary>
        /// Update stock quantity and last updated timestamp
        /// </summary>
        /// <param name="newQuantity">New stock quantity</param>
        /// <param name="updateTimestamp">Whether to update the timestamp</param>
        /// <returns>True if update was successful</returns>
        public bool UpdateStock(int newQuantity, bool updateTimestamp = true)
        {
            if (newQuantity < 0)
                return false;

            CurrentStock = newQuantity;

            if (updateTimestamp)
                LastUpdated = DateTime.Now;

            return true;
        }

        /// <summary>
        /// Add stock quantity (stock in)
        /// </summary>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>True if successful</returns>
        public bool AddStock(int quantity)
        {
            if (quantity <= 0)
                return false;

            CurrentStock += quantity;
            LastUpdated = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Remove stock quantity (stock out)
        /// </summary>
        /// <param name="quantity">Quantity to remove</param>
        /// <returns>True if successful, false if insufficient stock</returns>
        public bool RemoveStock(int quantity)
        {
            if (quantity <= 0 || quantity > CurrentStock)
                return false;

            CurrentStock -= quantity;
            LastUpdated = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Check if sufficient stock is available for allocation
        /// </summary>
        /// <param name="requiredQuantity">Required quantity</param>
        /// <param name="respectMinimumStock">Whether to respect minimum stock level</param>
        /// <returns>True if sufficient stock is available</returns>
        public bool HasSufficientStock(int requiredQuantity, bool respectMinimumStock = true)
        {
            if (requiredQuantity <= 0)
                return true;

            var availableStock = respectMinimumStock ? AvailableQuantity : CurrentStock;
            return availableStock >= requiredQuantity;
        }

        /// <summary>
        /// Calculate suggested reorder quantity
        /// </summary>
        /// <returns>Suggested quantity to order</returns>
        public int CalculateSuggestedReorderQuantity()
        {
            if (!NeedsReorder)
                return 0;

            // Suggest ordering enough to reach maximum stock
            return QuantityToMaximum;
        }

        /// <summary>
        /// Get stock alert level
        /// </summary>
        /// <returns>Alert level (0 = Normal, 1 = Warning, 2 = Critical, 3 = Emergency)</returns>
        public int GetStockAlertLevel()
        {
            if (CurrentStock <= 0)
                return 3; // Emergency - Out of stock

            if (CurrentStock <= MinimumStock)
                return 2; // Critical - Below minimum

            if (CurrentStock <= ReorderLevel)
                return 1; // Warning - Below reorder level

            return 0; // Normal
        }

        /// <summary>
        /// Generate stock summary for reporting
        /// </summary>
        /// <returns>Dictionary with stock summary data</returns>
        public Dictionary<string, object> GenerateStockSummary()
        {
            return new Dictionary<string, object>
            {
                {"InventoryId", InventoryId},
                {"SiteName", Site?.SiteName},
                {"BookTitle", Book?.BookTitle},
                {"BookCode", Book?.BookCode},
                {"CurrentStock", CurrentStock},
                {"MinimumStock", MinimumStock},
                {"MaximumStock", MaximumStock},
                {"ReorderLevel", ReorderLevel},
                {"StockStatus", StockStatus},
                {"StockLevelPercentage", StockLevelPercentage},
                {"AvailableQuantity", AvailableQuantity},
                {"NeedsReorder", NeedsReorder},
                {"SuggestedReorderQuantity", CalculateSuggestedReorderQuantity()},
                {"AlertLevel", GetStockAlertLevel()},
                {"LastUpdated", LastUpdated},
                {"DaysSinceLastUpdate", DaysSinceLastUpdate},
                {"StockActivity", StockActivity}
            };
        }

        /// <summary>
        /// Create inventory record for a new book at a site
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="initialStock">Initial stock quantity</param>
        /// <param name="minStock">Minimum stock level</param>
        /// <param name="maxStock">Maximum stock level</param>
        /// <param name="reorderLevel">Reorder level</param>
        /// <returns>New inventory record</returns>
        public static Inventory CreateNewInventory(int companyId, int siteId, int bookId,
            int initialStock = 0, int minStock = 5, int maxStock = 100, int reorderLevel = 10)
        {
            return new Inventory
            {
                CompanyId = companyId,
                SiteId = siteId,
                BookId = bookId,
                CurrentStock = initialStock,
                MinimumStock = minStock,
                MaximumStock = maxStock,
                ReorderLevel = reorderLevel,
                LastUpdated = DateTime.Now
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the inventory</returns>
        public override string ToString() => DisplayName;
    }
}
