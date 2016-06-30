using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace OlivesAdministration.ViewModels
{
    public class EditPatientStatusViewModel : FindPatientViewModel
    {
        [InAccountStatus(new[] { Shared.Enumerations.StatusAccount.Inactive, Shared.Enumerations.StatusAccount.Active},
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidAccountStatus")]
        public byte Status { get; set; }
    }
}