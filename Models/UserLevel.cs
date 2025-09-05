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
        public int UserLevelId { get; set; }

        [Required(ErrorMessage = "Level code is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Level code must be between 2 and 20 characters")]
        [Display(Name = "Level Code")]
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

        // Navigation Properties - Let EF handle relationships
        public virtual ICollection<User> Users { get; set; } = new List<User>();

        // Computed Properties
        [NotMapped]
        [Display(Name = "User Level")]
        public string DisplayName => $"{LevelCode} - {LevelName}";

        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        [NotMapped]
        [Display(Name = "Active Users")]
        public int ActiveUsersCount => Users?.Count(u => u.IsActive) ?? 0;

        [NotMapped]
        [Display(Name = "Total Users")]
        public int TotalUsersCount => Users?.Count ?? 0;

        // Static method to create default levels
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

        public override string ToString() => DisplayName;
    }
}
