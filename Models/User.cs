using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a system user including staff, admins, and teachers
    /// Supports multi-tenancy with company and site associations
    /// </summary>
    [Table("Users")]

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Site")]
        [ForeignKey("Site")]
        public int? SiteId { get; set; }

        [Required(ErrorMessage = "User level is required")]
        [Display(Name = "User Level")]
        [ForeignKey("UserLevel")]
        public int UserLevelId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [Display(Name = "Username")]
        [RegularExpression("^[a-zA-Z0-9._]+$", ErrorMessage = "Username can only contain letters, numbers, dots, and underscores")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password hash cannot exceed 255 characters")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Full name computed column (FirstName + LastName)
        /// </summary>
        [StringLength(101, ErrorMessage = "Full name cannot exceed 101 characters")]
        [Display(Name = "Full Name")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(1, ErrorMessage = "Gender must be a single character")]
        [Display(Name = "Gender")]
        [RegularExpression("^[MF]$", ErrorMessage = "Gender must be M (Male) or F (Female)")]
        public string? Gender { get; set; }

        [StringLength(255, ErrorMessage = "Photo path cannot exceed 255 characters")]
        [Display(Name = "Photo")]
        public string? PhotoPath { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Last Login")]
        [DataType(DataType.DateTime)]
        public DateTime? LastLoginDate { get; set; }

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
        /// Company that this user belongs to
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// Site that this user is assigned to (null for head office users)
        /// </summary>
        [Display(Name = "Site")]
        public virtual Site? Site { get; set; }

        /// <summary>
        /// User level/role of this user
        /// </summary>
        [Required]
        [Display(Name = "User Level")]
        public virtual UserLevel UserLevel { get; set; } = null!;

        /// <summary>
        /// Teacher profile if this user is a teacher
        /// </summary>
        [Display(Name = "Teacher Profile")]
        public virtual Teacher? Teacher { get; set; }

        /// <summary>
        /// System notifications for this user
        /// </summary>
        [Display(Name = "Notifications")]
        public virtual ICollection<SystemNotification> Notifications { get; set; } = new List<SystemNotification>();

        /// <summary>
        /// Audit logs for actions performed by this user
        /// </summary>
        [Display(Name = "Audit Logs")]
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        // Audit Navigation Properties (for CreatedBy/UpdatedBy in other entities)

        /// <summary>
        /// Companies created by this user
        /// </summary>
        [InverseProperty("CreatedByUser")]
        public virtual ICollection<Company> CreatedCompanies { get; set; } = new List<Company>();

        /// <summary>
        /// Companies updated by this user
        /// </summary>
        [InverseProperty("UpdatedByUser")]
        public virtual ICollection<Company> UpdatedCompanies { get; set; } = new List<Company>();

        /// <summary>
        /// Sites created by this user
        /// </summary>
        [InverseProperty("CreatedByUser")]
        public virtual ICollection<Site> CreatedSites { get; set; } = new List<Site>();

        /// <summary>
        /// Sites updated by this user
        /// </summary>
        [InverseProperty("UpdatedByUser")]
        public virtual ICollection<Site> UpdatedSites { get; set; } = new List<Site>();

        /// <summary>
        /// Grades created by this user
        /// </summary>
        [InverseProperty("CreatedByUser")]
        public virtual ICollection<Grade> CreatedGrades { get; set; } = new List<Grade>();

        /// <summary>
        /// Grades updated by this user
        /// </summary>
        [InverseProperty("UpdatedByUser")]
        public virtual ICollection<Grade> UpdatedGrades { get; set; } = new List<Grade>();

        /// <summary>
        /// Students created by this user
        /// </summary>
        [InverseProperty("CreatedByUser")]
        public virtual ICollection<Student> CreatedStudents { get; set; } = new List<Student>();

        /// <summary>
        /// Students updated by this user
        /// </summary>
        [InverseProperty("UpdatedByUser")]
        public virtual ICollection<Student> UpdatedStudents { get; set; } = new List<Student>();

        /// <summary>
        /// Teachers created by this user
        /// </summary>
        [InverseProperty("CreatedByUser")]
        public virtual ICollection<Teacher> CreatedTeachers { get; set; } = new List<Teacher>();

        /// <summary>
        /// Teachers updated by this user
        /// </summary>
        [InverseProperty("UpdatedByUser")]
        public virtual ICollection<Teacher> UpdatedTeachers { get; set; } = new List<Teacher>();

        // Self-referencing for audit trail

        /// <summary>
        /// User who created this user record
        /// </summary>
        [InverseProperty("CreatedUsers")]
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// User who last updated this user record
        /// </summary>
        [InverseProperty("UpdatedUsers")]
        public virtual User? UpdatedByUser { get; set; }

        /// <summary>
        /// Users created by this user
        /// </summary>
        [InverseProperty("CreatedByUser")]
        public virtual ICollection<User> CreatedUsers { get; set; } = new List<User>();

        /// <summary>
        /// Users updated by this user
        /// </summary>
        [InverseProperty("UpdatedByUser")]
        public virtual ICollection<User> UpdatedUsers { get; set; } = new List<User>();

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name for UI (Full Name with username)
        /// </summary>
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{FullName} ({Username})";

        /// <summary>
        /// Initials from first and last name
        /// </summary>
        [NotMapped]
        [Display(Name = "Initials")]
        public string Initials
        {
            get
            {
                var firstInitial = !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString().ToUpper() : "";
                var lastInitial = !string.IsNullOrEmpty(LastName) ? LastName[0].ToString().ToUpper() : "";
                return $"{firstInitial}{lastInitial}";
            }
        }

        /// <summary>
        /// Status display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        /// <summary>
        /// Gender display for UI
        /// </summary>
        [NotMapped]
        [Display(Name = "Gender")]
        public string GenderDisplay => Gender switch
        {
            "M" => "Male",
            "F" => "Female",
            _ => "Not Specified"
        };

        /// <summary>
        /// Age calculated from date of birth
        /// </summary>
        [NotMapped]
        [Display(Name = "Age")]
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue) return null;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Days since last login
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Since Last Login")]
        public int? DaysSinceLastLogin
        {
            get
            {
                if (!LastLoginDate.HasValue) return null;
                return (int)(DateTime.Now - LastLoginDate.Value).TotalDays;
            }
        }

        /// <summary>
        /// Check if user is a head office user
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Head Office User")]
        public bool IsHeadOfficeUser => SiteId == null;

        /// <summary>
        /// Check if user is a teacher
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Teacher")]
        public bool IsTeacher => Teacher != null;

        /// <summary>
        /// User's work location display
        /// </summary>
        [NotMapped]
        [Display(Name = "Work Location")]
        public string WorkLocationDisplay => Site?.DisplayName ?? "Head Office";

        /// <summary>
        /// Complete user information for display
        /// </summary>
        [NotMapped]
        [Display(Name = "User Info")]
        public string UserInfoDisplay => $"{FullName} - {UserLevel?.LevelName} at {WorkLocationDisplay}";

        /// <summary>
        /// Count of unread notifications
        /// </summary>
        [NotMapped]
        [Display(Name = "Unread Notifications")]
        public int UnreadNotificationsCount => Notifications?.Count(n => !n.IsRead &&
            (n.ExpiryDate == null || n.ExpiryDate > DateTime.Now)) ?? 0;

        /// <summary>
        /// Check if user has new notifications
        /// </summary>
        [NotMapped]
        [Display(Name = "Has New Notifications")]
        public bool HasNewNotifications => UnreadNotificationsCount > 0;

        /// <summary>
        /// User's full address
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Address")]
        public string FullAddress
        {
            get
            {
                var addressParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Address)) addressParts.Add(Address);
                if (!string.IsNullOrWhiteSpace(City)) addressParts.Add(City);
                return string.Join(", ", addressParts);
            }
        }

        /// <summary>
        /// Check if password needs to be changed (older than 90 days)
        /// </summary>
        [NotMapped]
        [Display(Name = "Password Needs Change")]
        public bool PasswordNeedsChange
        {
            get
            {
                if (!UpdatedDate.HasValue) return (DateTime.Now - CreatedDate).TotalDays > 90;
                return (DateTime.Now - UpdatedDate.Value).TotalDays > 90;
            }
        }

        // Business Logic Methods

        /// <summary>
        /// Check if username is unique within the same company
        /// </summary>
        /// <param name="otherUsers">Other users to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsUsernameUnique(IEnumerable<User> otherUsers)
        {
            return !otherUsers.Any(u =>
                u.UserId != UserId &&
                u.Username.ToLower() == Username.ToLower() &&
                u.CompanyId == CompanyId);
        }

        /// <summary>
        /// Check if email is unique across all users
        /// </summary>
        /// <param name="otherUsers">Other users to check against</param>
        /// <returns>True if unique, false otherwise</returns>
        public bool IsEmailUnique(IEnumerable<User> otherUsers)
        {
            return !otherUsers.Any(u =>
                u.UserId != UserId &&
                u.Email.ToLower() == Email.ToLower());
        }

        /// <summary>
        /// Hash password using SHA-256 with salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="salt">Salt for hashing (optional, will generate if not provided)</param>
        /// <returns>Hashed password with salt</returns>
        public static string HashPassword(string password, string? salt = null)
        {
            if (string.IsNullOrEmpty(salt))
                salt = GenerateSalt();

            using var sha256 = SHA256.Create();
            var saltedPassword = password + salt;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            var hashedPassword = Convert.ToBase64String(hashedBytes);

            return $"{salt}:{hashedPassword}";
        }

        /// <summary>
        /// Verify password against stored hash
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="storedHash">Stored password hash</param>
        /// <returns>True if password matches</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split(':');
                if (parts.Length != 2) return false;

                var salt = parts[0];
                var hash = parts[1];

                var newHash = HashPassword(password, salt);
                return newHash == storedHash;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generate random salt for password hashing
        /// </summary>
        /// <returns>Random salt string</returns>
        private static string GenerateSalt()
        {
            var saltBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Set user password with automatic hashing
        /// </summary>
        /// <param name="password">Plain text password</param>
        public void SetPassword(string password)
        {
            PasswordHash = HashPassword(password);
        }

        /// <summary>
        /// Verify user password
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <returns>True if password is correct</returns>
        public bool VerifyPassword(string password)
        {
            return VerifyPassword(password, PasswordHash);
        }

        /// <summary>
        /// Update last login date
        /// </summary>
        public void UpdateLastLogin()
        {
            LastLoginDate = DateTime.Now;
        }

        /// <summary>
        /// Check if user can access specific site
        /// </summary>
        /// <param name="targetSiteId">Site ID to check access</param>
        /// <returns>True if user can access the site</returns>
        public bool CanAccessSite(int targetSiteId)
        {
            // Head office users can access all sites within their company
            if (IsHeadOfficeUser) return true;

            // Site users can only access their own site
            return SiteId == targetSiteId;
        }

        /// <summary>
        /// Check if user can access specific company data
        /// </summary>
        /// <param name="targetCompanyId">Company ID to check access</param>
        /// <returns>True if user can access the company data</returns>
        public bool CanAccessCompany(int targetCompanyId)
        {
            return CompanyId == targetCompanyId;
        }

        /// <summary>
        /// Check if user has specific permission level
        /// </summary>
        /// <param name="requiredLevel">Required user level code</param>
        /// <returns>True if user has required level or higher</returns>
        public bool HasPermissionLevel(string requiredLevel)
        {
            var levelHierarchy = new Dictionary<string, int>
            {
                {"SUPER", 1},
                {"HO_ADMIN", 2},
                {"BRANCH_MGR", 3},
                {"TEACHER", 4},
                {"STAFF", 5}
            };

            if (!levelHierarchy.ContainsKey(UserLevel.LevelCode) ||
                !levelHierarchy.ContainsKey(requiredLevel))
                return false;

            return levelHierarchy[UserLevel.LevelCode] <= levelHierarchy[requiredLevel];
        }

        /// <summary>
        /// Get user's active notifications
        /// </summary>
        /// <returns>Active (non-expired) notifications</returns>
        public IEnumerable<SystemNotification> GetActiveNotifications()
        {
            return Notifications?.Where(n =>
                n.ExpiryDate == null || n.ExpiryDate > DateTime.Now)
                .OrderByDescending(n => n.CreatedDate) ?? Enumerable.Empty<SystemNotification>();
        }

        /// <summary>
        /// Get user's unread notifications
        /// </summary>
        /// <returns>Unread and active notifications</returns>
        public IEnumerable<SystemNotification> GetUnreadNotifications()
        {
            return GetActiveNotifications().Where(n => !n.IsRead);
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        public void MarkAllNotificationsAsRead()
        {
            var unreadNotifications = GetUnreadNotifications().ToList();
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Generate user profile summary
        /// </summary>
        /// <returns>Dictionary containing user profile data</returns>
        public Dictionary<string, object> GetProfileSummary()
        {
            return new Dictionary<string, object>
            {
                {"UserId", UserId},
                {"Username", Username},
                {"FullName", FullName},
                {"Email", Email},
                {"UserLevel", UserLevel?.LevelName},
                {"Company", Company?.CompanyName},
                {"Site", Site?.SiteName ?? "Head Office"},
                {"IsActive", IsActive},
                {"IsTeacher", IsTeacher},
                {"Age", Age},
                {"LastLogin", LastLoginDate},
                {"DaysSinceLastLogin", DaysSinceLastLogin},
                {"UnreadNotifications", UnreadNotificationsCount},
                {"PasswordNeedsChange", PasswordNeedsChange}
            };
        }

        /// <summary>
        /// Validate user business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateUserRules()
        {
            var errors = new List<string>();

            // Head office users should not have site assignment
            if (IsHeadOfficeUser && UserLevel?.LevelCode == "HO_ADMIN" && SiteId.HasValue)
            {
                errors.Add("Head office administrators should not be assigned to a specific site");
            }

            // Site-specific users should have site assignment
            if (!IsHeadOfficeUser && UserLevel?.LevelCode == "BRANCH_MGR" && !SiteId.HasValue)
            {
                errors.Add("Branch managers must be assigned to a specific site");
            }

            // Teachers must have site assignment
            if (UserLevel?.LevelCode == "TEACHER" && !SiteId.HasValue)
            {
                errors.Add("Teachers must be assigned to a specific site");
            }

            // Age validation for date of birth
            if (DateOfBirth.HasValue && Age < 16)
            {
                errors.Add("User must be at least 16 years old");
            }

            if (DateOfBirth.HasValue && Age > 100)
            {
                errors.Add("Invalid date of birth");
            }

            return errors;
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the user</returns>
        public override string ToString() => DisplayName;
    }
}
