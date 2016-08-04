using Olives.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeRelationshipViewModel
    {
        [AccountValidate(AccountValidateInputType.Id, true, Role.Doctor, StatusAccount.Active,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int Target { get; set; }
    }
}