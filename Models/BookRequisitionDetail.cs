using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents individual book items within a book requisition
    /// Contains details about requested, approved, and fulfilled quantities for specific books
    /// </summary>
    [Table("BookRequisitionDetails")]
    public class BookRequisitionDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequisitionDetailId { get; set; }

        [Required(ErrorMessage = "Requisition is required")]
        [Display(Name = "Requisition")]
        [ForeignKey("BookRequisition")]
        public int RequisitionId { get; set; }

        [Required(ErrorMessage = "Book is required")]
        [Display(Name = "Book")]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Requested quantity is required")]
        [Range(1, 10000, ErrorMessage = "Requested quantity must be between 1 and 10,000")]
        [Display(Name = "Requested Quantity")]
        public int RequestedQuantity { get; set; }

        [Range(0, 10000, ErrorMessage = "Approved quantity must be between 0 and 10,000")]
        [Display(Name = "Approved Quantity")]
        public int ApprovedQuantity { get; set; } = 0;

        [Range(0, 10000, ErrorMessage = "Fulfilled quantity must be between 0 and 10,000")]
        [Display(Name = "Fulfilled Quantity")]
        public int FulfilledQuantity { get; set; } = 0;

        [Range(0, 9999999.99, ErrorMessage = "Unit cost must be between 0 and 9,999,999.99")]
        [Display(Name = "Unit Cost")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal? UnitCost { get; set; }

        [StringLength(255, ErrorMessage = "Notes cannot exceed 255 characters")]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        // Navigation Properties

        /// <summary>
        /// The parent book requisition
        /// </summary>
        [Required]
        [Display(Name = "Book Requisition")]
        public virtual BookRequisition BookRequisition { get; set; } = null!;

        /// <summary>
        /// The book being requested
        /// </summary>
        [Required]
        [Display(Name = "Book")]
        public virtual Book Book { get; set; } = null!;

        // Computed Properties (Not Mapped - calculated properties)

        /// <summary>
        /// Total cost based on fulfilled quantity and unit cost (computed column in database)
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Cost")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalCost => (FulfilledQuantity * (UnitCost ?? 0));

        /// <summary>
        /// Remaining quantity to be approved (Requested - Approved)
        /// </summary>
        [NotMapped]
        [Display(Name = "Remaining to Approve")]
        public int RemainingToApprove => RequestedQuantity - ApprovedQuantity;

        /// <summary>
        /// Remaining quantity to be fulfilled (Approved - Fulfilled)
        /// </summary>
        [NotMapped]
        [Display(Name = "Remaining to Fulfill")]
        public int RemainingToFulfill => ApprovedQuantity - FulfilledQuantity;

        /// <summary>
        /// Approval percentage (Approved / Requested * 100)
        /// </summary>
        [NotMapped]
        [Display(Name = "Approval Percentage")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal ApprovalPercentage => RequestedQuantity > 0 ? (decimal)ApprovedQuantity / RequestedQuantity : 0;

        /// <summary>
        /// Fulfillment percentage (Fulfilled / Approved * 100)
        /// </summary>
        [NotMapped]
        [Display(Name = "Fulfillment Percentage")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal FulfillmentPercentage => ApprovedQuantity > 0 ? (decimal)FulfilledQuantity / ApprovedQuantity : 0;

        /// <summary>
        /// Overall completion percentage (Fulfilled / Requested * 100)
        /// </summary>
        [NotMapped]
        [Display(Name = "Completion Percentage")]
        [DisplayFormat(DataFormatString = "{0:P0}")]
        public decimal CompletionPercentage => RequestedQuantity > 0 ? (decimal)FulfilledQuantity / RequestedQuantity : 0;

        /// <summary>
        /// Status based on quantities
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string Status
        {
            get
            {
                if (ApprovedQuantity == 0) return "Pending Approval";
                if (FulfilledQuantity == 0) return "Approved - Pending Fulfillment";
                if (FulfilledQuantity < ApprovedQuantity) return "Partially Fulfilled";
                if (FulfilledQuantity == ApprovedQuantity) return "Fulfilled";
                return "Over Fulfilled"; // This shouldn't normally happen
            }
        }

        /// <summary>
        /// Display name for dropdowns and lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => Book?.BookTitle ?? $"Book ID: {BookId}";

        /// <summary>
        /// Short description for quick identification
        /// </summary>
        [NotMapped]
        [Display(Name = "Description")]
        public string Description => $"{DisplayName} - Requested: {RequestedQuantity}, Approved: {ApprovedQuantity}, Fulfilled: {FulfilledQuantity}";

        // Business Logic Methods

        /// <summary>
        /// Validate business rules for book requisition detail
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateBusinessRules()
        {
            var errors = new List<string>();

            // Approved quantity cannot exceed requested quantity
            if (ApprovedQuantity > RequestedQuantity)
            {
                errors.Add($"Approved quantity ({ApprovedQuantity}) cannot exceed requested quantity ({RequestedQuantity})");
            }

            // Fulfilled quantity cannot exceed approved quantity
            if (FulfilledQuantity > ApprovedQuantity)
            {
                errors.Add($"Fulfilled quantity ({FulfilledQuantity}) cannot exceed approved quantity ({ApprovedQuantity})");
            }

            // Unit cost should be specified if there are fulfilled items
            if (FulfilledQuantity > 0 && (!UnitCost.HasValue || UnitCost.Value <= 0))
            {
                errors.Add("Unit cost must be specified and greater than zero when items are fulfilled");
            }

            // Quantities should be positive
            if (RequestedQuantity <= 0)
            {
                errors.Add("Requested quantity must be greater than zero");
            }

            if (ApprovedQuantity < 0)
            {
                errors.Add("Approved quantity cannot be negative");
            }

            if (FulfilledQuantity < 0)
            {
                errors.Add("Fulfilled quantity cannot be negative");
            }

            return errors;
        }

        /// <summary>
        /// Check if this detail can be approved
        /// </summary>
        /// <returns>True if can be approved</returns>
        public bool CanBeApproved()
        {
            return ApprovedQuantity == 0 && RequestedQuantity > 0;
        }

        /// <summary>
        /// Check if this detail can be fulfilled
        /// </summary>
        /// <returns>True if can be fulfilled</returns>
        public bool CanBeFulfilled()
        {
            return ApprovedQuantity > FulfilledQuantity;
        }

        /// <summary>
        /// Check if this detail is fully completed
        /// </summary>
        /// <returns>True if fully completed</returns>
        public bool IsCompleted()
        {
            return FulfilledQuantity > 0 && FulfilledQuantity == ApprovedQuantity;
        }

        /// <summary>
        /// Update approval quantity with validation
        /// </summary>
        /// <param name="quantity">Quantity to approve</param>
        /// <returns>True if successful</returns>
        public bool UpdateApprovedQuantity(int quantity)
        {
            if (quantity < 0 || quantity > RequestedQuantity)
            {
                return false;
            }

            ApprovedQuantity = quantity;

            // If approved quantity is reduced, fulfilled quantity should not exceed it
            if (FulfilledQuantity > ApprovedQuantity)
            {
                FulfilledQuantity = ApprovedQuantity;
            }

            return true;
        }

        /// <summary>
        /// Update fulfilled quantity with validation
        /// </summary>
        /// <param name="quantity">Quantity to fulfill</param>
        /// <param name="unitCost">Unit cost per item</param>
        /// <returns>True if successful</returns>
        public bool UpdateFulfilledQuantity(int quantity, decimal? unitCost = null)
        {
            if (quantity < 0 || quantity > ApprovedQuantity)
            {
                return false;
            }

            FulfilledQuantity = quantity;

            if (unitCost.HasValue)
            {
                UnitCost = unitCost;
            }

            return true;
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the book requisition detail</returns>
        public override string ToString() => Description;
    }
}
