namespace Shared.ViewModels
{
    public class PatientViewModel : PersonViewModel
    {
        /// <summary>
        /// How much money patient has.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// Patient's weight.
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Patient height.
        /// </summary>
        public double? Height { get; set; }
    }
}