using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a book requisition in the KMSI Course Management System
    /// Manages book ordering requests from sites to head office or suppliers
    /// </summary>
    [Table("BookRequisitions")]
    public class BookRequisition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequisitionId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site is required")]
        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Requisition number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Requisition number must be between 5 and 20 characters")]
        [Display(Name = "Requisition Number")]
        [RegularExpression("^[A-Z0-9-]+$", ErrorMessage = "Requisition number must contain only uppercase letters, numbers, and hyphens")]
        public string RequisitionNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Request date is required")]
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Required date is required")]
        [Display(Name = "Required Date")]
        [DataType(DataType.Date)]
        public DateTime RequiredDate { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Fulfilled, Cancelled

        [Range(0, 999, ErrorMessage = "Total items must be between 0 and 999")]
        [Display(Name = "Total Items")]
        public int TotalItems { get; set; } = 0;

        [Range(0, 99999, ErrorMessage = "Total quantity must be between 0 and 99999")]
        [Display(Name = "Total Quantity")]
        public int TotalQuantity { get; set; } = 0;

        [Display(Name = "Approved By")]
        [ForeignKey("ApprovedByUser")]
        public int? ApprovedBy { get; set; }

        [Display(Name = "Approved Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "Fulfilled Date")]
        [DataType(DataType.DateTime)]
        public DateTime? FulfilledDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

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
        /// Company that owns this requisition
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site requesting the books
        /// </summary>
        [Required]
        [Display(Name = "Site")]
        public virtual Site Site { get; set; } = null!;

        /// <summary>
        /// User who approved this requisition
        /// </summary>
        [Display(Name = "Approved By")]
        public virtual User? ApprovedByUser { get; set; }

        /// <summary>
        /// User who created this requisition
        /// </summary>
        [Display(Name = "Created By")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this requisition
        /// </summary>
        [Display(Name = "Updated By")]
        public virtual User? UpdatedByUser { get; set; }

        /// <summary>
        /// Book requisition details (individual book items)
        /// </summary>
        [Display(Name = "Requisition Details")]
        public virtual ICollection<BookRequisitionDetail> BookRequisitionDetails { get; set; } = new List<BookRequisitionDetail>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{RequisitionNumber} - {Site?.SiteName}";

        /// <summary>
        /// Short display name for compact views
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplayName => $"{RequisitionNumber} ({RequestDate:dd/MM/yyyy})";

        /// <summary>
        /// Status display with icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Display")]
        public string StatusDisplay => Status switch
        {
            "Pending" => "⏳ Pending",
            "Approved" => "✅ Approved",
            "Rejected" => "❌ Rejected",
            "Fulfilled" => "📦 Fulfilled",
            "Cancelled" => "🚫 Cancelled",
            _ => Status
        };

        /// <summary>
        /// Priority based on required date and urgency
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority")]
        public string Priority
        {
            get
            {
                if (Status == "Cancelled" || Status == "Rejected") return "None";

                var daysUntilRequired = (RequiredDate.Date - DateTime.Today).Days;
                return daysUntilRequired switch
                {
                    < 0 => "Overdue",
                    <= 3 => "Urgent",
                    <= 7 => "High",
                    <= 14 => "Medium",
                    _ => "Normal"
                };
            }
        }

        /// <summary>
        /// Priority display with colors/icons
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority Display")]
        public string PriorityDisplay => Priority switch
        {
            "Overdue" => "🔴 Overdue",
            "Urgent" => "🟠 Urgent",
            "High" => "🟡 High",
            "Medium" => "🔵 Medium",
            "Normal" => "🟢 Normal",
            "None" => "⚫ None",
            _ => Priority
        };

        /// <summary>
        /// Days until required date
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Required")]
        public int DaysUntilRequired => (RequiredDate.Date - DateTime.Today).Days;

        /// <summary>
        /// Days since created
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Created")]
        public int DaysSinceCreated => (DateTime.Today - CreatedDate.Date).Days;

        /// <summary>
        /// Processing time in days (for completed requisitions)
        /// </summary>
        [NotMapped]
        [Display(Name = "Processing Time (Days)")]
        public int? ProcessingTimeDays
        {
            get
            {
                if (Status == "Fulfilled" && FulfilledDate.HasValue)
                    return (FulfilledDate.Value.Date - CreatedDate.Date).Days;
                if (Status == "Approved" && ApprovedDate.HasValue)
                    return (ApprovedDate.Value.Date - CreatedDate.Date).Days;
                return null;
            }
        }

        /// <summary>
        /// Approval time in days
        /// </summary>
        [NotMapped]
        [Display(Name = "Approval Time (Days)")]
        public int? ApprovalTimeDays => ApprovedDate.HasValue ?
            (ApprovedDate.Value.Date - CreatedDate.Date).Days : null;

        /// <summary>
        /// Indicates if requisition is active (not cancelled, rejected, or fulfilled)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Active")]
        public bool IsActive => Status != "Cancelled" && Status != "Rejected" && Status != "Fulfilled";

        /// <summary>
        /// Indicates if requisition is pending approval
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Pending")]
        public bool IsPending => Status == "Pending";

        /// <summary>
        /// Indicates if requisition is approved
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Approved")]
        public bool IsApproved => Status == "Approved";

        /// <summary>
        /// Indicates if requisition is rejected
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Rejected")]
        public bool IsRejected => Status == "Rejected";

        /// <summary>
        /// Indicates if requisition is fulfilled
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Fulfilled")]
        public bool IsFulfilled => Status == "Fulfilled";

        /// <summary>
        /// Indicates if requisition is cancelled
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Cancelled")]
        public bool IsCancelled => Status == "Cancelled";

        /// <summary>
        /// Indicates if requisition is overdue
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Overdue")]
        public bool IsOverdue => RequiredDate.Date < DateTime.Today && IsActive;

        /// <summary>
        /// Current step in the workflow
        /// </summary>
        [NotMapped]
        [Display(Name = "Current Step")]
        public string CurrentStep => Status switch
        {
            "Pending" => "Awaiting Approval",
            "Approved" => "Awaiting Fulfillment",
            "Rejected" => "Rejected - No Action Required",
            "Fulfilled" => "Completed",
            "Cancelled" => "Cancelled - No Action Required",
            _ => "Unknown"
        };

        /// <summary>
        /// Requisition type based on quantity and urgency
        /// </summary>
        [NotMapped]
        [Display(Name = "Requisition Type")]
        public string RequisitionType
        {
            get
            {
                if (TotalQuantity <= 10) return "Small Order";
                if (TotalQuantity <= 50) return "Regular Order";
                if (TotalQuantity <= 200) return "Large Order";
                return "Bulk Order";
            }
        }

        /// <summary>
        /// Completion percentage based on fulfilled quantity vs requested
        /// </summary>
        [NotMapped]
        [Display(Name = "Completion Percentage")]
        public decimal CompletionPercentage
        {
            get
            {
                if (TotalQuantity == 0) return 0;
                var fulfilledQuantity = BookRequisitionDetails?.Sum(rd => rd.FulfilledQuantity) ?? 0;
                return Math.Round((decimal)fulfilledQuantity / TotalQuantity * 100, 2);
            }
        }

        /// <summary>
        /// Total estimated cost
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Estimated Cost")]
        public decimal TotalEstimatedCost => BookRequisitionDetails?.Sum(rd => rd.TotalCost) ?? 0;

        /// <summary>
        /// Number of different books requested
        /// </summary>
        [NotMapped]
        [Display(Name = "Book Types Count")]
        public int BookTypesCount => BookRequisitionDetails?.Count ?? 0;

        /// <summary>
        /// Average quantity per book type
        /// </summary>
        [NotMapped]
        [Display(Name = "Average Quantity Per Book")]
        public decimal AverageQuantityPerBook => BookTypesCount > 0 ?
            Math.Round((decimal)TotalQuantity / BookTypesCount, 2) : 0;

        // Static Constants for Status
        public static class RequisitionStatus
        {
            public const string Pending = "Pending";
            public const string Approved = "Approved";
            public const string Rejected = "Rejected";
            public const string Fulfilled = "Fulfilled";
            public const string Cancelled = "Cancelled";
        }

        // Business Logic Methods

        /// <summary>
        /// Validate book requisition business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateRequisitionRules()
        {
            var errors = new List<string>();

            // Date validations
            if (RequestDate.Date > DateTime.Today)
            {
                errors.Add("Request date cannot be in the future");
            }

            if (RequiredDate.Date <= RequestDate.Date)
            {
                errors.Add("Required date must be after request date");
            }

            if (RequiredDate.Date < DateTime.Today && Status == RequisitionStatus.Pending)
            {
                errors.Add("Cannot create pending requisitions with past required dates");
            }

            // Status validations
            var validStatuses = new[] {
                RequisitionStatus.Pending, RequisitionStatus.Approved,
                RequisitionStatus.Rejected, RequisitionStatus.Fulfilled, RequisitionStatus.Cancelled
            };
            if (!validStatuses.Contains(Status))
            {
                errors.Add($"Invalid status. Must be one of: {string.Join(", ", validStatuses)}");
            }

            // Approval validations
            if (Status == RequisitionStatus.Approved && !ApprovedBy.HasValue)
            {
                errors.Add("Approved requisitions must have an approver");
            }

            if (ApprovedDate.HasValue && ApprovedDate.Value.Date > DateTime.Today)
            {
                errors.Add("Approval date cannot be in the future");
            }

            // Fulfillment validations
            if (Status == RequisitionStatus.Fulfilled && !FulfilledDate.HasValue)
            {
                errors.Add("Fulfilled requisitions must have a fulfillment date");
            }

            if (FulfilledDate.HasValue && FulfilledDate.Value.Date > DateTime.Today)
            {
                errors.Add("Fulfillment date cannot be in the future");
            }

            // Quantity validations
            if (TotalItems < 0)
            {
                errors.Add("Total items cannot be negative");
            }

            if (TotalQuantity < 0)
            {
                errors.Add("Total quantity cannot be negative");
            }

            // Business rule: Total items should match detail count
            if (BookRequisitionDetails != null && BookRequisitionDetails.Count != TotalItems)
            {
                errors.Add("Total items should match the number of detail records");
            }

            // Business rule: Total quantity should match sum of detail quantities
            if (BookRequisitionDetails != null)
            {
                var detailQuantitySum = BookRequisitionDetails.Sum(rd => rd.RequestedQuantity);
                if (detailQuantitySum != TotalQuantity)
                {
                    errors.Add("Total quantity should match the sum of detail quantities");
                }
            }

            return errors;
        }

        /// <summary>
        /// Generate unique requisition number
        /// </summary>
        /// <param name="companyCode">Company code</param>
        /// <param name="siteCode">Site code</param>
        /// <param name="requestDate">Request date</param>
        /// <param name="existingNumbers">Existing requisition numbers</param>
        /// <returns>Unique requisition number</returns>
        public static string GenerateRequisitionNumber(string companyCode, string siteCode,
            DateTime requestDate, IEnumerable<string> existingNumbers)
        {
            var dateCode = requestDate.ToString("yyyyMM");
            var prefix = $"REQ-{companyCode}-{siteCode}-{dateCode}-";

            var existingSequences = existingNumbers
                .Where(n => n.StartsWith(prefix))
                .Select(n =>
                {
                    var parts = n.Split('-');
                    return parts.Length == 5 && int.TryParse(parts[4], out int num) ? num : 0;
                })
                .DefaultIfEmpty(0);

            var nextNumber = existingSequences.Max() + 1;
            return $"{prefix}{nextNumber:000}";
        }

        /// <summary>
        /// Update requisition status with validation
        /// </summary>
        /// <param name="newStatus">New status</param>
        /// <param name="userId">User making the change</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>True if status was updated successfully</returns>
        public bool UpdateStatus(string newStatus, int? userId = null, string? notes = null)
        {
            var validTransitions = GetValidStatusTransitions();
            if (!validTransitions.Contains(newStatus))
                return false;

            Status = newStatus;
            UpdatedBy = userId;
            UpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(notes))
                Notes = notes;

            // Set specific date fields based on status
            switch (newStatus)
            {
                case RequisitionStatus.Approved:
                    ApprovedBy = userId;
                    ApprovedDate = DateTime.Now;
                    break;
                case RequisitionStatus.Fulfilled:
                    FulfilledDate = DateTime.Now;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Get valid status transitions based on current status
        /// </summary>
        /// <returns>List of valid next statuses</returns>
        public List<string> GetValidStatusTransitions()
        {
            return Status switch
            {
                RequisitionStatus.Pending => new List<string> {
                    RequisitionStatus.Approved, RequisitionStatus.Rejected, RequisitionStatus.Cancelled
                },
                RequisitionStatus.Approved => new List<string> {
                    RequisitionStatus.Fulfilled, RequisitionStatus.Cancelled
                },
                RequisitionStatus.Rejected => new List<string> {
                    RequisitionStatus.Pending // Can be resubmitted
                },
                RequisitionStatus.Fulfilled => new List<string>(), // Terminal status
                RequisitionStatus.Cancelled => new List<string> {
                    RequisitionStatus.Pending // Can be reactivated
                },
                _ => new List<string>()
            };
        }

        /// <summary>
        /// Approve requisition
        /// </summary>
        /// <param name="approverId">User ID of approver</param>
        /// <param name="approvalNotes">Optional approval notes</param>
        /// <returns>True if approved successfully</returns>
        public bool Approve(int approverId, string? approvalNotes = null)
        {
            if (Status != RequisitionStatus.Pending)
                return false;

            Status = RequisitionStatus.Approved;
            ApprovedBy = approverId;
            ApprovedDate = DateTime.Now;
            UpdatedBy = approverId;
            UpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(approvalNotes))
                Notes = approvalNotes;

            return true;
        }

        /// <summary>
        /// Reject requisition
        /// </summary>
        /// <param name="rejectorId">User ID of person rejecting</param>
        /// <param name="rejectionReason">Reason for rejection</param>
        /// <returns>True if rejected successfully</returns>
        public bool Reject(int rejectorId, string rejectionReason)
        {
            if (Status != RequisitionStatus.Pending)
                return false;

            Status = RequisitionStatus.Rejected;
            UpdatedBy = rejectorId;
            UpdatedDate = DateTime.Now;
            Notes = $"Rejected: {rejectionReason}";

            return true;
        }

        /// <summary>
        /// Mark requisition as fulfilled
        /// </summary>
        /// <param name="fulfillerId">User ID of person fulfilling</param>
        /// <param name="fulfillmentNotes">Optional fulfillment notes</param>
        /// <returns>True if marked as fulfilled successfully</returns>
        public bool MarkAsFulfilled(int fulfillerId, string? fulfillmentNotes = null)
        {
            if (Status != RequisitionStatus.Approved)
                return false;

            Status = RequisitionStatus.Fulfilled;
            FulfilledDate = DateTime.Now;
            UpdatedBy = fulfillerId;
            UpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(fulfillmentNotes))
                Notes = fulfillmentNotes;

            return true;
        }

        /// <summary>
        /// Cancel requisition
        /// </summary>
        /// <param name="cancellerId">User ID of person cancelling</param>
        /// <param name="cancellationReason">Reason for cancellation</param>
        /// <returns>True if cancelled successfully</returns>
        public bool Cancel(int cancellerId, string cancellationReason)
        {
            if (Status == RequisitionStatus.Fulfilled)
                return false;

            Status = RequisitionStatus.Cancelled;
            UpdatedBy = cancellerId;
            UpdatedDate = DateTime.Now;
            Notes = $"Cancelled: {cancellationReason}";

            return true;
        }

        /// <summary>
        /// Recalculate totals based on detail records
        /// </summary>
        public void RecalculateTotals()
        {
            if (BookRequisitionDetails != null)
            {
                TotalItems = BookRequisitionDetails.Count;
                TotalQuantity = BookRequisitionDetails.Sum(rd => rd.RequestedQuantity);
            }
            else
            {
                TotalItems = 0;
                TotalQuantity = 0;
            }
        }

        /// <summary>
        /// Get requisition summary for reporting
        /// </summary>
        /// <returns>Dictionary with requisition summary</returns>
        public Dictionary<string, object> GetRequisitionSummary()
        {
            return new Dictionary<string, object>
            {
                {"RequisitionId", RequisitionId},
                {"RequisitionNumber", RequisitionNumber},
                {"SiteName", Site?.SiteName},
                {"RequestDate", RequestDate},
                {"RequiredDate", RequiredDate},
                {"Status", Status},
                {"Priority", Priority},
                {"TotalItems", TotalItems},
                {"TotalQuantity", TotalQuantity},
                {"TotalEstimatedCost", TotalEstimatedCost},
                {"CompletionPercentage", CompletionPercentage},
                {"DaysUntilRequired", DaysUntilRequired},
                {"DaysSinceCreated", DaysSinceCreated},
                {"ProcessingTimeDays", ProcessingTimeDays},
                {"ApprovalTimeDays", ApprovalTimeDays},
                {"RequisitionType", RequisitionType},
                {"IsOverdue", IsOverdue},
                {"CurrentStep", CurrentStep},
                {"CreatedByName", CreatedByUser?.FullName},
                {"ApprovedByName", ApprovedByUser?.FullName},
                {"ApprovedDate", ApprovedDate},
                {"FulfilledDate", FulfilledDate}
            };
        }

        /// <summary>
        /// Create a new book requisition
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="requisitionNumber">Requisition number</param>
        /// <param name="requestDate">Request date</param>
        /// <param name="requiredDate">Required date</param>
        /// <param name="notes">Optional notes</param>
        /// <param name="createdBy">Creator user ID</param>
        /// <returns>New book requisition instance</returns>
        public static BookRequisition CreateRequisition(int companyId, int siteId, string requisitionNumber,
            DateTime requestDate, DateTime requiredDate, string? notes = null, int? createdBy = null)
        {
            return new BookRequisition
            {
                CompanyId = companyId,
                SiteId = siteId,
                RequisitionNumber = requisitionNumber,
                RequestDate = requestDate,
                RequiredDate = requiredDate,
                Status = RequisitionStatus.Pending,
                Notes = notes,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get requisitions by status
        /// </summary>
        /// <param name="requisitions">Collection of requisitions</param>
        /// <param name="status">Status to filter by</param>
        /// <returns>Filtered requisitions</returns>
        public static IEnumerable<BookRequisition> GetByStatus(IEnumerable<BookRequisition> requisitions, string status)
        {
            return requisitions.Where(r => r.Status == status);
        }

        /// <summary>
        /// Get overdue requisitions
        /// </summary>
        /// <param name="requisitions">Collection of requisitions</param>
        /// <returns>Overdue requisitions</returns>
        public static IEnumerable<BookRequisition> GetOverdueRequisitions(IEnumerable<BookRequisition> requisitions)
        {
            return requisitions.Where(r => r.IsOverdue);
        }

        /// <summary>
        /// Calculate requisition statistics
        /// </summary>
        /// <param name="requisitions">Collection of requisitions</param>
        /// <returns>Dictionary with statistics</returns>
        public static Dictionary<string, object> CalculateStatistics(IEnumerable<BookRequisition> requisitions)
        {
            var reqList = requisitions.ToList();
            var totalRequisitions = reqList.Count;

            if (totalRequisitions == 0)
            {
                return new Dictionary<string, object>
                {
                    {"TotalRequisitions", 0},
                    {"PendingRequisitions", 0},
                    {"ApprovedRequisitions", 0},
                    {"FulfilledRequisitions", 0},
                    {"AverageProcessingTime", 0}
                };
            }

            var pendingCount = reqList.Count(r => r.Status == RequisitionStatus.Pending);
            var approvedCount = reqList.Count(r => r.Status == RequisitionStatus.Approved);
            var rejectedCount = reqList.Count(r => r.Status == RequisitionStatus.Rejected);
            var fulfilledCount = reqList.Count(r => r.Status == RequisitionStatus.Fulfilled);
            var cancelledCount = reqList.Count(r => r.Status == RequisitionStatus.Cancelled);
            var overdueCount = reqList.Count(r => r.IsOverdue);

            var completedReqs = reqList.Where(r => r.ProcessingTimeDays.HasValue).ToList();
            var avgProcessingTime = completedReqs.Any() ?
                completedReqs.Average(r => r.ProcessingTimeDays!.Value) : 0;

            var totalQuantity = reqList.Sum(r => r.TotalQuantity);
            var totalEstimatedCost = reqList.Sum(r => r.TotalEstimatedCost);

            return new Dictionary<string, object>
            {
                {"TotalRequisitions", totalRequisitions},
                {"PendingRequisitions", pendingCount},
                {"ApprovedRequisitions", approvedCount},
                {"RejectedRequisitions", rejectedCount},
                {"FulfilledRequisitions", fulfilledCount},
                {"CancelledRequisitions", cancelledCount},
                {"OverdueRequisitions", overdueCount},
                {"ApprovalRate", Math.Round((decimal)approvedCount / totalRequisitions * 100, 2)},
                {"FulfillmentRate", Math.Round((decimal)fulfilledCount / totalRequisitions * 100, 2)},
                {"AverageProcessingTime", Math.Round(avgProcessingTime, 2)},
                {"TotalQuantityRequested", totalQuantity},
                {"TotalEstimatedValue", totalEstimatedCost},
                {"AverageQuantityPerRequisition", Math.Round((decimal)totalQuantity / totalRequisitions, 2)},
                {"AverageValuePerRequisition", Math.Round(totalEstimatedCost / totalRequisitions, 2)}
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the book requisition</returns>
        public override string ToString() => DisplayName;
    }
}
