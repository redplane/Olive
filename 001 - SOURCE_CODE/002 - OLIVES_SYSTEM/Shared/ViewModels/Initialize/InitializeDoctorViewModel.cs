using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializeDoctorViewModel : InitializePersonViewModel
    {
        /// <summary>
        /// Doctor specialty.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int Specialty { get; set; }

        /// <summary>
        /// Id of city where doctor lives.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int City { get; set; }
        
        /// <summary>
        /// Rank of doctor.
        /// </summary>
        public double Rank { get; set; }

        /// <summary>
        /// Number of people voted for this doctor.
        /// </summary>
        public int Voters { get; set; }

        /// <summary>
        /// Money in doctor account.
        /// </summary>
        public int Money { get; set; }

        #region Constructor

        /// <summary>
        /// Initialize an instance of InitializeDoctorViewModel with default information.
        /// </summary>
        public InitializeDoctorViewModel()
        {
            Role = Role.Doctor;
        }

        #endregion
    }
}