using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize.Personal
{
    public class InitializeBloodSugarViewModel
    {
        /// <summary>
        ///     Value of sugar in blood.
        ///     Unit : mmol/L
        /// </summary>
        [Range(Values.MinSugarBloodMmol, Values.MaxSugarBloodMmol, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double Value { get; set; }

        /// <summary>
        ///     Time when measurement was made.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }

        /// <summary>
        ///     Note of measurement.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}