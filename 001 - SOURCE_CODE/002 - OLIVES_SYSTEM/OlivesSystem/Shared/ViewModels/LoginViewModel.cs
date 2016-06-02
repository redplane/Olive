using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace Shared.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        ///     Email which was used for account registration.
        /// </summary>
        [Required(ErrorMessage = ErrorCodes.EmailIsRequired)]
        [RegularExpression(Regexes.Email, ErrorMessage = ErrorCodes.InvalidEmail)]
        [MaxLength(Constants.Constants.EmailMaxLength, ErrorMessage = ErrorCodes.InvalidEmailLength)]
        public string Email { get; set; }

        /// <summary>
        ///     Password of account.
        /// </summary>
        [Required(ErrorMessage = ErrorCodes.PasswordIsRequired)]
        [RegularExpression(Regexes.Password, ErrorMessage = ErrorCodes.InvalidPasswordFormat)]
        [MaxLength(Constants.Constants.PasswordMaxLength, ErrorMessage = ErrorCodes.InvalidPasswordLength)]
        public string Password { get; set; }
    }
}