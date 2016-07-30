using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;
using Shared.ViewModels.Initialize;

namespace OlivesAdministration.ViewModels.Initialize
{
    public class InitializePatientViewModel : InitializePersonViewModel
    {
        public double Money { get; set; }

        /// <summary>
        ///     Patient weight.
        /// </summary>
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidHeight")]
        public float? Weight { get; set; }

        /// <summary>
        ///     Patient height.
        /// </summary>
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidWeight")]
        public float? Height { get; set; }

        /// <summary>
        ///     Patient anamneses.
        /// </summary>
        public string[] Anamneses { get; set; }
    }
}