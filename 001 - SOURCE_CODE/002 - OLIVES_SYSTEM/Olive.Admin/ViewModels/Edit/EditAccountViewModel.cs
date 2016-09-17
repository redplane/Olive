using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Edit
{
    public class EditAccountViewModel
    {
        /// <summary>
        ///     Password of this account.
        /// </summary>
        [MinLength(Values.MinPasswordLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "MinLengthPassword")]
        [MaxLength(Values.MaxPasswordLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "MaxLengthPassword")]
        [RegexMatch(Regexes.Password, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "RegexPassword")]
        public string Password { get; set; }

        /// <summary>
        /// Status of account which can be modified to.
        /// </summary>
        [InEnumerationsArray(new object[] {AccountStatus.Disabled, AccountStatus.Active}, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public AccountStatus? Status { get; set; }
    }
}