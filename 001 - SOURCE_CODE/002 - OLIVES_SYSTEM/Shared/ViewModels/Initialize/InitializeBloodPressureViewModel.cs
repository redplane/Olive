using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializeBloodPressureViewModel
    {
        /// <summary>
        /// Minimum pressure of blood.
        /// </summary>
        [Range(Values.MinDiastolic, Values.MaxDiastolic, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Diastolic { get; set; }

        /// <summary>
        /// Maximum pressure of blood.
        /// </summary>
        [Range(Values.MinSystolic, Values.MaxSystolic, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Systolic { get; set; }

        /// <summary>
        /// Time when measurement was done.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }

        /// <summary>
        /// Note of measurement.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}