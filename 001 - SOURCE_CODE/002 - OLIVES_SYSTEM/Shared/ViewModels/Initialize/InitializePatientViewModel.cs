using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializePatientViewModel : InitializePersonViewModel
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of InitializePatientViewModel with default information.
        /// </summary>
        public InitializePatientViewModel()
        {
            Role = Role.Patient;
        }

        #endregion

        /// <summary>
        ///     Money patient has.
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        ///     Weight of patient.
        /// </summary>
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double? Weight { get; set; }

        /// <summary>
        ///     Height of patient.
        /// </summary>
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double? Height { get; set; }
    }
}