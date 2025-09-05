using System.ComponentModel.DataAnnotations;

namespace KMSI_Projects.Models.ViewModels
{
    public class ErrorViewModel
    {
        /// <summary>
        /// Request ID for tracking the error
        /// </summary>
        [Display(Name = "Request ID")]
        public string? RequestId { get; set; }

        /// <summary>
        /// Whether to show the Request ID
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// Error message to display
        /// </summary>
        [Display(Name = "Error Message")]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Error title/heading
        /// </summary>
        [Display(Name = "Error Title")]
        public string ErrorTitle { get; set; } = "An Error Occurred";

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        [Display(Name = "Status Code")]
        public int? StatusCode { get; set; }

        /// <summary>
        /// Whether this is a development environment
        /// </summary>
        public bool IsDevelopment { get; set; } = false;

        /// <summary>
        /// Exception details (only for development)
        /// </summary>
        [Display(Name = "Exception Details")]
        public string? ExceptionDetails { get; set; }

        /// <summary>
        /// Stack trace (only for development)
        /// </summary>
        [Display(Name = "Stack Trace")]
        public string? StackTrace { get; set; }

        /// <summary>
        /// Suggested action for the user
        /// </summary>
        [Display(Name = "Suggested Action")]
        public string SuggestedAction { get; set; } = "Please try again later or contact support if the problem persists.";

        /// <summary>
        /// URL to return to
        /// </summary>
        [Display(Name = "Return URL")]
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Contact email for support
        /// </summary>
        [Display(Name = "Support Email")]
        public string SupportEmail { get; set; } = "support@kawaimusic.id";

        /// <summary>
        /// Whether to show the back button
        /// </summary>
        public bool ShowBackButton { get; set; } = true;

        /// <summary>
        /// Whether to show the home button
        /// </summary>
        public bool ShowHomeButton { get; set; } = true;
    }
}
