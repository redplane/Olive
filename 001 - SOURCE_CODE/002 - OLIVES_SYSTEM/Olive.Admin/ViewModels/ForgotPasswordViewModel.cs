using System.ComponentModel.DataAnnotations;
using Shared.Resources;

namespace OliveAdmin.ViewModels
{
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Email which the validation url should be sent to.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(ValidationMessage), ErrorMessageResourceName = "EmailIsRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessage), ErrorMessageResourceName = "EmailFormatIsInvalid")]
        [StringLength(Shared.Models.Constant.MaximumLengthEmail, ErrorMessageResourceType = typeof(ValidationMessage), ErrorMessageResourceName = "EmailMaxLengthExceeded")]
        public string Email { get; set; }
    }
}