using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents audit trail records for tracking data changes across the system
    /// Captures who changed what, when, and from where for compliance and security
    /// </summary>
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuditLogId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Table name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Table name must be between 1 and 100 characters")]
        [Display(Name = "Table Name")]
        public string TableName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Record ID is required")]
        [Display(Name = "Record ID")]
        public int RecordId { get; set; }

        [Required(ErrorMessage = "Action is required")]
        [StringLength(20, ErrorMessage = "Action cannot exceed 20 characters")]
        [Display(Name = "Action")]
        public string Action { get; set; } = string.Empty; // Insert, Update, Delete

        [Display(Name = "Old Values")]
        [DataType(DataType.MultilineText)]
        public string? OldValues { get; set; }

        [Display(Name = "New Values")]
        [DataType(DataType.MultilineText)]
        public string? NewValues { get; set; }

        [StringLength(45, ErrorMessage = "IP Address cannot exceed 45 characters")]
        [Display(Name = "IP Address")]
        public string? IPAddress { get; set; }

        [StringLength(500, ErrorMessage = "User Agent cannot exceed 500 characters")]
        [Display(Name = "User Agent")]
        public string? UserAgent { get; set; }

        [Display(Name = "Action Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ActionDate { get; set; } = DateTime.Now;

        // Navigation Properties

        /// <summary>
        /// Company where the action occurred
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// User who performed the action
        /// </summary>
        [Required]
        [Display(Name = "User")]
        public virtual User User { get; set; } = null!;

        // Computed Properties (Not Mapped - calculated properties)

        /// <summary>
        /// Time elapsed since the action occurred
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Ago")]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - ActionDate;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes > 1 ? "s" : "")} ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 365)
                    return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) > 1 ? "s" : "")} ago";

                return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) > 1 ? "s" : "")} ago";
            }
        }

        /// <summary>
        /// Action type with friendly description
        /// </summary>
        [NotMapped]
        [Display(Name = "Action Description")]
        public string ActionDescription
        {
            get
            {
                return Action.ToUpper() switch
                {
                    "INSERT" => "Created",
                    "UPDATE" => "Modified",
                    "DELETE" => "Deleted",
                    _ => Action
                };
            }
        }

        /// <summary>
        /// Color code for UI display based on action type
        /// </summary>
        [NotMapped]
        [Display(Name = "Action Color")]
        public string ActionColor
        {
            get
            {
                return Action.ToUpper() switch
                {
                    "INSERT" => "success", // Green
                    "UPDATE" => "warning", // Orange/Yellow
                    "DELETE" => "danger",  // Red
                    _ => "secondary"       // Gray
                };
            }
        }

        /// <summary>
        /// Icon for UI display based on action type
        /// </summary>
        [NotMapped]
        [Display(Name = "Action Icon")]
        public string ActionIcon
        {
            get
            {
                return Action.ToUpper() switch
                {
                    "INSERT" => "fas fa-plus-circle",
                    "UPDATE" => "fas fa-edit",
                    "DELETE" => "fas fa-trash-alt",
                    _ => "fas fa-info-circle"
                };
            }
        }

        /// <summary>
        /// Display name for the audited entity
        /// </summary>
        [NotMapped]
        [Display(Name = "Entity")]
        public string EntityDisplay => $"{TableName} (ID: {RecordId})";

        /// <summary>
        /// Short description of the audit entry
        /// </summary>
        [NotMapped]
        [Display(Name = "Summary")]
        public string Summary => $"{User?.FullName ?? "Unknown User"} {ActionDescription.ToLower()} {TableName} record {RecordId}";

        /// <summary>
        /// Browser information extracted from User Agent
        /// </summary>
        [NotMapped]
        [Display(Name = "Browser")]
        public string Browser
        {
            get
            {
                if (string.IsNullOrEmpty(UserAgent)) return "Unknown";

                if (UserAgent.Contains("Chrome")) return "Chrome";
                if (UserAgent.Contains("Firefox")) return "Firefox";
                if (UserAgent.Contains("Safari") && !UserAgent.Contains("Chrome")) return "Safari";
                if (UserAgent.Contains("Edge")) return "Edge";
                if (UserAgent.Contains("Opera")) return "Opera";

                return "Other";
            }
        }

        /// <summary>
        /// Operating System extracted from User Agent
        /// </summary>
        [NotMapped]
        [Display(Name = "Operating System")]
        public string OperatingSystem
        {
            get
            {
                if (string.IsNullOrEmpty(UserAgent)) return "Unknown";

                if (UserAgent.Contains("Windows")) return "Windows";
                if (UserAgent.Contains("Mac OS")) return "macOS";
                if (UserAgent.Contains("Linux")) return "Linux";
                if (UserAgent.Contains("Android")) return "Android";
                if (UserAgent.Contains("iOS")) return "iOS";

                return "Other";
            }
        }

        /// <summary>
        /// Check if this is a recent action (within last 24 hours)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Recent")]
        public bool IsRecent => (DateTime.Now - ActionDate).TotalHours < 24;

        /// <summary>
        /// Check if this action has data changes
        /// </summary>
        [NotMapped]
        [Display(Name = "Has Changes")]
        public bool HasChanges => !string.IsNullOrEmpty(OldValues) || !string.IsNullOrEmpty(NewValues);

        /// <summary>
        /// Display name for dropdowns and lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{ActionDescription} {TableName} - {ActionDate:MMM dd, yyyy HH:mm}";

        // Business Logic Methods

        /// <summary>
        /// Parse old values from JSON string
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <returns>Deserialized old values or default</returns>
        public T? GetOldValues<T>() where T : class
        {
            if (string.IsNullOrEmpty(OldValues)) return null;

            try
            {
                return JsonSerializer.Deserialize<T>(OldValues);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Parse new values from JSON string
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <returns>Deserialized new values or default</returns>
        public T? GetNewValues<T>() where T : class
        {
            if (string.IsNullOrEmpty(NewValues)) return null;

            try
            {
                return JsonSerializer.Deserialize<T>(NewValues);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Get changes as a dictionary for display
        /// </summary>
        /// <returns>Dictionary of field changes</returns>
        public Dictionary<string, (object? OldValue, object? NewValue)> GetChanges()
        {
            var changes = new Dictionary<string, (object? OldValue, object? NewValue)>();

            try
            {
                var oldDict = string.IsNullOrEmpty(OldValues)
                    ? new Dictionary<string, object?>()
                    : JsonSerializer.Deserialize<Dictionary<string, object?>>(OldValues) ?? new Dictionary<string, object?>();

                var newDict = string.IsNullOrEmpty(NewValues)
                    ? new Dictionary<string, object?>()
                    : JsonSerializer.Deserialize<Dictionary<string, object?>>(NewValues) ?? new Dictionary<string, object?>();

                // Get all unique keys from both dictionaries
                var allKeys = oldDict.Keys.Union(newDict.Keys).ToList();

                foreach (var key in allKeys)
                {
                    var oldValue = oldDict.GetValueOrDefault(key);
                    var newValue = newDict.GetValueOrDefault(key);

                    // Only include if values are different
                    if (!Equals(oldValue, newValue))
                    {
                        changes[key] = (oldValue, newValue);
                    }
                }
            }
            catch (JsonException)
            {
                // If JSON parsing fails, return empty dictionary
            }

            return changes;
        }

        /// <summary>
        /// Get formatted change summary for display
        /// </summary>
        /// <returns>Human-readable change summary</returns>
        public string GetChangeSummary()
        {
            var changes = GetChanges();
            if (changes.Count == 0)
            {
                return Action.ToUpper() switch
                {
                    "INSERT" => "Record created",
                    "DELETE" => "Record deleted",
                    _ => "No changes detected"
                };
            }

            var summaries = changes.Select(kvp =>
                $"{kvp.Key}: '{kvp.Value.OldValue ?? "null"}' → '{kvp.Value.NewValue ?? "null"}'");

            return string.Join("; ", summaries);
        }

        /// <summary>
        /// Validate business rules for audit log
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateBusinessRules()
        {
            var errors = new List<string>();

            // Valid action types
            var validActions = new[] { "Insert", "Update", "Delete" };
            if (!validActions.Contains(Action, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Action must be one of: {string.Join(", ", validActions)}");
            }

            // Table name should be a valid identifier
            if (!string.IsNullOrEmpty(TableName) && !System.Text.RegularExpressions.Regex.IsMatch(TableName, @"^[A-Za-z][A-Za-z0-9_]*$"))
            {
                errors.Add("Table name must be a valid database table identifier");
            }

            // Record ID should be positive
            if (RecordId <= 0)
            {
                errors.Add("Record ID must be a positive integer");
            }

            // IP Address format validation (basic)
            if (!string.IsNullOrEmpty(IPAddress) &&
                !System.Net.IPAddress.TryParse(IPAddress, out _))
            {
                errors.Add("IP Address format is invalid");
            }

            // JSON validation for old/new values
            if (!string.IsNullOrEmpty(OldValues) && !IsValidJson(OldValues))
            {
                errors.Add("Old values must be valid JSON");
            }

            if (!string.IsNullOrEmpty(NewValues) && !IsValidJson(NewValues))
            {
                errors.Add("New values must be valid JSON");
            }

            // Action date should not be in the future
            if (ActionDate > DateTime.Now.AddMinutes(5)) // Allow 5 minutes tolerance for server time differences
            {
                errors.Add("Action date cannot be in the future");
            }

            return errors;
        }

        /// <summary>
        /// Check if string is valid JSON
        /// </summary>
        /// <param name="jsonString">String to validate</param>
        /// <returns>True if valid JSON</returns>
        private static bool IsValidJson(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        /// <summary>
        /// Create audit log entry for entity creation
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <param name="newValues">New values as object</param>
        /// <param name="ipAddress">IP Address</param>
        /// <param name="userAgent">User Agent</param>
        /// <returns>New audit log entry</returns>
        public static AuditLog CreateInsertLog(int companyId, int userId, string tableName, int recordId,
            object newValues, string? ipAddress = null, string? userAgent = null)
        {
            return new AuditLog
            {
                CompanyId = companyId,
                UserId = userId,
                TableName = tableName,
                RecordId = recordId,
                Action = "Insert",
                NewValues = JsonSerializer.Serialize(newValues),
                IPAddress = ipAddress,
                UserAgent = userAgent
            };
        }

        /// <summary>
        /// Create audit log entry for entity update
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <param name="oldValues">Old values as object</param>
        /// <param name="newValues">New values as object</param>
        /// <param name="ipAddress">IP Address</param>
        /// <param name="userAgent">User Agent</param>
        /// <returns>New audit log entry</returns>
        public static AuditLog CreateUpdateLog(int companyId, int userId, string tableName, int recordId,
            object oldValues, object newValues, string? ipAddress = null, string? userAgent = null)
        {
            return new AuditLog
            {
                CompanyId = companyId,
                UserId = userId,
                TableName = tableName,
                RecordId = recordId,
                Action = "Update",
                OldValues = JsonSerializer.Serialize(oldValues),
                NewValues = JsonSerializer.Serialize(newValues),
                IPAddress = ipAddress,
                UserAgent = userAgent
            };
        }

        /// <summary>
        /// Create audit log entry for entity deletion
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="tableName">Table name</param>
        /// <param name="recordId">Record ID</param>
        /// <param name="oldValues">Old values as object</param>
        /// <param name="ipAddress">IP Address</param>
        /// <param name="userAgent">User Agent</param>
        /// <returns>New audit log entry</returns>
        public static AuditLog CreateDeleteLog(int companyId, int userId, string tableName, int recordId,
            object oldValues, string? ipAddress = null, string? userAgent = null)
        {
            return new AuditLog
            {
                CompanyId = companyId,
                UserId = userId,
                TableName = tableName,
                RecordId = recordId,
                Action = "Delete",
                OldValues = JsonSerializer.Serialize(oldValues),
                IPAddress = ipAddress,
                UserAgent = userAgent
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the audit log</returns>
        public override string ToString() => DisplayName;
    }
}
