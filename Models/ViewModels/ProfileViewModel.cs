using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Company")]
        public string CompanyName { get; set; } = string.Empty;

        [Display(Name = "Site")]
        public string SiteName { get; set; } = string.Empty;

        [Display(Name = "Level")]
        public int UserLevelId { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }
    }
}
