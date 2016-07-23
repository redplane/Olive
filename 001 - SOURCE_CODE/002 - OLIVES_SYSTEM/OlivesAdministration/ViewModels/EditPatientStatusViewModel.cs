using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace OlivesAdministration.ViewModels
{
    public class EditPatientStatusViewModel
    {
        [InEnumerationsArray(new object[] { StatusAccount.Active, StatusAccount.Inactive, StatusAccount.Pending },
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public byte Status { get; set; }
    }
}