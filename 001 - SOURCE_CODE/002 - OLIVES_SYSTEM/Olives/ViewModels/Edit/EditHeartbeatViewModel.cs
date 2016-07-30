using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Edit
{
    public class EditHeartbeatViewModel
    {
        /// <summary>
        ///     Heart rate.
        /// </summary>
        [Range(Values.MinHeartRate, Values.MaxHeartRate, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double? Rate { get; set; }

        /// <summary>
        ///     Time when measurement was done.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? Time { get; set; }

        /// <summary>
        ///     Note of measurement.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}