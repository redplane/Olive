namespace Shared.ViewModels.Initialize
{
    public class InitializeAppointmentViewModel
    {
        /// <summary>
        /// Account of dater.
        /// </summary>
        public int Dater { get; set; }
        
        /// <summary>
        /// When the appointment should start.
        /// </summary>
        public double From { get; set; }
        
        /// <summary>
        /// When the appointment should be ended.
        /// </summary>
        public double To { get; set; } 

        /// <summary>
        /// Appointment note.
        /// </summary>
        public string Note { get; set; }
    }
}