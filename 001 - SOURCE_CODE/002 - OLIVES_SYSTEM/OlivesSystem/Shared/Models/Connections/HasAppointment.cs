namespace Shared.Models.Connections
{
    public class HasAppointment
    {
        /// <summary>
        /// Appointment Id.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Content of appointment.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Time which doctor and patient need to meet each other.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Time after that appointment is invalid.
        /// </summary>
        public long DueDate { get; set; }
    }
}