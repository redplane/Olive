namespace Shared.Models.Nodes
{
    public class Appointment
    {
        /// <summary>
        ///     Identity of appointment.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     When the appointment should begin.
        /// </summary>
        public long From { get; set; }

        /// <summary>
        ///     When the appointment should end.
        /// </summary>
        public long To { get; set; }

        /// <summary>
        ///     Note of appointment.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        ///     Time when the appointment was created.
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        ///     Time when the appointment was lastly modified.
        /// </summary>
        public long LastModified { get; set; }

        /// <summary>
        ///     Status of appointment.
        ///     0 : Cancelled.
        ///     1 : Pending.
        ///     2 : Confirmed.
        ///     3 : Active.
        ///     4 : Done.
        /// </summary>
        public byte Status { get; set; }
    }
}