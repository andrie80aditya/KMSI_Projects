using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents stock movement record in the KMSI Course Management System
    /// Tracks all inventory transactions including stock in, out, transfers, and adjustments
    /// </summary>
    [Table("StockMovements")]
    public class StockMovement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockMovementId { get; set; }

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

        [Required(ErrorMessage = "Movement type is required")]
        [StringLength(20, ErrorMessage = "Movement type cannot exceed 20 characters")]
        [Display(Name = "Movement Type")]
        public string MovementType { get; set; } = string.Empty; // Stock In, Stock Out, Transfer In, Transfer Out, Adjustment

        [Required(ErrorMessage = "Quantity is required")]
        [Range(-99999, 99999, ErrorMessage = "Quantity must be between -99,999 and 99,999")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [StringLength(20, ErrorMessage = "Reference type cannot exceed 20 characters")]
        [Display(Name = "Reference Type")]
        public string? ReferenceType { get; set; } // Registration, Requisition, Transfer, Adjustment

        [Display(Name = "Reference ID")]
        public int? ReferenceId { get; set; }

        [Display(Name = "From Site")]
        [ForeignKey("FromSite")]
        public int? FromSiteId { get; set; }

        [Display(Name = "To Site")]
        [ForeignKey("ToSite")]
        public int? ToSiteId { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Unit cost must be between 0 and 9,999,999.99")]
        [Display(Name = "Unit Cost")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// Total cost computed column (Quantity * UnitCost)
        /// </summary>
        [Range(-999999999.99, 999999999.99, ErrorMessage = "Total cost must be between -999,999,999.99 and 999,999,999.99")]
        [Display(Name = "Total Cost")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? TotalCost { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Display(Name = "Movement Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime MovementDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Company this stock movement belongs to
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site where this stock movement occurred
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// Book involved in this stock movement
        /// </summary>
        [Required]
        [Display(Name = "Book")]
        public virtual Book Book { get; set; } = null!;

        /// <summary>
        /// Source site for transfers (Transfer Out)
        /// </summary>
        [Display(Name = "From Site")]
        public virtual Site? FromSite { get; set; }

        /// <summary>
        /// Destination site for transfers (Transfer In)
        /// </summary>
        [Display(Name = "To Site")]
        public virtual Site? ToSite { get; set; }

        /// <summary>
        /// User who created this stock movement record
        /// </summary>
        [InverseProperty("CreatedStockMovements")]
        public virtual User? CreatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining movement type and book
        /// </summary>
        [NotMapped]
        [Display(Name = "Stock Movement")]
        public string DisplayName => $"{MovementType} - {Book?.BookTitle ?? "Unknown Book"} ({Quantity})";

        /// <summary>
        /// Short display for lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Movement")]
        public string ShortDisplay => $"{MovementType} ({Quantity:+#;-#;0}) - {Book?.BookCode}";

        /// <summary>
        /// Movement type display with color information
        /// </summary>
        [NotMapped]
        [Display(Name = "Movement Type")]
        public string MovementTypeDisplay => MovementType;

        /// <summary>
        /// Movement type color for UI styling
        /// </summary>
        [NotMapped]
        [Display(Name = "Movement Color")]
        public string MovementTypeColor => MovementType switch
        {
            "Stock In" => "success",
            "Transfer In" => "info",
            "Stock Out" => "warning",
            "Transfer Out" => "primary",
            "Adjustment" => "secondary",
            _ => "dark"
        };

        /// <summary>
        /// Movement type icon
        /// </summary>
        [NotMapped]
        [Display(Name = "Movement Icon")]
        public string MovementTypeIcon => MovementType switch
        {
            "Stock In" => "fa-arrow-down",
            "Transfer In" => "fa-arrow-right",
            "Stock Out" => "fa-arrow-up",
            "Transfer Out" => "fa-arrow-left",
            "Adjustment" => "fa-edit",
            _ => "fa-exchange"
        };

        /// <summary>
        /// Reference display combining type and ID
        /// </summary>
        [NotMapped]
        [Display(Name = "Reference")]
        public string ReferenceDisplay => !string.IsNullOrEmpty(ReferenceType) && ReferenceId.HasValue
            ? $"{ReferenceType} #{ReferenceId}"
            : "Manual Entry";

        /// <summary>
        /// Quantity display with sign and formatting
        /// </summary>
        [NotMapped]
        [Display(Name = "Quantity")]
        public string QuantityDisplay => Quantity >= 0 ? $"+{Quantity}" : Quantity.ToString();

        /// <summary>
        /// Check if this is a positive movement (increases stock)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Positive Movement")]
        public bool IsPositiveMovement => MovementType is "Stock In" or "Transfer In" ||
            (MovementType == "Adjustment" && Quantity > 0);

        /// <summary>
        /// Check if this is a negative movement (decreases stock)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Negative Movement")]
        public bool IsNegativeMovement => MovementType is "Stock Out" or "Transfer Out" ||
            (MovementType == "Adjustment" && Quantity < 0);

        /// <summary>
        /// Check if this is a transfer movement
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Transfer")]
        public bool IsTransfer => MovementType is "Transfer In" or "Transfer Out";

        /// <summary>
        /// Check if this is an adjustment
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Adjustment")]
        public bool IsAdjustment => MovementType == "Adjustment";

        /// <summary>
        /// Transfer direction display
        /// </summary>
        [NotMapped]
        [Display(Name = "Transfer Direction")]
        public string TransferDirectionDisplay
        {
            get
            {
                if (!IsTransfer) return "N/A";

                if (MovementType == "Transfer Out" && ToSite != null)
                    return $"{Site?.SiteName} → {ToSite.SiteName}";

                if (MovementType == "Transfer In" && FromSite != null)
                    return $"{FromSite.SiteName} → {Site?.SiteName}";

                return "Transfer";
            }
        }

        /// <summary>
        /// Days since movement
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Ago")]
        public int DaysAgo => (DateTime.Now.Date - MovementDate.Date).Days;

        /// <summary>
        /// Time ago display
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Ago")]
        public string TimeAgoDisplay
        {
            get
            {
                var timeSpan = DateTime.Now - MovementDate;

                if (timeSpan.TotalMinutes < 1) return "Just now";
                if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
                if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";

                return MovementDate.ToString("MMM dd, yyyy");
            }
        }

        /// <summary>
        /// Unit cost display
        /// </summary>
        [NotMapped]
        [Display(Name = "Unit Cost")]
        public string UnitCostDisplay => UnitCost?.ToString("C") ?? "Not Specified";

        /// <summary>
        /// Total cost display
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Cost")]
        public string TotalCostDisplay => TotalCost?.ToString("C") ?? "Not Calculated";

        /// <summary>
        /// Movement impact on inventory (+ or -)
        /// </summary>
        [NotMapped]
        [Display(Name = "Inventory Impact")]
        public int InventoryImpact => IsPositiveMovement ? Math.Abs(Quantity) : -Math.Abs(Quantity);

        /// <summary>
        /// Cost per unit average
        /// </summary>
        [NotMapped]
        [Display(Name = "Avg Cost Per Unit")]
        [DataType(DataType.Currency)]
        public decimal? AverageCostPerUnit => Quantity != 0 && TotalCost.HasValue ? Math.Abs(TotalCost.Value / Quantity) : null;

        // Business Logic Methods

        /// <summary>
        /// Validate stock movement business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateStockMovementRules()
        {
            var errors = new List<string>();

            // Transfer validations
            if (IsTransfer)
            {
                if (MovementType == "Transfer Out" && ToSiteId == null)
                    errors.Add("Transfer Out movements must specify destination site");

                if (MovementType == "Transfer In" && FromSiteId == null)
                    errors.Add("Transfer In movements must specify source site");

                if (MovementType == "Transfer Out" && ToSiteId == SiteId)
                    errors.Add("Cannot transfer to the same site");

                if (MovementType == "Transfer In" && FromSiteId == SiteId)
                    errors.Add("Cannot transfer from the same site");
            }

            // Quantity validations
            if (Quantity == 0)
                errors.Add("Quantity cannot be zero");

            // Movement type validations
            var validMovementTypes = new[] { "Stock In", "Stock Out", "Transfer In", "Transfer Out", "Adjustment" };
            if (!validMovementTypes.Contains(MovementType))
                errors.Add($"Invalid movement type: {MovementType}");

            // Cost validations
            if (UnitCost.HasValue && UnitCost < 0)
                errors.Add("Unit cost cannot be negative");

            // Reference validations
            if (!string.IsNullOrEmpty(ReferenceType) && !ReferenceId.HasValue)
                errors.Add("Reference type specified but reference ID is missing");

            return errors;
        }

        /// <summary>
        /// Create stock in movement
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="quantity">Quantity received</param>
        /// <param name="unitCost">Cost per unit</param>
        /// <param name="description">Movement description</param>
        /// <param name="createdBy">User who created the movement</param>
        /// <returns>New stock in movement</returns>
        public static StockMovement CreateStockIn(int companyId, int siteId, int bookId, int quantity,
            decimal? unitCost = null, string? description = null, int? createdBy = null)
        {
            return new StockMovement
            {
                CompanyId = companyId,
                SiteId = siteId,
                BookId = bookId,
                MovementType = "Stock In",
                Quantity = Math.Abs(quantity), // Ensure positive
                UnitCost = unitCost,
                Description = description ?? "Stock received",
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Create stock out movement
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="quantity">Quantity issued</param>
        /// <param name="referenceType">Reference type (Registration, etc.)</param>
        /// <param name="referenceId">Reference ID</param>
        /// <param name="description">Movement description</param>
        /// <param name="createdBy">User who created the movement</param>
        /// <returns>New stock out movement</returns>
        public static StockMovement CreateStockOut(int companyId, int siteId, int bookId, int quantity,
            string? referenceType = null, int? referenceId = null, string? description = null, int? createdBy = null)
        {
            return new StockMovement
            {
                CompanyId = companyId,
                SiteId = siteId,
                BookId = bookId,
                MovementType = "Stock Out",
                Quantity = Math.Abs(quantity), // Store as positive, interpretation comes from type
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Description = description ?? "Stock issued",
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Create transfer out movement
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="fromSiteId">Source site ID</param>
        /// <param name="toSiteId">Destination site ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="quantity">Quantity transferred</param>
        /// <param name="referenceType">Reference type (Requisition, etc.)</param>
        /// <param name="referenceId">Reference ID</param>
        /// <param name="description">Transfer description</param>
        /// <param name="createdBy">User who created the movement</param>
        /// <returns>New transfer out movement</returns>
        public static StockMovement CreateTransferOut(int companyId, int fromSiteId, int toSiteId, int bookId,
            int quantity, string? referenceType = null, int? referenceId = null, string? description = null, int? createdBy = null)
        {
            return new StockMovement
            {
                CompanyId = companyId,
                SiteId = fromSiteId,
                ToSiteId = toSiteId,
                BookId = bookId,
                MovementType = "Transfer Out",
                Quantity = Math.Abs(quantity),
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Description = description ?? $"Transfer to site {toSiteId}",
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Create transfer in movement
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="fromSiteId">Source site ID</param>
        /// <param name="toSiteId">Destination site ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="quantity">Quantity received</param>
        /// <param name="referenceType">Reference type (Requisition, etc.)</param>
        /// <param name="referenceId">Reference ID</param>
        /// <param name="description">Transfer description</param>
        /// <param name="createdBy">User who created the movement</param>
        /// <returns>New transfer in movement</returns>
        public static StockMovement CreateTransferIn(int companyId, int fromSiteId, int toSiteId, int bookId,
            int quantity, string? referenceType = null, int? referenceId = null, string? description = null, int? createdBy = null)
        {
            return new StockMovement
            {
                CompanyId = companyId,
                SiteId = toSiteId,
                FromSiteId = fromSiteId,
                BookId = bookId,
                MovementType = "Transfer In",
                Quantity = Math.Abs(quantity),
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                Description = description ?? $"Transfer from site {fromSiteId}",
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Create stock adjustment movement
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="adjustmentQuantity">Adjustment quantity (positive or negative)</param>
        /// <param name="reason">Reason for adjustment</param>
        /// <param name="createdBy">User who created the movement</param>
        /// <returns>New adjustment movement</returns>
        public static StockMovement CreateAdjustment(int companyId, int siteId, int bookId, int adjustmentQuantity,
            string reason, int? createdBy = null)
        {
            return new StockMovement
            {
                CompanyId = companyId,
                SiteId = siteId,
                BookId = bookId,
                MovementType = "Adjustment",
                Quantity = adjustmentQuantity, // Keep sign for adjustments
                ReferenceType = "Adjustment",
                Description = reason,
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Create complete transfer (both out and in movements)
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="fromSiteId">Source site ID</param>
        /// <param name="toSiteId">Destination site ID</param>
        /// <param name="bookId">Book ID</param>
        /// <param name="quantity">Quantity to transfer</param>
        /// <param name="referenceType">Reference type</param>
        /// <param name="referenceId">Reference ID</param>
        /// <param name="description">Transfer description</param>
        /// <param name="createdBy">User who created the movements</param>
        /// <returns>Pair of transfer movements (out, in)</returns>
        public static (StockMovement TransferOut, StockMovement TransferIn) CreateCompleteTransfer(
            int companyId, int fromSiteId, int toSiteId, int bookId, int quantity,
            string? referenceType = null, int? referenceId = null, string? description = null, int? createdBy = null)
        {
            var transferOut = CreateTransferOut(companyId, fromSiteId, toSiteId, bookId, quantity,
                referenceType, referenceId, description, createdBy);

            var transferIn = CreateTransferIn(companyId, fromSiteId, toSiteId, bookId, quantity,
                referenceType, referenceId, description, createdBy);

            return (transferOut, transferIn);
        }

        /// <summary>
        /// Get stock movements summary for a book and site
        /// </summary>
        /// <param name="movements">Stock movements to analyze</param>
        /// <param name="bookId">Book ID to filter by</param>
        /// <param name="siteId">Site ID to filter by</param>
        /// <returns>Stock movement summary</returns>
        public static Dictionary<string, object> GetBookSiteMovementSummary(IEnumerable<StockMovement> movements,
            int bookId, int siteId)
        {
            var relevantMovements = movements
                .Where(m => m.BookId == bookId && m.SiteId == siteId)
                .OrderByDescending(m => m.MovementDate)
                .ToList();

            var stockIn = relevantMovements.Where(m => m.IsPositiveMovement).ToList();
            var stockOut = relevantMovements.Where(m => m.IsNegativeMovement).ToList();

            return new Dictionary<string, object>
            {
                {"BookId", bookId},
                {"SiteId", siteId},
                {"TotalMovements", relevantMovements.Count},
                {"StockIn", new
                {
                    Count = stockIn.Count,
                    Quantity = stockIn.Sum(m => Math.Abs(m.Quantity)),
                    Value = stockIn.Sum(m => m.TotalCost ?? 0)
                }},
                {"StockOut", new
                {
                    Count = stockOut.Count,
                    Quantity = stockOut.Sum(m => Math.Abs(m.Quantity)),
                    Value = stockOut.Sum(m => m.TotalCost ?? 0)
                }},
                {"NetQuantity", stockIn.Sum(m => Math.Abs(m.Quantity)) - stockOut.Sum(m => Math.Abs(m.Quantity))},
                {"LastMovement", relevantMovements.FirstOrDefault()?.MovementDate},
                {"AverageUnitCost", relevantMovements.Where(m => m.UnitCost.HasValue).Average(m => m.UnitCost)},
                {"RecentMovements", relevantMovements.Take(5).Select(m => new
                {
                    m.MovementType,
                    m.Quantity,
                    m.MovementDate,
                    m.Description
                }).ToList()}
            };
        }

        /// <summary>
        /// Get inventory valuation from stock movements
        /// </summary>
        /// <param name="movements">Stock movements for valuation</param>
        /// <param name="siteId">Site ID to filter by (optional)</param>
        /// <returns>Inventory valuation data</returns>
        public static Dictionary<string, object> GetInventoryValuation(IEnumerable<StockMovement> movements,
            int? siteId = null)
        {
            var relevantMovements = siteId.HasValue
                ? movements.Where(m => m.SiteId == siteId.Value)
                : movements;

            var movementsList = relevantMovements.Where(m => m.UnitCost.HasValue).ToList();

            var byBook = movementsList
                .GroupBy(m => m.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    Book = g.First().Book,
                    TotalIn = g.Where(m => m.IsPositiveMovement).Sum(m => Math.Abs(m.Quantity)),
                    TotalOut = g.Where(m => m.IsNegativeMovement).Sum(m => Math.Abs(m.Quantity)),
                    CurrentStock = g.Where(m => m.IsPositiveMovement).Sum(m => Math.Abs(m.Quantity)) -
                                  g.Where(m => m.IsNegativeMovement).Sum(m => Math.Abs(m.Quantity)),
                    TotalValue = g.Sum(m => m.TotalCost ?? 0),
                    AverageUnitCost = g.Average(m => m.UnitCost ?? 0)
                })
                .Where(b => b.CurrentStock > 0)
                .ToList();

            return new Dictionary<string, object>
            {
                {"SiteId", siteId},
                {"TotalBooks", byBook.Count},
                {"TotalStockQuantity", byBook.Sum(b => b.CurrentStock)},
                {"TotalStockValue", byBook.Sum(b => b.CurrentStock * b.AverageUnitCost)},
                {"BookValuations", byBook.OrderByDescending(b => b.TotalValue).ToList()},
                {"MovementsSummary", new
                {
                    TotalMovements = movementsList.Count,
                    TotalValue = movementsList.Sum(m => m.TotalCost ?? 0),
                    AverageMovementValue = movementsList.Average(m => m.TotalCost ?? 0)
                }}
            };
        }

        /// <summary>
        /// Get movement activity report
        /// </summary>
        /// <param name="movements">Movements to analyze</param>
        /// <param name="startDate">Report start date</param>
        /// <param name="endDate">Report end date</param>
        /// <returns>Movement activity report</returns>
        public static Dictionary<string, object> GetMovementActivityReport(IEnumerable<StockMovement> movements,
            DateTime startDate, DateTime endDate)
        {
            var periodMovements = movements
                .Where(m => m.MovementDate >= startDate && m.MovementDate <= endDate)
                .ToList();

            var byType = periodMovements.GroupBy(m => m.MovementType)
                .ToDictionary(g => g.Key, g => new
                {
                    Count = g.Count(),
                    TotalQuantity = g.Sum(m => Math.Abs(m.Quantity)),
                    TotalValue = g.Sum(m => m.TotalCost ?? 0)
                });

            var bySite = periodMovements.GroupBy(m => m.SiteId)
                .ToDictionary(g => g.Key, g => new
                {
                    SiteName = g.First().Site?.SiteName,
                    Count = g.Count(),
                    TotalQuantity = g.Sum(m => Math.Abs(m.Quantity)),
                    TotalValue = g.Sum(m => m.TotalCost ?? 0)
                });

            var dailyActivity = periodMovements
                .GroupBy(m => m.MovementDate.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => new
                {
                    Count = g.Count(),
                    TotalQuantity = g.Sum(m => Math.Abs(m.Quantity)),
                    TotalValue = g.Sum(m => m.TotalCost ?? 0)
                });

            return new Dictionary<string, object>
            {
                {"Period", new { Start = startDate, End = endDate }},
                {"TotalMovements", periodMovements.Count},
                {"TotalQuantity", periodMovements.Sum(m => Math.Abs(m.Quantity))},
                {"TotalValue", periodMovements.Sum(m => m.TotalCost ?? 0)},
                {"ByMovementType", byType},
                {"BySite", bySite},
                {"DailyActivity", dailyActivity},
                {"TopBooks", periodMovements.GroupBy(m => m.BookId)
                    .OrderByDescending(g => g.Sum(m => Math.Abs(m.Quantity)))
                    .Take(10)
                    .Select(g => new
                    {
                        BookId = g.Key,
                        BookTitle = g.First().Book?.BookTitle,
                        TotalQuantity = g.Sum(m => Math.Abs(m.Quantity)),
                        MovementCount = g.Count()
                    }).ToList()}
            };
        }

        /// <summary>
        /// Get movements requiring attention (anomalies, high values, etc.)
        /// </summary>
        /// <param name="movements">Movements to analyze</param>
        /// <param name="highValueThreshold">Threshold for high value movements</param>
        /// <param name="highQuantityThreshold">Threshold for high quantity movements</param>
        /// <returns>Movements requiring attention</returns>
        public static List<object> GetMovementsRequiringAttention(IEnumerable<StockMovement> movements,
            decimal highValueThreshold = 1000m, int highQuantityThreshold = 100)
        {
            var alerts = new List<object>();

            foreach (var movement in movements.Where(m => m.MovementDate >= DateTime.Today.AddDays(-30)))
            {
                var reasons = new List<string>();

                if (movement.TotalCost.HasValue && movement.TotalCost.Value > highValueThreshold)
                    reasons.Add($"High value movement ({movement.TotalCost:C})");

                if (Math.Abs(movement.Quantity) > highQuantityThreshold)
                    reasons.Add($"High quantity movement ({Math.Abs(movement.Quantity)} units)");

                if (movement.MovementType == "Adjustment" && Math.Abs(movement.Quantity) > 10)
                    reasons.Add("Large stock adjustment");

                if (movement.IsTransfer && movement.UnitCost == null)
                    reasons.Add("Transfer without cost information");

                if (movement.MovementType == "Stock Out" && string.IsNullOrEmpty(movement.ReferenceType))
                    reasons.Add("Stock out without reference");

                if (reasons.Any())
                {
                    alerts.Add(new
                    {
                        StockMovementId = movement.StockMovementId,
                        MovementType = movement.MovementType,
                        Book = movement.Book?.BookTitle,
                        Site = movement.Site?.SiteName,
                        Quantity = movement.Quantity,
                        TotalCost = movement.TotalCost,
                        MovementDate = movement.MovementDate,
                        Reasons = reasons,
                        Priority = GetAlertPriority(reasons)
                    });
                }
            }

            return alerts.OrderBy(a => ((dynamic)a).Priority).ToList();
        }

        /// <summary>
        /// Determine alert priority based on reasons
        /// </summary>
        /// <param name="reasons">List of alert reasons</param>
        /// <returns>Priority level (1=highest, 5=lowest)</returns>
        private static int GetAlertPriority(List<string> reasons)
        {
            if (reasons.Any(r => r.Contains("High value"))) return 1;
            if (reasons.Any(r => r.Contains("Large stock adjustment"))) return 2;
            if (reasons.Any(r => r.Contains("High quantity"))) return 3;
            if (reasons.Any(r => r.Contains("without reference"))) return 4;
            return 5;
        }

        /// <summary>
        /// Calculate stock levels from movements
        /// </summary>
        /// <param name="movements">All movements for calculation</param>
        /// <param name="asOfDate">Calculate stock as of this date</param>
        /// <returns>Current stock levels by book and site</returns>
        public static Dictionary<string, Dictionary<string, object>> CalculateStockLevels(
            IEnumerable<StockMovement> movements, DateTime? asOfDate = null)
        {
            asOfDate ??= DateTime.Now;

            var relevantMovements = movements.Where(m => m.MovementDate <= asOfDate).ToList();

            var stockLevels = new Dictionary<string, Dictionary<string, object>>();

            var groupedBySiteAndBook = relevantMovements
                .GroupBy(m => new { m.SiteId, m.BookId })
                .ToList();

            foreach (var group in groupedBySiteAndBook)
            {
                var siteKey = $"Site-{group.Key.SiteId}";
                var bookKey = $"Book-{group.Key.BookId}";

                if (!stockLevels.ContainsKey(siteKey))
                    stockLevels[siteKey] = new Dictionary<string, object>();

                var stockIn = group.Where(m => m.IsPositiveMovement).Sum(m => Math.Abs(m.Quantity));
                var stockOut = group.Where(m => m.IsNegativeMovement).Sum(m => Math.Abs(m.Quantity));
                var currentStock = stockIn - stockOut;

                var lastMovement = group.OrderByDescending(m => m.MovementDate).FirstOrDefault();
                var averageCost = group.Where(m => m.UnitCost.HasValue && m.IsPositiveMovement)
                    .Average(m => m.UnitCost);

                stockLevels[siteKey][bookKey] = new
                {
                    SiteId = group.Key.SiteId,
                    SiteName = group.First().Site?.SiteName,
                    BookId = group.Key.BookId,
                    BookTitle = group.First().Book?.BookTitle,
                    BookCode = group.First().Book?.BookCode,
                    CurrentStock = currentStock,
                    StockIn = stockIn,
                    StockOut = stockOut,
                    LastMovementDate = lastMovement?.MovementDate,
                    LastMovementType = lastMovement?.MovementType,
                    AverageCost = averageCost,
                    TotalValue = currentStock * (averageCost ?? 0),
                    MovementCount = group.Count()
                };
            }

            return stockLevels;
        }

        /// <summary>
        /// Get slow-moving or dead stock analysis
        /// </summary>
        /// <param name="movements">Stock movements to analyze</param>
        /// <param name="slowMovingDays">Days without movement to consider slow-moving</param>
        /// <param name="deadStockDays">Days without movement to consider dead stock</param>
        /// <returns>Slow-moving and dead stock analysis</returns>
        public static Dictionary<string, object> GetSlowMovingStockAnalysis(IEnumerable<StockMovement> movements,
            int slowMovingDays = 90, int deadStockDays = 180)
        {
            var stockLevels = CalculateStockLevels(movements);
            var slowMovingDate = DateTime.Today.AddDays(-slowMovingDays);
            var deadStockDate = DateTime.Today.AddDays(-deadStockDays);

            var slowMovingItems = new List<object>();
            var deadStockItems = new List<object>();

            foreach (var site in stockLevels)
            {
                foreach (var book in site.Value)
                {
                    var bookData = (dynamic)book.Value;
                    var lastMovementDate = (DateTime?)bookData.LastMovementDate;
                    var currentStock = (int)bookData.CurrentStock;

                    if (currentStock > 0 && lastMovementDate.HasValue)
                    {
                        if (lastMovementDate < deadStockDate)
                        {
                            deadStockItems.Add(bookData);
                        }
                        else if (lastMovementDate < slowMovingDate)
                        {
                            slowMovingItems.Add(bookData);
                        }
                    }
                }
            }

            return new Dictionary<string, object>
            {
                {"AnalysisDate", DateTime.Today},
                {"SlowMovingThreshold", $"{slowMovingDays} days"},
                {"DeadStockThreshold", $"{deadStockDays} days"},
                {"SlowMovingItems", new
                {
                    Count = slowMovingItems.Count,
                    Items = slowMovingItems.OrderByDescending(i => ((dynamic)i).TotalValue).ToList()
                }},
                {"DeadStockItems", new
                {
                    Count = deadStockItems.Count,
                    Items = deadStockItems.OrderByDescending(i => ((dynamic)i).TotalValue).ToList()
                }},
                {"TotalSlowMovingValue", slowMovingItems.Sum(i => (decimal)((dynamic)i).TotalValue)},
                {"TotalDeadStockValue", deadStockItems.Sum(i => (decimal)((dynamic)i).TotalValue)}
            };
        }

        /// <summary>
        /// Generate ABC analysis based on movement value
        /// </summary>
        /// <param name="movements">Movements for ABC analysis</param>
        /// <param name="periodStart">Analysis period start</param>
        /// <param name="periodEnd">Analysis period end</param>
        /// <returns>ABC analysis results</returns>
        public static Dictionary<string, object> GenerateABCAnalysis(IEnumerable<StockMovement> movements,
            DateTime periodStart, DateTime periodEnd)
        {
            var periodMovements = movements
                .Where(m => m.MovementDate >= periodStart && m.MovementDate <= periodEnd && m.TotalCost.HasValue)
                .ToList();

            var bookValues = periodMovements
                .GroupBy(m => m.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    BookTitle = g.First().Book?.BookTitle,
                    BookCode = g.First().Book?.BookCode,
                    TotalValue = g.Sum(m => Math.Abs(m.TotalCost ?? 0)),
                    TotalQuantity = g.Sum(m => Math.Abs(m.Quantity)),
                    MovementCount = g.Count()
                })
                .OrderByDescending(b => b.TotalValue)
                .ToList();

            var totalValue = bookValues.Sum(b => b.TotalValue);
            var runningTotal = 0m;

            var abcCategories = bookValues.Select((book, index) =>
            {
                runningTotal += book.TotalValue;
                var cumulativePercentage = runningTotal / totalValue * 100;

                var category = cumulativePercentage <= 80 ? "A" :
                              cumulativePercentage <= 95 ? "B" : "C";

                return new
                {
                    book.BookId,
                    book.BookTitle,
                    book.BookCode,
                    book.TotalValue,
                    book.TotalQuantity,
                    book.MovementCount,
                    ValuePercentage = book.TotalValue / totalValue * 100,
                    CumulativePercentage = cumulativePercentage,
                    Category = category,
                    Rank = index + 1
                };
            }).ToList();

            var categoryStats = abcCategories
                .GroupBy(i => i.Category)
                .ToDictionary(g => g.Key, g => new
                {
                    ItemCount = g.Count(),
                    TotalValue = g.Sum(i => i.TotalValue),
                    ValuePercentage = g.Sum(i => i.ValuePercentage),
                    Items = g.ToList()
                });

            return new Dictionary<string, object>
            {
                {"Period", new { Start = periodStart, End = periodEnd }},
                {"TotalValue", totalValue},
                {"TotalItems", bookValues.Count},
                {"CategoryBreakdown", categoryStats},
                {"ABCItems", abcCategories},
                {"Recommendations", new
                {
                    CategoryA = "Focus on tight control, frequent monitoring, and accurate forecasting",
                    CategoryB = "Regular monitoring with moderate control measures",
                    CategoryC = "Simple controls with periodic review"
                }}
            };
        }

        /// <summary>
        /// Get transfer efficiency report
        /// </summary>
        /// <param name="movements">Transfer movements to analyze</param>
        /// <param name="startDate">Analysis period start</param>
        /// <param name="endDate">Analysis period end</param>
        /// <returns>Transfer efficiency analysis</returns>
        public static Dictionary<string, object> GetTransferEfficiencyReport(IEnumerable<StockMovement> movements,
            DateTime startDate, DateTime endDate)
        {
            var transfers = movements
                .Where(m => m.IsTransfer && m.MovementDate >= startDate && m.MovementDate <= endDate)
                .ToList();

            var transferPairs = transfers
                .Where(m => m.MovementType == "Transfer Out")
                .Select(transferOut => new
                {
                    TransferOut = transferOut,
                    TransferIn = transfers.FirstOrDefault(t =>
                        t.MovementType == "Transfer In" &&
                        t.BookId == transferOut.BookId &&
                        t.FromSiteId == transferOut.SiteId &&
                        t.SiteId == transferOut.ToSiteId &&
                        Math.Abs((t.MovementDate - transferOut.MovementDate).TotalDays) <= 7)
                })
                .Where(pair => pair.TransferIn != null)
                .ToList();

            var completedTransfers = transferPairs.Count;
            var pendingTransfers = transfers.Count(m => m.MovementType == "Transfer Out") - completedTransfers;

            var averageTransferTime = transferPairs
                .Where(p => p.TransferIn != null)
                .Average(p => (p.TransferIn!.MovementDate - p.TransferOut.MovementDate).TotalDays);

            var transferRoutes = transferPairs
                .GroupBy(p => new { From = p.TransferOut.SiteId, To = p.TransferOut.ToSiteId })
                .Select(g => new
                {
                    FromSiteId = g.Key.From,
                    ToSiteId = g.Key.To,
                    FromSiteName = g.First().TransferOut.Site?.SiteName,
                    ToSiteName = g.First().TransferOut.ToSite?.SiteName,
                    TransferCount = g.Count(),
                    TotalQuantity = g.Sum(p => Math.Abs(p.TransferOut.Quantity)),
                    AverageTransferTime = g.Average(p => (p.TransferIn!.MovementDate - p.TransferOut.MovementDate).TotalDays)
                })
                .OrderByDescending(r => r.TransferCount)
                .ToList();

            return new Dictionary<string, object>
            {
                {"Period", new { Start = startDate, End = endDate }},
                {"TotalTransfers", transfers.Count},
                {"CompletedTransfers", completedTransfers},
                {"PendingTransfers", pendingTransfers},
                {"CompletionRate", transfers.Count > 0 ? (double)completedTransfers / (transfers.Count / 2) : 0},
                {"AverageTransferTime", averageTransferTime},
                {"TransferRoutes", transferRoutes},
                {"TopTransferredBooks", transfers
                    .GroupBy(m => m.BookId)
                    .OrderByDescending(g => g.Sum(m => Math.Abs(m.Quantity)))
                    .Take(10)
                    .Select(g => new
                    {
                        BookId = g.Key,
                        BookTitle = g.First().Book?.BookTitle,
                        TotalQuantity = g.Sum(m => Math.Abs(m.Quantity)),
                        TransferCount = g.Count()
                    }).ToList()}
            };
        }

        /// <summary>
        /// Get movement summary for dashboard
        /// </summary>
        /// <returns>Stock movement summary data</returns>
        public Dictionary<string, object> GetMovementSummary()
        {
            return new Dictionary<string, object>
            {
                {"StockMovementId", StockMovementId},
                {"MovementType", MovementType},
                {"MovementTypeColor", MovementTypeColor},
                {"MovementTypeIcon", MovementTypeIcon},
                {"Book", new { Book?.BookId, Book?.BookCode, Book?.BookTitle }},
                {"Site", Site?.SiteName},
                {"Quantity", Quantity},
                {"QuantityDisplay", QuantityDisplay},
                {"InventoryImpact", InventoryImpact},
                {"UnitCost", UnitCost},
                {"TotalCost", TotalCost},
                {"MovementDate", MovementDate},
                {"TimeAgo", TimeAgoDisplay},
                {"Reference", ReferenceDisplay},
                {"Description", Description},
                {"Transfer", IsTransfer ? new
                {
                    IsTransfer = true,
                    Direction = TransferDirectionDisplay,
                    FromSite = FromSite?.SiteName,
                    ToSite = ToSite?.SiteName
                } : null},
                {"CreatedBy", CreatedByUser?.FullName}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the stock movement</returns>
        public override string ToString() => DisplayName;
    }
}
