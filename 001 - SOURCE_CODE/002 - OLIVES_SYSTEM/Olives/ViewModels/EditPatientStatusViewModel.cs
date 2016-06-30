using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels
{
    public class EditPatientStatusViewModel : FindPatientViewModel
    {
        [InAccountStatus(new[] {StatusAccount.Inactive, StatusAccount.Active},
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidAccountStatus")]
        public byte Status { get; set; }
    }
}