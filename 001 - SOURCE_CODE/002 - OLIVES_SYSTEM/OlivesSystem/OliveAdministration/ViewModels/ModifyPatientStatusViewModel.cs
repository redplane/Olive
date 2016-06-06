using Shared.Attributes;
using Shared.Constants;

namespace DotnetSignalR.ViewModels
{
    public class ModifyPatientStatusViewModel : FindPatientViewModel
    {
        [BytesMatch(new[] { AccountStatus.Inactive, AccountStatus.Active }, ErrorMessage = ErrorCodes.InvalidPatientStatus)]
        public byte Status { get; set; }
    }
}