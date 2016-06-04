using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace DotnetSignalR.ViewModels
{
    public class EditPersonViewModel : Shared.ViewModels.EditPersonViewModel
    {
        public double Money { get; set; }
        
        [Range(AccountStatus.Inactive, AccountStatus.Active, ErrorMessage = ErrorCodes.InvalidAccountStatus)]
        public byte Status { get; set; }
    }
}