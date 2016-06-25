using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class SugarbloodViewModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Value of sugar in blood.
        /// Unit : mmol/L
        /// </summary>
        [Range(Values.MinSugarBloodMmol, Values.MaxSugarBloodMmol, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double Value { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }

        public double Created { get; set; }

        public double? LastModified { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        public string Note { get; set; }
    }
}