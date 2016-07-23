using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels
{
    public class EditPatientStatusViewModel
    {
        public int Id { get; set; }

        [InEnumerationsArray(new object[] {StatusAccount.Inactive, StatusAccount.Active},
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidAccountStatus")]
        public byte Status { get; set; }
    }
}