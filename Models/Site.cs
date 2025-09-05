using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents a site/branch location within a company
    /// Each site is a physical location where courses are conducted
    /// </summary>
    [Table("Sites")]
    public class Site
    {
        [Key]
        public int SiteId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Site code is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "Site code must be between 2 and 10 characters")]
        [Display(Name = "Site Code")]
        public string SiteCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Site name is required")]
        [StringLength(100, ErrorMessage = "Site name cannot exceed 100 characters")]
        [Display(Name = "Site Name")]
        public string SiteName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters")]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(50, ErrorMessage = "Province name cannot exceed 50 characters")]
        [Display(Name = "Province")]
        public string? Province { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Manager name cannot exceed 100 characters")]
        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

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

        // Navigation Properties - Let EF handle relationships
        public virtual Company Company { get; set; } = null!;
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

        // Computed Properties
        [NotMapped]
        [Display(Name = "Display Name")]
        public string DisplayName => $"{SiteCode} - {SiteName}";

        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Active" : "Inactive";

        [NotMapped]
        [Display(Name = "Full Address")]
        public string FullAddress
        {
            get
            {
                var addressParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(Address)) addressParts.Add(Address);
                if (!string.IsNullOrWhiteSpace(City)) addressParts.Add(City);
                if (!string.IsNullOrWhiteSpace(Province)) addressParts.Add(Province);
                return string.Join(", ", addressParts);
            }
        }
    }
}
