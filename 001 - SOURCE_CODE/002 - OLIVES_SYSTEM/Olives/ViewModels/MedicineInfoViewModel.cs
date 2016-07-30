using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Olives.ViewModels
{
    public class MedicineInfoViewModel
    {
        public double Quantity { get; set; }

        [ValueStringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Unit { get; set; }

        [ValueStringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}