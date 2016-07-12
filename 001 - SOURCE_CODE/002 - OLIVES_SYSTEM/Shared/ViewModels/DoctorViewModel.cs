namespace Shared.ViewModels
{
    public class DoctorViewModel : PersonViewModel
    {
        /// <summary>
        ///     Money of doctor.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        ///     Rank of doctor.
        /// </summary>
        public double Rank { get; set; }

        /// <summary>
        ///     Specialty of doctor.
        /// </summary>
        public SpecialtyViewModel Specialty { get; set; }

        /// <summary>
        ///     Number of people voted for this doctor.
        /// </summary>
        public int Voters { get; set; }

        /// <summary>
        ///     City where doctor lives in.
        /// </summary>
        public CityViewModel City { get; set; }
    }
}