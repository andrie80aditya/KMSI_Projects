using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents book pricing information for multi-site pricing strategy
    /// Allows different sites to have different prices for the same book
    /// Supports time-based pricing with effective and expiry dates
    /// </summary>
    [Table("BookPrices")]
    public class BookPrice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookPriceId { get; set; }

        [Required(ErrorMessage = "Book is required")]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999999.99, ErrorMessage = "Price must be between 0.01 and 999,999,999.99")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be exactly 3 characters")]
        [Display(Name = "Currency")]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency code must be 3 uppercase letters (e.g., IDR, USD)")]
        public string Currency { get; set; } = "IDR";

        [Required(ErrorMessage = "Effective date is required")]
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;

        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        [ForeignKey("CreatedByUser")]
        public int? CreatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// The book this price applies to
        /// </summary>
        [Required]
        [Display(Name = "Book")]
        public virtual Book Book { get; set; } = null!;

        /// <summary>
        /// The site where this price is applicable
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// User who created this price record
        /// </summary>
        [Display(Name = "Created By User")]
        public virtual User? CreatedByUser { get; set; }

        // Computed Properties (Not Mapped - calculated properties)

        /// <summary>
        /// Check if this price is currently valid based on dates
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Current")]
        public bool IsCurrent
        {
            get
            {
                var now = DateTime.Today;
                return IsActive &&
                       EffectiveDate <= now &&
                       (!ExpiryDate.HasValue || ExpiryDate.Value >= now);
            }
        }

        /// <summary>
        /// Check if this price is expired
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Expired")]
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Today;

        /// <summary>
        /// Check if this price is future (not yet effective)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Future")]
        public bool IsFuture => EffectiveDate > DateTime.Today;

        /// <summary>
        /// Days until effective (negative if already effective)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Effective")]
        public int DaysUntilEffective => (EffectiveDate - DateTime.Today).Days;

        /// <summary>
        /// Days until expiry (null if no expiry date)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Expiry")]
        public int? DaysUntilExpiry => ExpiryDate.HasValue ? (ExpiryDate.Value - DateTime.Today).Days : null;

        /// <summary>
        /// Price validity period in days
        /// </summary>
        [NotMapped]
        [Display(Name = "Validity Period (Days)")]
        public int? ValidityPeriod => ExpiryDate.HasValue ? (ExpiryDate.Value - EffectiveDate).Days + 1 : null;

        /// <summary>
        /// Status based on dates and active flag
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                if (!IsActive) return "Inactive";
                if (IsFuture) return "Future";
                if (IsExpired) return "Expired";
                if (IsCurrent) return "Current";
                return "Unknown";
            }
        }

        /// <summary>
        /// Formatted price with currency
        /// </summary>
        [NotMapped]
        [Display(Name = "Formatted Price")]
        public string FormattedPrice => $"{Price:N2} {Currency}";

        /// <summary>
        /// Display name for dropdowns and lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{Book?.BookTitle ?? "Unknown Book"} - {Site?.SiteName ?? "Unknown Site"} - {FormattedPrice}";

        /// <summary>
        /// Short description for quick identification
        /// </summary>
        [NotMapped]
        [Display(Name = "Description")]
        public string Description => $"{DisplayName} ({Status})";

        /// <summary>
        /// Validity period description
        /// </summary>
        [NotMapped]
        [Display(Name = "Validity Description")]
        public string ValidityDescription
        {
            get
            {
                if (ExpiryDate.HasValue)
                {
                    return $"{EffectiveDate:MMM dd, yyyy} - {ExpiryDate.Value:MMM dd, yyyy}";
                }
                return $"From {EffectiveDate:MMM dd, yyyy} (No expiry)";
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Validate business rules for book price
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateBusinessRules()
        {
            var errors = new List<string>();

            // Price must be positive
            if (Price <= 0)
            {
                errors.Add("Price must be greater than zero");
            }

            // Effective date validation
            if (EffectiveDate == default(DateTime))
            {
                errors.Add("Effective date is required");
            }

            // Expiry date must be after effective date
            if (ExpiryDate.HasValue && ExpiryDate.Value <= EffectiveDate)
            {
                errors.Add("Expiry date must be after effective date");
            }

            // Currency code validation
            if (string.IsNullOrWhiteSpace(Currency) || Currency.Length != 3)
            {
                errors.Add("Currency code must be exactly 3 characters");
            }

            // Check for reasonable price range
            if (Price > 10000000) // 10 million
            {
                errors.Add("Price seems unusually high. Please verify.");
            }

            return errors;
        }

        /// <summary>
        /// Check if this price conflicts with another price for the same book and site
        /// </summary>
        /// <param name="otherPrice">Other price to compare with</param>
        /// <returns>True if there's a conflict</returns>
        public bool ConflictsWith(BookPrice otherPrice)
        {
            if (otherPrice == null ||
                otherPrice.BookPriceId == BookPriceId ||
                otherPrice.BookId != BookId ||
                otherPrice.SiteId != SiteId)
            {
                return false;
            }

            // Check if both prices are active and have overlapping date ranges
            if (!IsActive || !otherPrice.IsActive)
            {
                return false;
            }

            var thisStart = EffectiveDate;
            var thisEnd = ExpiryDate ?? DateTime.MaxValue;
            var otherStart = otherPrice.EffectiveDate;
            var otherEnd = otherPrice.ExpiryDate ?? DateTime.MaxValue;

            // Check for date range overlap
            return thisStart <= otherEnd && otherStart <= thisEnd;
        }

        /// <summary>
        /// Check if this price is valid for a specific date
        /// </summary>
        /// <param name="date">Date to check</param>
        /// <returns>True if valid on the specified date</returns>
        public bool IsValidOn(DateTime date)
        {
            return IsActive &&
                   EffectiveDate <= date &&
                   (!ExpiryDate.HasValue || ExpiryDate.Value >= date);
        }

        /// <summary>
        /// Get the price with a discount applied
        /// </summary>
        /// <param name="discountPercentage">Discount percentage (0-100)</param>
        /// <returns>Discounted price</returns>
        public decimal GetDiscountedPrice(decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
            {
                throw new ArgumentException("Discount percentage must be between 0 and 100");
            }

            return Price * (1 - discountPercentage / 100);
        }

        /// <summary>
        /// Extend the validity period by adding days to expiry date
        /// </summary>
        /// <param name="days">Number of days to add</param>
        public void ExtendValidityBy(int days)
        {
            if (days <= 0)
            {
                throw new ArgumentException("Days must be positive");
            }

            if (ExpiryDate.HasValue)
            {
                ExpiryDate = ExpiryDate.Value.AddDays(days);
            }
            else
            {
                // If no expiry date, set it to effective date + extension days
                ExpiryDate = EffectiveDate.AddDays(days);
            }
        }

        /// <summary>
        /// Create a new price based on this one with updated values
        /// </summary>
        /// <param name="newPrice">New price amount</param>
        /// <param name="newEffectiveDate">New effective date</param>
        /// <param name="newExpiryDate">New expiry date (optional)</param>
        /// <returns>New BookPrice instance</returns>
        public BookPrice CreateNewVersion(decimal newPrice, DateTime newEffectiveDate, DateTime? newExpiryDate = null)
        {
            return new BookPrice
            {
                BookId = BookId,
                SiteId = SiteId,
                Price = newPrice,
                Currency = Currency,
                EffectiveDate = newEffectiveDate,
                ExpiryDate = newExpiryDate,
                IsActive = true,
                CreatedBy = CreatedBy
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the book price</returns>
        public override string ToString() => Description;
    }
}
