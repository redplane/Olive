using System.ComponentModel.DataAnnotations;
using Olives.Attributes;
using Olives.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeRelationshipRequestViewModel
    {
        /// <summary>
        /// Target must be a doctor and active.
        /// </summary>
        [AccountValidate(AccountValidateInputType.Id, true, Role.Doctor, StatusAccount.Active,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int Target { get; set; }

        /// <summary>
        /// Content of relationship request
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(OlivesValues.MaxRelationshipRequestContentLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Content { get; set; }
    }
}