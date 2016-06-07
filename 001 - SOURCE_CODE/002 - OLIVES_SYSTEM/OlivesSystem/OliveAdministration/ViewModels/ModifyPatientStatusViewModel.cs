using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace OliveAdministration.ViewModels
{
    public class ModifyPatientStatusViewModel : FindPatientViewModel
    {
        [BytesMatch(new[] {AccountStatus.Inactive, AccountStatus.Active}, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidAccountStatus")]
        public byte Status { get; set; }
    }
}