using Shared.Attributes;
using Shared.Resources;
using Shared.ViewModels.Initialize;

namespace Olives.ViewModels
{
    public class OliveInitializePersonViewModel : InitializePersonViewModel
    {
        /// <summary>
        ///     Role of account.
        /// </summary>
        [InEnumerationsArray(new object[] {Shared.Enumerations.Role.Patient, Shared.Enumerations.Role.Doctor},
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidRole")]
        public int Role { get; set; }
    }
}