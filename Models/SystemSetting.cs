using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents system settings in the KMSI Course Management System
    /// Supports both global and company-specific configuration settings
    /// </summary>
    [Table("SystemSettings")]
    public class SystemSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SettingId { get; set; }

        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int? CompanyId { get; set; } // NULL for global settings

        [Required(ErrorMessage = "Setting key is required")]
        [StringLength(100, ErrorMessage = "Setting key cannot exceed 100 characters")]
        [Display(Name = "Setting Key")]
        [RegularExpression("^[A-Za-z0-9._-]+$", ErrorMessage = "Setting key can only contain letters, numbers, dots, hyphens, and underscores")]
        public string SettingKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "Setting value is required")]
        [StringLength(1000, ErrorMessage = "Setting value cannot exceed 1000 characters")]
        [Display(Name = "Setting Value")]
        public string SettingValue { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data type is required")]
        [StringLength(20, ErrorMessage = "Data type cannot exceed 20 characters")]
        [Display(Name = "Data Type")]
        public string DataType { get; set; } = "String"; // String, Integer, Boolean, Decimal, Date, JSON

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        [Display(Name = "Description")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        [Display(Name = "Is Editable")]
        public bool IsEditable { get; set; } = true;

        [Display(Name = "Updated Date")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Updated By")]
        [ForeignKey("UpdatedByUser")]
        public int? UpdatedBy { get; set; }

        // Navigation Properties

        /// <summary>
        /// Company this setting belongs to (null for global settings)
        /// </summary>
        [Display(Name = "Company")]
        public virtual Company? Company { get; set; }

        /// <summary>
        /// User who last updated this setting
        /// </summary>
        [InverseProperty("UpdatedSystemSettings")]
        public virtual User? UpdatedByUser { get; set; }

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining key and description
        /// </summary>
        [NotMapped]
        [Display(Name = "Setting")]
        public string DisplayName => !string.IsNullOrEmpty(Description)
            ? $"{SettingKey} - {Description}"
            : SettingKey;

        /// <summary>
        /// Scope display (Global or Company specific)
        /// </summary>
        [NotMapped]
        [Display(Name = "Scope")]
        public string ScopeDisplay => CompanyId.HasValue ? $"Company: {Company?.CompanyName}" : "Global";

        /// <summary>
        /// Full display name with scope
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Setting Name")]
        public string FullDisplayName => $"{SettingKey} ({ScopeDisplay})";

        /// <summary>
        /// Editable status display
        /// </summary>
        [NotMapped]
        [Display(Name = "Editable Status")]
        public string EditableStatusDisplay => IsEditable ? "Editable" : "Read Only";

        /// <summary>
        /// Data type display with validation info
        /// </summary>
        [NotMapped]
        [Display(Name = "Data Type Info")]
        public string DataTypeDisplay => DataType switch
        {
            "String" => "Text",
            "Integer" => "Whole Number",
            "Boolean" => "True/False",
            "Decimal" => "Decimal Number",
            "Date" => "Date",
            "JSON" => "JSON Object",
            _ => DataType
        };

        /// <summary>
        /// Category display with default fallback
        /// </summary>
        [NotMapped]
        [Display(Name = "Category")]
        public string CategoryDisplay => Category ?? "General";

        /// <summary>
        /// Days since last update
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Updated")]
        public int DaysSinceUpdated => (DateTime.Now - UpdatedDate).Days;

        /// <summary>
        /// Check if setting is global (applies to all companies)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Global")]
        public bool IsGlobal => !CompanyId.HasValue;

        /// <summary>
        /// Check if setting is company-specific
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Company Specific")]
        public bool IsCompanySpecific => CompanyId.HasValue;

        /// <summary>
        /// Formatted value for display based on data type
        /// </summary>
        [NotMapped]
        [Display(Name = "Formatted Value")]
        public string FormattedValue
        {
            get
            {
                try
                {
                    return DataType switch
                    {
                        "Boolean" => GetBooleanValue() ? "Yes" : "No",
                        "Date" => GetDateValue()?.ToString("yyyy-MM-dd") ?? "Invalid Date",
                        "Decimal" => GetDecimalValue().ToString("F2"),
                        "Integer" => GetIntegerValue().ToString(),
                        "JSON" => "JSON Object",
                        _ => SettingValue
                    };
                }
                catch
                {
                    return $"Invalid {DataType}";
                }
            }
        }

        /// <summary>
        /// Value summary for lists (truncated if too long)
        /// </summary>
        [NotMapped]
        [Display(Name = "Value Summary")]
        public string ValueSummary => SettingValue.Length > 50
            ? SettingValue.Substring(0, 47) + "..."
            : SettingValue;

        // Typed Value Getters

        /// <summary>
        /// Get setting value as string
        /// </summary>
        /// <returns>String value</returns>
        public string GetStringValue() => SettingValue;

        /// <summary>
        /// Get setting value as integer
        /// </summary>
        /// <returns>Integer value or default</returns>
        public int GetIntegerValue()
        {
            return int.TryParse(SettingValue, out int result) ? result : 0;
        }

        /// <summary>
        /// Get setting value as boolean
        /// </summary>
        /// <returns>Boolean value or false</returns>
        public bool GetBooleanValue()
        {
            return bool.TryParse(SettingValue, out bool result) && result;
        }

        /// <summary>
        /// Get setting value as decimal
        /// </summary>
        /// <returns>Decimal value or 0</returns>
        public decimal GetDecimalValue()
        {
            return decimal.TryParse(SettingValue, out decimal result) ? result : 0m;
        }

        /// <summary>
        /// Get setting value as DateTime
        /// </summary>
        /// <returns>DateTime value or null</returns>
        public DateTime? GetDateValue()
        {
            return DateTime.TryParse(SettingValue, out DateTime result) ? result : null;
        }

        /// <summary>
        /// Get setting value as JSON object
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <returns>Deserialized object or default</returns>
        public T? GetJsonValue<T>() where T : class
        {
            try
            {
                return JsonSerializer.Deserialize<T>(SettingValue);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Get setting value as dynamic JSON object
        /// </summary>
        /// <returns>JsonDocument or null</returns>
        public JsonDocument? GetJsonDocument()
        {
            try
            {
                return JsonDocument.Parse(SettingValue);
            }
            catch
            {
                return null;
            }
        }

        // Typed Value Setters

        /// <summary>
        /// Set setting value from string
        /// </summary>
        /// <param name="value">String value</param>
        public void SetStringValue(string value)
        {
            DataType = "String";
            SettingValue = value;
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Set setting value from integer
        /// </summary>
        /// <param name="value">Integer value</param>
        public void SetIntegerValue(int value)
        {
            DataType = "Integer";
            SettingValue = value.ToString();
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Set setting value from boolean
        /// </summary>
        /// <param name="value">Boolean value</param>
        public void SetBooleanValue(bool value)
        {
            DataType = "Boolean";
            SettingValue = value.ToString().ToLower();
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Set setting value from decimal
        /// </summary>
        /// <param name="value">Decimal value</param>
        public void SetDecimalValue(decimal value)
        {
            DataType = "Decimal";
            SettingValue = value.ToString("F2");
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Set setting value from DateTime
        /// </summary>
        /// <param name="value">DateTime value</param>
        public void SetDateValue(DateTime value)
        {
            DataType = "Date";
            SettingValue = value.ToString("yyyy-MM-dd HH:mm:ss");
            UpdatedDate = DateTime.Now;
        }

        /// <summary>
        /// Set setting value from JSON object
        /// </summary>
        /// <param name="value">Object to serialize to JSON</param>
        public void SetJsonValue(object value)
        {
            DataType = "JSON";
            SettingValue = JsonSerializer.Serialize(value);
            UpdatedDate = DateTime.Now;
        }

        // Business Logic Methods

        /// <summary>
        /// Check if setting key is unique within the same scope (global or company)
        /// </summary>
        /// <param name="otherSettings">Other settings to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsSettingKeyUnique(IEnumerable<SystemSetting> otherSettings)
        {
            return !otherSettings.Any(s =>
                s.SettingId != SettingId &&
                s.SettingKey.ToLower() == SettingKey.ToLower() &&
                s.CompanyId == CompanyId);
        }

        /// <summary>
        /// Validate setting value against data type
        /// </summary>
        /// <returns>True if value is valid for the data type</returns>
        public bool IsValueValidForDataType()
        {
            try
            {
                return DataType switch
                {
                    "String" => true, // Strings are always valid
                    "Integer" => int.TryParse(SettingValue, out _),
                    "Boolean" => bool.TryParse(SettingValue, out _),
                    "Decimal" => decimal.TryParse(SettingValue, out _),
                    "Date" => DateTime.TryParse(SettingValue, out _),
                    "JSON" => IsValidJson(),
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the value is valid JSON
        /// </summary>
        /// <returns>True if valid JSON</returns>
        private bool IsValidJson()
        {
            try
            {
                JsonDocument.Parse(SettingValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update setting value with validation
        /// </summary>
        /// <param name="newValue">New value</param>
        /// <param name="updatedBy">User ID who updated</param>
        /// <returns>True if update successful</returns>
        public bool UpdateValue(string newValue, int updatedBy)
        {
            if (!IsEditable) return false;

            var oldValue = SettingValue;
            SettingValue = newValue;

            if (!IsValueValidForDataType())
            {
                SettingValue = oldValue; // Revert if invalid
                return false;
            }

            UpdatedBy = updatedBy;
            UpdatedDate = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Get setting default value based on data type
        /// </summary>
        /// <returns>Default value for the data type</returns>
        public string GetDefaultValue()
        {
            return DataType switch
            {
                "String" => string.Empty,
                "Integer" => "0",
                "Boolean" => "false",
                "Decimal" => "0.00",
                "Date" => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                "JSON" => "{}",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Reset setting to default value
        /// </summary>
        /// <param name="updatedBy">User ID who reset</param>
        public void ResetToDefault(int updatedBy)
        {
            if (IsEditable)
            {
                SettingValue = GetDefaultValue();
                UpdatedBy = updatedBy;
                UpdatedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Clone setting for another company
        /// </summary>
        /// <param name="targetCompanyId">Target company ID</param>
        /// <param name="createdBy">User ID who created the clone</param>
        /// <returns>Cloned setting</returns>
        public SystemSetting CloneForCompany(int targetCompanyId, int createdBy)
        {
            return new SystemSetting
            {
                CompanyId = targetCompanyId,
                SettingKey = SettingKey,
                SettingValue = SettingValue,
                DataType = DataType,
                Description = Description,
                Category = Category,
                IsEditable = IsEditable,
                UpdatedBy = createdBy,
                UpdatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Get setting categories from a collection of settings
        /// </summary>
        /// <param name="settings">Settings collection</param>
        /// <returns>List of unique categories</returns>
        public static List<string> GetCategories(IEnumerable<SystemSetting> settings)
        {
            return settings
                .Where(s => !string.IsNullOrEmpty(s.Category))
                .Select(s => s.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        /// <summary>
        /// Get settings grouped by category
        /// </summary>
        /// <param name="settings">Settings collection</param>
        /// <returns>Dictionary of settings grouped by category</returns>
        public static Dictionary<string, List<SystemSetting>> GroupByCategory(IEnumerable<SystemSetting> settings)
        {
            return settings
                .GroupBy(s => s.CategoryDisplay)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.SettingKey).ToList());
        }

        /// <summary>
        /// Get effective setting value (company-specific overrides global)
        /// </summary>
        /// <param name="allSettings">All available settings</param>
        /// <param name="settingKey">Setting key to find</param>
        /// <param name="companyId">Company ID (null for global)</param>
        /// <returns>Effective setting or null if not found</returns>
        public static SystemSetting? GetEffectiveSetting(IEnumerable<SystemSetting> allSettings, string settingKey, int? companyId = null)
        {
            // First try to find company-specific setting
            if (companyId.HasValue)
            {
                var companySetting = allSettings.FirstOrDefault(s =>
                    s.SettingKey == settingKey && s.CompanyId == companyId);
                if (companySetting != null) return companySetting;
            }

            // Fallback to global setting
            return allSettings.FirstOrDefault(s =>
                s.SettingKey == settingKey && s.CompanyId == null);
        }

        /// <summary>
        /// Create default system settings
        /// </summary>
        /// <returns>List of default settings</returns>
        public static List<SystemSetting> CreateDefaultSettings()
        {
            return new List<SystemSetting>
            {
                new SystemSetting
                {
                    SettingKey = "DefaultClassDuration",
                    SettingValue = "60",
                    DataType = "Integer",
                    Description = "Default class duration in minutes",
                    Category = "Scheduling",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "TrialClassDuration",
                    SettingValue = "30",
                    DataType = "Integer",
                    Description = "Trial class duration in minutes",
                    Category = "Scheduling",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "MaxStudentsPerTeacher",
                    SettingValue = "20",
                    DataType = "Integer",
                    Description = "Maximum students per teacher",
                    Category = "Scheduling",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "BillingReminderDays",
                    SettingValue = "3",
                    DataType = "Integer",
                    Description = "Days before due date to send billing reminder",
                    Category = "Billing",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "AutoGenerateScheduleWeeks",
                    SettingValue = "12",
                    DataType = "Integer",
                    Description = "Number of weeks to auto-generate schedule",
                    Category = "Scheduling",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "MinimumBookStock",
                    SettingValue = "5",
                    DataType = "Integer",
                    Description = "Minimum book stock level",
                    Category = "Inventory",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "ReorderBookStock",
                    SettingValue = "10",
                    DataType = "Integer",
                    Description = "Reorder level for book stock",
                    Category = "Inventory",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "EnableEmailNotifications",
                    SettingValue = "true",
                    DataType = "Boolean",
                    Description = "Enable automatic email notifications",
                    Category = "Notifications",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "DefaultTaxRate",
                    SettingValue = "0.05",
                    DataType = "Decimal",
                    Description = "Default tax rate for payroll (5%)",
                    Category = "Financial",
                    IsEditable = true
                },
                new SystemSetting
                {
                    SettingKey = "SystemMaintenanceMode",
                    SettingValue = "false",
                    DataType = "Boolean",
                    Description = "Enable system maintenance mode",
                    Category = "System",
                    IsEditable = false
                }
            };
        }

        /// <summary>
        /// Get setting summary for dashboard
        /// </summary>
        /// <returns>Setting summary data</returns>
        public Dictionary<string, object> GetSettingSummary()
        {
            return new Dictionary<string, object>
            {
                {"SettingId", SettingId},
                {"SettingKey", SettingKey},
                {"SettingValue", SettingValue},
                {"FormattedValue", FormattedValue},
                {"DataType", DataType},
                {"Category", CategoryDisplay},
                {"Description", Description},
                {"IsGlobal", IsGlobal},
                {"IsCompanySpecific", IsCompanySpecific},
                {"CompanyName", Company?.CompanyName},
                {"IsEditable", IsEditable},
                {"LastUpdated", UpdatedDate},
                {"DaysSinceUpdated", DaysSinceUpdated},
                {"UpdatedBy", UpdatedByUser?.FullName}
            };
        }

        /// <summary>
        /// Validate system setting business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateSystemSettingRules()
        {
            var errors = new List<string>();

            // Value must be valid for data type
            if (!IsValueValidForDataType())
            {
                errors.Add($"Value '{SettingValue}' is not valid for data type '{DataType}'");
            }

            // System-critical settings should not be editable
            var systemCriticalKeys = new[] { "SystemMaintenanceMode", "DatabaseVersion", "SystemVersion" };
            if (systemCriticalKeys.Contains(SettingKey) && IsEditable)
            {
                errors.Add($"System-critical setting '{SettingKey}' should not be editable");
            }

            // Global settings should not have company ID
            if (IsGlobal && CompanyId.HasValue)
            {
                errors.Add("Global settings should not have company assignment");
            }

            // Numeric range validations for specific settings
            if (SettingKey == "DefaultClassDuration" && GetIntegerValue() < 15)
            {
                errors.Add("Default class duration should be at least 15 minutes");
            }

            if (SettingKey == "MaxStudentsPerTeacher" && GetIntegerValue() < 1)
            {
                errors.Add("Maximum students per teacher should be at least 1");
            }

            return errors;
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the setting</returns>
        public override string ToString() => DisplayName;
    }
}
