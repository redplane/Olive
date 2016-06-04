using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace DotnetSignalR.ViewModels
{
    public class CreateDoctorViewModel : Shared.ViewModels.CreateDoctorViewModel
    {
        public double Money { get; set; }

        [Range(AccountStatus.Inactive, AccountStatus.Active, ErrorMessage = ErrorCodes.InvalidAccountStatus)]
        public byte Status { get; set; }
    }
}