using Shared.Enumerations;

namespace Shared.Models.Edges
{
    public class HasAppointment
    {
        /// <summary>
        /// Time when the appointment should be started.
        /// </summary>
        public double Start { get; set; }

        /// <summary>
        /// Time when the appointment should be ended.
        /// </summary>
        public double Expire { get; set; }

        /// <summary>
        /// Message of appointment.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Status of appointment.
        /// </summary>
        public StatusAppointment Status { get; set; }

        /// <summary>
        /// When the appointment was created.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// When the appointment was lastly modified.
        /// </summary>
        public double? LastModified { get; set; }
    }
}