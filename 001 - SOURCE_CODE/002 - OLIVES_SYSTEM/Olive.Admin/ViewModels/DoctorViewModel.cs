namespace OliveAdmin.ViewModels
{
    public class DoctorViewModel : PersonViewModel
    {
        /// <summary>
        /// Rank of doctor.
        /// </summary>
        public double Rank { get; set; }

        /// <summary>
        /// Specialty information of doctor.
        /// </summary>
        public SpecialtyViewModel Specialty { get; set; }

        /// <summary>
        /// Number of people who voted for this doctor.
        /// </summary>
        public int Voters { get; set; }
        
        /// <summary>
        /// Place information where doctor works or lives.
        /// </summary>
        public PlaceViewModel Place { get; set; }

        /// <summary>
        /// Url where doctor saves profile.
        /// </summary>
        public string ProfileUrl { get; set; }
    }
}