using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    public class RecentActivityViewModel
    {
        /// <summary>
        /// Type of activity (e.g., "New Student", "Examination", "Payment")
        /// </summary>
        [Display(Name = "Activity Type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Description of the activity
        /// </summary>
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the activity occurred
        /// </summary>
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        /// <summary>
        /// FontAwesome icon class for the activity
        /// </summary>
        [Display(Name = "Icon")]
        public string Icon { get; set; } = "fas fa-info-circle";

        /// <summary>
        /// Bootstrap color class for the activity (success, info, warning, danger, etc.)
        /// </summary>
        [Display(Name = "Color")]
        public string Color { get; set; } = "info";

        /// <summary>
        /// Optional URL for linking to the activity details
        /// </summary>
        [Display(Name = "URL")]
        public string? Url { get; set; }

        /// <summary>
        /// User ID who performed the activity
        /// </summary>
        [Display(Name = "User ID")]
        public int? UserId { get; set; }

        /// <summary>
        /// Name of the user who performed the activity
        /// </summary>
        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        /// <summary>
        /// Priority level of the activity (1 = High, 2 = Medium, 3 = Low)
        /// </summary>
        [Display(Name = "Priority")]
        public int Priority { get; set; } = 2;

        /// <summary>
        /// Whether this activity has been read/acknowledged
        /// </summary>
        [Display(Name = "Is Read")]
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Additional data in JSON format for complex activities
        /// </summary>
        [Display(Name = "Additional Data")]
        public string? AdditionalData { get; set; }

        // Computed Properties

        /// <summary>
        /// Relative time display (e.g., "2 hours ago", "yesterday")
        /// </summary>
        [Display(Name = "Time Ago")]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - Date;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 365)
                    return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) != 1 ? "s" : "")} ago";

                return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) != 1 ? "s" : "")} ago";
            }
        }

        /// <summary>
        /// Priority display text
        /// </summary>
        [Display(Name = "Priority Text")]
        public string PriorityText
        {
            get
            {
                return Priority switch
                {
                    1 => "High",
                    2 => "Medium",
                    3 => "Low",
                    _ => "Normal"
                };
            }
        }

        /// <summary>
        /// Priority color class for UI
        /// </summary>
        [Display(Name = "Priority Color")]
        public string PriorityColor
        {
            get
            {
                return Priority switch
                {
                    1 => "danger",
                    2 => "warning",
                    3 => "secondary",
                    _ => "info"
                };
            }
        }

        /// <summary>
        /// Display name for the activity with user info
        /// </summary>
        [Display(Name = "Display Name")]
        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(UserName))
                    return $"{Type} by {UserName}";
                return Type;
            }
        }

        /// <summary>
        /// Check if this activity is recent (within last 24 hours)
        /// </summary>
        [Display(Name = "Is Recent")]
        public bool IsRecent => (DateTime.Now - Date).TotalHours < 24;

        /// <summary>
        /// Check if this activity is today
        /// </summary>
        [Display(Name = "Is Today")]
        public bool IsToday => Date.Date == DateTime.Today;
    }
}
