using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize.Medical
{
    public class InitializePrescriptedMedicineViewModel
    {
        /// <summary>
        ///     Id of prescription which prescripted medicine should belong to.
        /// </summary>
        public int Prescription { get; set; }

        /// <summary>
        ///     Name of medicine.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Medicine { get; set; }

        [NumericCompare(0, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeGreaterThan")]
        public double Quantity { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(16, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Unit { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}