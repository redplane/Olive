using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;
using Shared.ViewModels;
using Shared.ViewModels.Initialize;

namespace Olives.ViewModels
{
    public class OliveInitializePersonViewModel : InitializePersonViewModel
    {
        /// <summary>
        ///     Role of account.
        /// </summary>
        [InNumericArray(new[] {AccountRole.Patient, AccountRole.Doctor}, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidRole")]
        public int Role { get; set; }
    }
}