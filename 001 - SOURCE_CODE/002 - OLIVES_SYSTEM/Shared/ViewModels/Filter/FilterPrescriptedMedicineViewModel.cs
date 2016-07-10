using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterPrescriptedMedicineViewModel : IPagination
    {
        /// <summary>
        /// Id of prescription
        /// </summary>
        public int? Prescription { get; set; }

        /// <summary>
        /// Owner of prescripted medicine.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        /// Name of medicine.
        /// </summary>
        [StringLength(32, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Medicine { get; set; }

        /// <summary>
        /// Minium quantity.
        /// </summary>
        [NumericCompare(0, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeGreaterThan")]
        [NumericPropertyCompare("MaxQuantity", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MinQuantity { get; set; }

        /// <summary>
        /// Maximum quantity.
        /// </summary>
        [NumericCompare(0, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeGreaterThan")]
        [NumericPropertyCompare("MinQuantity", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxQuantity { get; set; }

        /// <summary>
        /// Time after which medicine should be expired.
        /// </summary>
        [NumericCompare(0, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeGreaterThan")]
        [NumericPropertyCompare("MaxExpired", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MinExpired { get; set; }

        /// <summary>
        /// Time before which medicine should have been expired.
        /// </summary>
        [NumericCompare(0, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeGreaterThan")]
        [NumericPropertyCompare("MinExpired", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxExpired { get; set; }

        /// <summary>
        /// Unit of medicine.
        /// </summary>
        [StringLength(16, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Unit { get; set; }

        /// <summary>
        /// Whether record should be sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] {SortDirection.Ascending, SortDirection.Decending}, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        /// <summary>
        /// What property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(new object[] { PrescriptedMedicineSort.Expired, PrescriptedMedicineSort.MedicineName, PrescriptedMedicineSort.Quantity }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public PrescriptedMedicineSort Sort { get; set; } = PrescriptedMedicineSort.Expired;

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Records { get; set; } = FieldLength.RecordMax;
    }
}