using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels
{
    public class EditPatientStatusViewModel : FindPatientViewModel
    {
        [InNumericArray(new[] {(int)AccountStatus.Inactive, (int)AccountStatus.Active},
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidAccountStatus")]
        public byte Status { get; set; }
    }
}