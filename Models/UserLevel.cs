using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents user levels/roles in the KMSI system
    /// Defines hierarchical access levels for different types of users
    /// </summary>
    [Table("UserLevels")]
    public class UserLevel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserLevelId { get; set; }

        [Required(ErrorMessage = "Level code is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Level code must be between 2 and 20 characters")]
        [Display(Name = "Level Code")]
        [RegularExpression("^[A-Z_]+$", ErrorMessage = "Level code must contain only uppercase letters and underscores")]
        public string LevelCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Level name is required")]
        [StringLength(50, ErrorMessage = "Level name cannot exceed 50 characters")]
        [Display(Name = "Level Name")]
        public string LevelName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Range(1, 100, ErrorMessage = "Sort order must be between 1 and 100")]
        [Display(Name = "Sort Order")]
        public int? SortOrder { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation Properties

        /// <summary>
        /// Users assigned to this user level
        /// </summary>
        [Display(Name = "Users")]
        public virtual ICollection<User> Users { get; set; } = new List<User>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining code and name
        /// </summary>
        [NotMapped]
        [Display(Name = "User Level")]
        public string DisplayName => $"{LevelCode} - {LevelName}";

        /// <summary>
        /// Status display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Count of active users in this level
        /// </summary>
        [NotMapped]
        [Display(Name = "Active Users")]
        public int ActiveUsersCount => Users?.Count(u => u.IsActive) ?? 0;

        /// <summary>
        /// Count of inactive users in this level
        /// </summary>
        [NotMapped]
        [Display(Name = "Inactive Users")]
        public int InactiveUsersCount => Users?.Count(u => !u.IsActive) ?? 0;

        /// <summary>
        /// Total users count in this level
        /// </summary>
        [NotMapped]
        [Display(Name = "Total Users")]
        public int TotalUsersCount => Users?.Count ?? 0;

        /// <summary>
        /// Check if this is a system administrator level
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Admin Level")]
        public bool IsAdminLevel => LevelCode == "SUPER" || LevelCode == "HO_ADMIN";

        /// <summary>
        /// Check if this is a management level
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Management Level")]
        public bool IsManagementLevel => LevelCode == "SUPER" || LevelCode == "HO_ADMIN" || LevelCode == "BRANCH_MGR";

        /// <summary>
        /// Check if this is an operational level
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Operational Level")]
        public bool IsOperationalLevel => LevelCode == "TEACHER" || LevelCode == "STAFF";

        /// <summary>
        /// Check if this level can manage other users
        /// </summary>
        [NotMapped]
        [Display(Name = "Can Manage Users")]
        public bool CanManageUsers => LevelCode == "SUPER" || LevelCode == "HO_ADMIN" || LevelCode == "BRANCH_MGR";

        /// <summary>
        /// Check if this level can access financial data
        /// </summary>
        [NotMapped]
        [Display(Name = "Can Access Financial Data")]
        public bool CanAccessFinancialData => LevelCode == "SUPER" || LevelCode == "HO_ADMIN" || LevelCode == "BRANCH_MGR";

        /// <summary>
        /// Check if this level can manage system settings
        /// </summary>
        [NotMapped]
        [Display(Name = "Can Manage System Settings")]
        public bool CanManageSystemSettings => LevelCode == "SUPER" || LevelCode == "HO_ADMIN";

        /// <summary>
        /// Check if this level can access consolidated reports
        /// </summary>
        [NotMapped]
        [Display(Name = "Can Access Consolidated Reports")]
        public bool CanAccessConsolidatedReports => LevelCode == "SUPER" || LevelCode == "HO_ADMIN";

        /// <summary>
        /// Check if this level can manage inventory
        /// </summary>
        [NotMapped]
        [Display(Name = "Can Manage Inventory")]
        public bool CanManageInventory => LevelCode == "SUPER" || LevelCode == "HO_ADMIN" || LevelCode == "BRANCH_MGR";

        /// <summary>
        /// Check if this level can approve book requisitions
        /// </summary>
        [NotMapped]
        [Display(Name = "Can Approve Requisitions")]
        public bool CanApproveRequisitions => LevelCode == "SUPER" || LevelCode == "HO_ADMIN";

        /// <summary>
        /// Check if this level requires site assignment
        /// </summary>
        [NotMapped]
        [Display(Name = "Requires Site Assignment")]
        public bool RequiresSiteAssignment => LevelCode == "BRANCH_MGR" || LevelCode == "TEACHER" || LevelCode == "STAFF";

        /// <summary>
        /// Check if this level can work across multiple sites
        /// </summary>
        [NotMapped]
        [Display(Name = "Can Work Multi-Site")]
        public bool CanWorkMultiSite => LevelCode == "SUPER" || LevelCode == "HO_ADMIN";

        /// <summary>
        /// Get permission level hierarchy value (lower number = higher permission)
        /// </summary>
        [NotMapped]
        [Display(Name = "Hierarchy Level")]
        public int HierarchyLevel => GetHierarchyLevel();

        /// <summary>
        /// Get permissions summary for this level
        /// </summary>
        [NotMapped]
        [Display(Name = "Permissions Summary")]
        public string PermissionsSummary
        {
            get
            {
                var permissions = new List<string>();

                if (CanManageUsers) permissions.Add("User Management");
                if (CanAccessFinancialData) permissions.Add("Financial Access");
                if (CanManageSystemSettings) permissions.Add("System Settings");
                if (CanAccessConsolidatedReports) permissions.Add("Consolidated Reports");
                if (CanManageInventory) permissions.Add("Inventory Management");
                if (CanApproveRequisitions) permissions.Add("Requisition Approval");

                return permissions.Any() ? string.Join(", ", permissions) : "Basic Access";
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Check if level code is unique
        /// </summary>
        /// <param name="otherLevels">Other user levels to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsLevelCodeUnique(IEnumerable<UserLevel> otherLevels)
        {
            return !otherLevels.Any(ul =>
                ul.UserLevelId != UserLevelId &&
                ul.LevelCode.ToUpper() == LevelCode.ToUpper());
        }

        /// <summary>
        /// Check if sort order is unique
        /// </summary>
        /// <param name="otherLevels">Other user levels to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsSortOrderUnique(IEnumerable<UserLevel> otherLevels)
        {
            if (!SortOrder.HasValue) return true;

            return !otherLevels.Any(ul =>
                ul.UserLevelId != UserLevelId &&
                ul.SortOrder == SortOrder &&
                ul.IsActive);
        }

        /// <summary>
        /// Get hierarchy level (1 = highest, 5 = lowest)
        /// </summary>
        /// <returns>Hierarchy level number</returns>
        private int GetHierarchyLevel()
        {
            return LevelCode switch
            {
                "SUPER" => 1,
                "HO_ADMIN" => 2,
                "BRANCH_MGR" => 3,
                "TEACHER" => 4,
                "STAFF" => 5,
                _ => 99 // Unknown levels get lowest priority
            };
        }

        /// <summary>
        /// Check if this level has higher permission than another level
        /// </summary>
        /// <param name="otherLevel">Other user level to compare</param>
        /// <returns>True if this level has higher permission</returns>
        public bool HasHigherPermissionThan(UserLevel otherLevel)
        {
            return HierarchyLevel < otherLevel.HierarchyLevel;
        }

        /// <summary>
        /// Check if this level has same or higher permission than another level
        /// </summary>
        /// <param name="otherLevel">Other user level to compare</param>
        /// <returns>True if this level has same or higher permission</returns>
        public bool HasSameOrHigherPermissionThan(UserLevel otherLevel)
        {
            return HierarchyLevel <= otherLevel.HierarchyLevel;
        }

        /// <summary>
        /// Get users by status in this level
        /// </summary>
        /// <param name="isActive">Active status to filter by</param>
        /// <returns>Users with the specified status</returns>
        public IEnumerable<User> GetUsersByStatus(bool isActive)
        {
            return Users?.Where(u => u.IsActive == isActive) ?? Enumerable.Empty<User>();
        }

        /// <summary>
        /// Get active users in this level
        /// </summary>
        /// <returns>Active users</returns>
        public IEnumerable<User> GetActiveUsers()
        {
            return GetUsersByStatus(true);
        }

        /// <summary>
        /// Get inactive users in this level
        /// </summary>
        /// <returns>Inactive users</returns>
        public IEnumerable<User> GetInactiveUsers()
        {
            return GetUsersByStatus(false);
        }

        /// <summary>
        /// Get users by company in this level
        /// </summary>
        /// <param name="companyId">Company ID to filter by</param>
        /// <returns>Users in the specified company</returns>
        public IEnumerable<User> GetUsersByCompany(int companyId)
        {
            return Users?.Where(u => u.CompanyId == companyId && u.IsActive) ?? Enumerable.Empty<User>();
        }

        /// <summary>
        /// Get users by site in this level
        /// </summary>
        /// <param name="siteId">Site ID to filter by</param>
        /// <returns>Users in the specified site</returns>
        public IEnumerable<User> GetUsersBySite(int siteId)
        {
            return Users?.Where(u => u.SiteId == siteId && u.IsActive) ?? Enumerable.Empty<User>();
        }

        /// <summary>
        /// Get head office users in this level
        /// </summary>
        /// <returns>Users assigned to head office (no site assignment)</returns>
        public IEnumerable<User> GetHeadOfficeUsers()
        {
            return Users?.Where(u => u.SiteId == null && u.IsActive) ?? Enumerable.Empty<User>();
        }

        /// <summary>
        /// Check if level can be safely deleted
        /// </summary>
        /// <returns>True if level can be deleted (no active users assigned)</returns>
        public bool CanBeDeleted()
        {
            return !Users?.Any(u => u.IsActive) ?? true;
        }

        /// <summary>
        /// Get default permissions for this user level
        /// </summary>
        /// <returns>Dictionary of permission flags</returns>
        public Dictionary<string, bool> GetDefaultPermissions()
        {
            return new Dictionary<string, bool>
            {
                // User Management
                {"CanCreateUsers", CanManageUsers},
                {"CanEditUsers", CanManageUsers},
                {"CanDeleteUsers", LevelCode == "SUPER"},
                {"CanAssignRoles", CanManageUsers},
                
                // Company & Site Management
                {"CanManageCompanies", LevelCode == "SUPER"},
                {"CanManageSites", LevelCode == "SUPER" || LevelCode == "HO_ADMIN"},
                {"CanViewAllSites", CanWorkMultiSite},
                
                // Student Management
                {"CanCreateStudents", true}, // All levels can create students
                {"CanEditStudents", true},
                {"CanDeleteStudents", IsManagementLevel},
                {"CanViewAllStudents", CanWorkMultiSite},
                
                // Teacher Management
                {"CanManageTeachers", IsManagementLevel},
                {"CanAssignTeachers", IsManagementLevel},
                {"CanViewTeacherReports", IsManagementLevel},
                
                // Academic Management
                {"CanManageGrades", IsManagementLevel},
                {"CanManageExams", IsManagementLevel || LevelCode == "TEACHER"},
                {"CanIssueCertificates", IsManagementLevel},
                
                // Financial Management
                {"CanViewBilling", CanAccessFinancialData || LevelCode == "TEACHER"},
                {"CanCreateBilling", CanAccessFinancialData},
                {"CanProcessPayments", CanAccessFinancialData},
                {"CanViewFinancialReports", CanAccessFinancialData},
                
                // Inventory Management
                {"CanViewInventory", true}, // All levels can view
                {"CanManageInventory", CanManageInventory},
                {"CanCreateRequisitions", true}, // All levels can create
                {"CanApproveRequisitions", CanApproveRequisitions},
                
                // Reporting & Analytics
                {"CanViewSiteReports", true},
                {"CanViewConsolidatedReports", CanAccessConsolidatedReports},
                {"CanExportReports", IsManagementLevel},
                
                // System Administration
                {"CanManageSystemSettings", CanManageSystemSettings},
                {"CanViewAuditLogs", IsAdminLevel},
                {"CanManageEmailTemplates", IsManagementLevel},
                
                // Scheduling
                {"CanCreateSchedules", IsManagementLevel || LevelCode == "TEACHER"},
                {"CanEditSchedules", IsManagementLevel || LevelCode == "TEACHER"},
                {"CanViewAllSchedules", IsManagementLevel}
            };
        }

        /// <summary>
        /// Get level statistics for dashboard
        /// </summary>
        /// <returns>Dictionary containing level statistics</returns>
        public Dictionary<string, object> GetLevelStatistics()
        {
            return new Dictionary<string, object>
            {
                {"UserLevelId", UserLevelId},
                {"LevelCode", LevelCode},
                {"LevelName", LevelName},
                {"HierarchyLevel", HierarchyLevel},
                {"IsActive", IsActive},
                {"ActiveUsers", ActiveUsersCount},
                {"InactiveUsers", InactiveUsersCount},
                {"TotalUsers", TotalUsersCount},
                {"IsAdminLevel", IsAdminLevel},
                {"IsManagementLevel", IsManagementLevel},
                {"IsOperationalLevel", IsOperationalLevel},
                {"CanManageUsers", CanManageUsers},
                {"RequiresSiteAssignment", RequiresSiteAssignment}
            };
        }

        /// <summary>
        /// Validate user level business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateUserLevelRules()
        {
            var errors = new List<string>();

            // Check for required system levels
            var systemLevels = new[] { "SUPER", "HO_ADMIN", "BRANCH_MGR", "TEACHER", "STAFF" };
            if (!systemLevels.Contains(LevelCode) && !LevelCode.StartsWith("CUSTOM_"))
            {
                errors.Add($"Custom level codes should start with 'CUSTOM_' prefix");
            }

            // Check sort order logic
            if (SortOrder.HasValue && SortOrder <= 0)
            {
                errors.Add("Sort order must be a positive number");
            }

            // Check if deactivating a level with active users
            if (!IsActive && ActiveUsersCount > 0)
            {
                errors.Add($"Cannot deactivate level with {ActiveUsersCount} active users");
            }

            return errors;
        }

        /// <summary>
        /// Get suitable user levels for assignment based on current user's level
        /// </summary>
        /// <param name="currentUserLevel">Current user's level</param>
        /// <param name="availableLevels">All available levels</param>
        /// <returns>Levels that current user can assign</returns>
        public static IEnumerable<UserLevel> GetAssignableLevels(UserLevel currentUserLevel, IEnumerable<UserLevel> availableLevels)
        {
            return availableLevels.Where(level =>
                level.IsActive &&
                currentUserLevel.HasSameOrHigherPermissionThan(level) &&
                level.UserLevelId != currentUserLevel.UserLevelId); // Can't assign same level
        }

        /// <summary>
        /// Create default system user levels
        /// </summary>
        /// <returns>List of default user levels</returns>
        public static List<UserLevel> CreateDefaultLevels()
        {
            return new List<UserLevel>
            {
                new UserLevel
                {
                    LevelCode = "SUPER",
                    LevelName = "Super Admin",
                    Description = "System administrator with full access",
                    SortOrder = 1,
                    IsActive = true
                },
                new UserLevel
                {
                    LevelCode = "HO_ADMIN",
                    LevelName = "Head Office Admin",
                    Description = "Head office administrator",
                    SortOrder = 2,
                    IsActive = true
                },
                new UserLevel
                {
                    LevelCode = "BRANCH_MGR",
                    LevelName = "Branch Manager",
                    Description = "Branch site manager",
                    SortOrder = 3,
                    IsActive = true
                },
                new UserLevel
                {
                    LevelCode = "TEACHER",
                    LevelName = "Teacher",
                    Description = "Music teacher",
                    SortOrder = 4,
                    IsActive = true
                },
                new UserLevel
                {
                    LevelCode = "STAFF",
                    LevelName = "Staff",
                    Description = "General staff member",
                    SortOrder = 5,
                    IsActive = true
                }
            };
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the user level</returns>
        public override string ToString() => DisplayName;
    }
}
