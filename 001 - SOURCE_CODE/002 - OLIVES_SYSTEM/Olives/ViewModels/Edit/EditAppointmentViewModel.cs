using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Edit
{
    public class EditAppointmentViewModel
    {
        /// <summary>
        ///     When the appointment should start.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? From { get; set; }

        /// <summary>
        ///     When the appointment should be ended.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? To { get; set; }

        /// <summary>
        ///     Appointment note.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }

        /// <summary>
        ///     Status of appointment.
        /// </summary>
        [InEnumerationsArray(
            new object[] {StatusAppointment.Cancelled, StatusAppointment.Done, StatusAppointment.Active})]
        public StatusAppointment? Status { get; set; }
    }
}