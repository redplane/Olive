using System.ComponentModel.DataAnnotations;
using Olives.Constants;
using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Edit
{
    public class EditDiaryViewModel
    {
        /// <summary>
        /// Time which diary is noted about.
        /// </summary>
        [EpochTimeCompare(Shared.Constants.Values.MinimumAllowedYear, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? Time { get; set; }

        /// <summary>
        /// Content of diary note.
        /// </summary>
        [StringLength(OlivesValues.MaxDiaryLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; } 
    }
}