using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMSI_Projects.Models
{
    /// <summary>
    /// Represents system notifications in the KMSI Course Management System
    /// Used for in-app notifications, alerts, and announcements to users
    /// </summary>
    [Table("SystemNotifications")]
    public class SystemNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "User is required")]
        [Display(Name = "User")]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        [Display(Name = "Message")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Notification type is required")]
        [StringLength(50, ErrorMessage = "Notification type cannot exceed 50 characters")]
        [Display(Name = "Notification Type")]
        public string NotificationType { get; set; } = "Info"; // Info, Warning, Error, Success

        [Display(Name = "Is Read")]
        public bool IsRead { get; set; } = false;

        [Display(Name = "Read Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ReadDate { get; set; }

        [Display(Name = "Expiry Date")]
        [DataType(DataType.DateTime)]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties

        /// <summary>
        /// Company this notification belongs to
        /// </summary>
        [Required]
        [Display(Name = "Company")]
        public virtual Company Company { get; set; } = null!;

        /// <summary>
        /// User this notification is for
        /// </summary>
        [Required]
        [Display(Name = "User")]
        public virtual User User { get; set; } = null!;

        // Computed Properties (Not Mapped)

        /// <summary>
        /// Display name combining type and title
        /// </summary>
        [NotMapped]
        [Display(Name = "Notification")]
        public string DisplayName => $"[{NotificationType}] {Title}";

        /// <summary>
        /// Short display for notification lists
        /// </summary>
        [NotMapped]
        [Display(Name = "Short Display")]
        public string ShortDisplay => Title.Length > 30 ? $"{Title.Substring(0, 27)}..." : Title;

        /// <summary>
        /// Read status display
        /// </summary>
        [NotMapped]
        [Display(Name = "Status")]
        public string StatusDisplay => IsRead ? "Read" : "Unread";

        /// <summary>
        /// Status color for UI styling
        /// </summary>
        [NotMapped]
        [Display(Name = "Status Color")]
        public string StatusColor => IsRead ? "secondary" : "primary";

        /// <summary>
        /// Notification type color for UI styling
        /// </summary>
        [NotMapped]
        [Display(Name = "Type Color")]
        public string TypeColor => NotificationType.ToLower() switch
        {
            "info" => "info",
            "warning" => "warning",
            "error" => "danger",
            "success" => "success",
            "alert" => "dark",
            "reminder" => "warning",
            "announcement" => "primary",
            "system" => "secondary",
            _ => "info"
        };

        /// <summary>
        /// Notification type icon
        /// </summary>
        [NotMapped]
        [Display(Name = "Type Icon")]
        public string TypeIcon => NotificationType.ToLower() switch
        {
            "info" => "fa-info-circle",
            "warning" => "fa-exclamation-triangle",
            "error" => "fa-times-circle",
            "success" => "fa-check-circle",
            "alert" => "fa-bell",
            "reminder" => "fa-clock",
            "announcement" => "fa-bullhorn",
            "system" => "fa-cog",
            _ => "fa-info-circle"
        };

        /// <summary>
        /// Notification priority based on type
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority")]
        public int Priority => NotificationType.ToLower() switch
        {
            "error" => 1,
            "alert" => 2,
            "warning" => 3,
            "reminder" => 4,
            "success" => 5,
            "announcement" => 6,
            "info" => 7,
            "system" => 8,
            _ => 9
        };

        /// <summary>
        /// Priority display
        /// </summary>
        [NotMapped]
        [Display(Name = "Priority Level")]
        public string PriorityDisplay => Priority switch
        {
            1 => "Critical",
            2 => "High",
            3 => "Medium",
            4 => "Normal",
            >= 5 => "Low",
            _ => "Unknown"
        };

        /// <summary>
        /// Time ago display (e.g., "2 hours ago")
        /// </summary>
        [NotMapped]
        [Display(Name = "Time Ago")]
        public string TimeAgoDisplay
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedDate;

                if (timeSpan.TotalMinutes < 1) return "Just now";
                if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
                if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) != 1 ? "s" : "")} ago";

                return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) != 1 ? "s" : "")} ago";
            }
        }

        /// <summary>
        /// Message preview (first 100 characters)
        /// </summary>
        [NotMapped]
        [Display(Name = "Message Preview")]
        public string MessagePreview => Message.Length > 100 ? $"{Message.Substring(0, 97)}..." : Message;

        /// <summary>
        /// Check if notification is active (not expired and not too old)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Active")]
        public bool IsActive
        {
            get
            {
                // Check if expired
                if (ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now)
                    return false;

                // Check if too old (older than 30 days)
                if ((DateTime.Now - CreatedDate).TotalDays > 30)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Check if notification is expired
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Expired")]
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Now;

        /// <summary>
        /// Check if notification is recent (created within last 24 hours)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Recent")]
        public bool IsRecent => (DateTime.Now - CreatedDate).TotalHours < 24;

        /// <summary>
        /// Check if notification is urgent (high priority unread)
        /// </summary>
        [NotMapped]
        [Display(Name = "Is Urgent")]
        public bool IsUrgent => !IsRead && Priority <= 3;

        /// <summary>
        /// Days until expiry (null if no expiry or already expired)
        /// </summary>
        [NotMapped]
        [Display(Name = "Days Until Expiry")]
        public int? DaysUntilExpiry
        {
            get
            {
                if (!ExpiryDate.HasValue || IsExpired) return null;
                return (int)Math.Ceiling((ExpiryDate.Value - DateTime.Now).TotalDays);
            }
        }

        /// <summary>
        /// Age of notification in days
        /// </summary>
        [NotMapped]
        [Display(Name = "Age (Days)")]
        public int AgeInDays => (int)Math.Floor((DateTime.Now - CreatedDate).TotalDays);

        /// <summary>
        /// Full notification display for detailed views
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Display")]
        public string FullDisplay => $"{DisplayName}\n{Message}\n{TimeAgoDisplay}";

        // Business Logic Methods

        /// <summary>
        /// Mark notification as read
        /// </summary>
        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Mark notification as unread
        /// </summary>
        public void MarkAsUnread()
        {
            if (IsRead)
            {
                IsRead = false;
                ReadDate = null;
            }
        }

        /// <summary>
        /// Set expiry date for the notification
        /// </summary>
        /// <param name="expiryDate">Date when notification should expire</param>
        public void SetExpiry(DateTime expiryDate)
        {
            if (expiryDate > CreatedDate)
            {
                ExpiryDate = expiryDate;
            }
        }

        /// <summary>
        /// Set expiry relative to creation date
        /// </summary>
        /// <param name="days">Days from creation date</param>
        public void SetExpiryInDays(int days)
        {
            if (days > 0)
            {
                ExpiryDate = CreatedDate.AddDays(days);
            }
        }

        /// <summary>
        /// Remove expiry date (make notification permanent)
        /// </summary>
        public void RemoveExpiry()
        {
            ExpiryDate = null;
        }

        /// <summary>
        /// Check if notification should be displayed to user
        /// </summary>
        /// <param name="showRead">Whether to show read notifications</param>
        /// <param name="showExpired">Whether to show expired notifications</param>
        /// <returns>True if notification should be displayed</returns>
        public bool ShouldDisplay(bool showRead = true, bool showExpired = false)
        {
            if (IsExpired && !showExpired) return false;
            if (IsRead && !showRead) return false;
            return true;
        }

        /// <summary>
        /// Get notification age category
        /// </summary>
        /// <returns>Age category string</returns>
        public string GetAgeCategory()
        {
            var hours = (DateTime.Now - CreatedDate).TotalHours;
            return hours switch
            {
                < 1 => "New",
                < 24 => "Recent",
                < 168 => "This Week", // 7 days
                < 720 => "This Month", // 30 days
                _ => "Old"
            };
        }

        /// <summary>
        /// Create a reminder notification
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="title">Reminder title</param>
        /// <param name="message">Reminder message</param>
        /// <param name="reminderDate">When to remind (sets as expiry)</param>
        /// <returns>Reminder notification</returns>
        public static SystemNotification CreateReminder(int companyId, int userId, string title, string message, DateTime? reminderDate = null)
        {
            var notification = new SystemNotification
            {
                CompanyId = companyId,
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = "Reminder",
                CreatedDate = DateTime.Now
            };

            if (reminderDate.HasValue && reminderDate.Value > DateTime.Now)
            {
                notification.SetExpiry(reminderDate.Value.AddDays(7)); // Expire 7 days after reminder date
            }

            return notification;
        }

        /// <summary>
        /// Create an alert notification
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="title">Alert title</param>
        /// <param name="message">Alert message</param>
        /// <returns>Alert notification</returns>
        public static SystemNotification CreateAlert(int companyId, int userId, string title, string message)
        {
            return new SystemNotification
            {
                CompanyId = companyId,
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = "Alert",
                CreatedDate = DateTime.Now
            };
        }

        /// <summary>
        /// Create a success notification
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="title">Success title</param>
        /// <param name="message">Success message</param>
        /// <returns>Success notification</returns>
        public static SystemNotification CreateSuccess(int companyId, int userId, string title, string message)
        {
            var notification = new SystemNotification
            {
                CompanyId = companyId,
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = "Success",
                CreatedDate = DateTime.Now
            };

            // Success notifications expire in 7 days
            notification.SetExpiryInDays(7);
            return notification;
        }

        /// <summary>
        /// Create an error notification
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="title">Error title</param>
        /// <param name="message">Error message</param>
        /// <returns>Error notification</returns>
        public static SystemNotification CreateError(int companyId, int userId, string title, string message)
        {
            return new SystemNotification
            {
                CompanyId = companyId,
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = "Error",
                CreatedDate = DateTime.Now
                // Error notifications don't expire automatically
            };
        }

        /// <summary>
        /// Create an announcement notification
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="title">Announcement title</param>
        /// <param name="message">Announcement message</param>
        /// <param name="expiryDays">Days until announcement expires</param>
        /// <returns>Announcement notification</returns>
        public static SystemNotification CreateAnnouncement(int companyId, int userId, string title, string message, int expiryDays = 30)
        {
            var notification = new SystemNotification
            {
                CompanyId = companyId,
                UserId = userId,
                Title = title,
                Message = message,
                NotificationType = "Announcement",
                CreatedDate = DateTime.Now
            };

            notification.SetExpiryInDays(expiryDays);
            return notification;
        }

        /// <summary>
        /// Get notification statistics for a user
        /// </summary>
        /// <param name="notifications">User's notifications</param>
        /// <returns>Statistics dictionary</returns>
        public static Dictionary<string, object> GetUserNotificationStats(IEnumerable<SystemNotification> notifications)
        {
            var activeNotifications = notifications.Where(n => n.IsActive).ToList();

            return new Dictionary<string, object>
            {
                {"Total", notifications.Count()},
                {"Active", activeNotifications.Count},
                {"Unread", activeNotifications.Count(n => !n.IsRead)},
                {"Read", activeNotifications.Count(n => n.IsRead)},
                {"Urgent", activeNotifications.Count(n => n.IsUrgent)},
                {"Recent", activeNotifications.Count(n => n.IsRecent)},
                {"Expired", notifications.Count(n => n.IsExpired)},
                {"ByType", activeNotifications.GroupBy(n => n.NotificationType)
                    .ToDictionary(g => g.Key, g => g.Count())}
            };
        }

        /// <summary>
        /// Get notifications grouped by type
        /// </summary>
        /// <param name="notifications">Notifications to group</param>
        /// <param name="activeOnly">Only include active notifications</param>
        /// <returns>Grouped notifications</returns>
        public static Dictionary<string, List<SystemNotification>> GroupByType(IEnumerable<SystemNotification> notifications, bool activeOnly = true)
        {
            var filteredNotifications = activeOnly
                ? notifications.Where(n => n.IsActive)
                : notifications;

            return filteredNotifications
                .GroupBy(n => n.NotificationType)
                .OrderBy(g => g.First().Priority)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(n => n.CreatedDate).ToList());
        }

        /// <summary>
        /// Get priority-sorted notifications for a user
        /// </summary>
        /// <param name="notifications">User's notifications</param>
        /// <param name="limit">Maximum number of notifications to return</param>
        /// <returns>Priority-sorted notifications</returns>
        public static List<SystemNotification> GetPrioritySorted(IEnumerable<SystemNotification> notifications, int limit = 10)
        {
            return notifications
                .Where(n => n.IsActive)
                .OrderBy(n => n.Priority)
                .ThenByDescending(n => n.CreatedDate)
                .Take(limit)
                .ToList();
        }

        /// <summary>
        /// Clean up old notifications
        /// </summary>
        /// <param name="notifications">All notifications</param>
        /// <param name="daysToKeep">Days to keep old notifications</param>
        /// <returns>Notifications that should be deleted</returns>
        public static List<SystemNotification> GetNotificationsToCleanup(IEnumerable<SystemNotification> notifications, int daysToKeep = 90)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            return notifications
                .Where(n => n.CreatedDate < cutoffDate && n.IsRead)
                .ToList();
        }

        /// <summary>
        /// Get notification summary for dashboard
        /// </summary>
        /// <returns>Notification summary data</returns>
        public Dictionary<string, object> GetNotificationSummary()
        {
            return new Dictionary<string, object>
            {
                {"NotificationId", NotificationId},
                {"Title", Title},
                {"Message", MessagePreview},
                {"NotificationType", NotificationType},
                {"Priority", Priority},
                {"PriorityDisplay", PriorityDisplay},
                {"IsRead", IsRead},
                {"IsUrgent", IsUrgent},
                {"IsRecent", IsRecent},
                {"IsExpired", IsExpired},
                {"TimeAgo", TimeAgoDisplay},
                {"TypeColor", TypeColor},
                {"TypeIcon", TypeIcon},
                {"CreatedDate", CreatedDate},
                {"ReadDate", ReadDate},
                {"ExpiryDate", ExpiryDate},
                {"DaysUntilExpiry", DaysUntilExpiry},
                {"AgeCategory", GetAgeCategory()}
            };
        }

        /// <summary>
        /// Validate notification business rules
        /// </summary>
        /// <returns>List of validation errors</returns>
        public List<string> ValidateNotificationRules()
        {
            var errors = new List<string>();

            // Expiry date should be in the future
            if (ExpiryDate.HasValue && ExpiryDate.Value <= CreatedDate)
            {
                errors.Add("Expiry date must be after creation date");
            }

            // Read date should not be before creation date
            if (ReadDate.HasValue && ReadDate.Value < CreatedDate)
            {
                errors.Add("Read date cannot be before creation date");
            }

            // Read status consistency
            if (IsRead && !ReadDate.HasValue)
            {
                errors.Add("Read notifications must have a read date");
            }

            if (!IsRead && ReadDate.HasValue)
            {
                errors.Add("Unread notifications should not have a read date");
            }

            // Valid notification types
            var validTypes = new[] { "Info", "Warning", "Error", "Success", "Alert", "Reminder", "Announcement", "System" };
            if (!validTypes.Contains(NotificationType, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"Invalid notification type: {NotificationType}");
            }

            return errors;
        }

        /// <summary>
        /// Override ToString for better display in dropdowns and logs
        /// </summary>
        /// <returns>String representation of the notification</returns>
        public override string ToString() => DisplayName;
    }
}
