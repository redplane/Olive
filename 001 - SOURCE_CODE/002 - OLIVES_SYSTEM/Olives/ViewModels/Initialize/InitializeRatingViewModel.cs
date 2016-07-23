using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeRatingViewModel
    {
        /// <summary>
        ///     Target of rating.
        /// </summary>
        public int? Target { get; set; }

        /// <summary>
        ///     Email of rated person.
        /// </summary>
        [RequiredIf("Target", null, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "RequireEmail")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        [RegularExpression(Regexes.Email, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        public string Email { get; set; }

        [Range(1, 5, ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Rate { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Comment { get; set; }
    }
}