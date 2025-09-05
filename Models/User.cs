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
        public int UserId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Display(Name = "Site")]
        public int? SiteId { get; set; }

        [Required(ErrorMessage = "User level is required")]
        [Display(Name = "User Level")]
        public int UserLevelId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password hash cannot exceed 255 characters")]
        [Display(Name = "Password Hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone")]
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
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }

        [Display(Name = "Updated Date")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }

        [Display(Name = "Updated By")]
        public int? UpdatedBy { get; set; }

        // Navigation Properties - Let EF handle relationships automatically
        public virtual Company Company { get; set; } = null!;
        public virtual Site? Site { get; set; }
        public virtual UserLevel UserLevel { get; set; } = null!;
        public virtual Teacher? Teacher { get; set; }

        // Collections for business relationships only
        public virtual ICollection<SystemNotification> Notifications { get; set; } = new List<SystemNotification>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [NotMapped]
        public string DisplayName => $"{FullName} ({Username})";

        [NotMapped]
        public bool IsHeadOfficeUser => !SiteId.HasValue;

        [NotMapped]
        public string LocationDisplay => IsHeadOfficeUser ? "Head Office" : Site?.SiteName ?? "Unknown Site";

        // Business Logic Methods
        public void SetPassword(string password)
        {
            PasswordHash = HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return VerifyPassword(password, PasswordHash);
        }

        public void UpdateLastLogin()
        {
            LastLoginDate = DateTime.Now;
        }

        public static string HashPassword(string password, string? salt = null)
        {
            if (string.IsNullOrEmpty(salt))
                salt = "defaultsalt2024";

            using var sha256 = SHA256.Create();
            var saltedPassword = password + salt;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            var hashedPassword = Convert.ToBase64String(hashedBytes);

            return $"{salt}:{hashedPassword}";
        }

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
    }
}
