using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterAppointmentViewModel : IPagination
    {
        [InEnumerationsArray(new object[] { PartnerFilterMode.ParterIsDater, PartnerFilterMode.PartnerIsRequester }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public PartnerFilterMode Mode { get; set; }

        public int Partner { get; set; }

        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinCreated { get; set; }

        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxCreated { get; set; }

        [NumericPropertyCompare("MaxFrom", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinFrom { get; set; }

        [NumericPropertyCompare("MinFrom", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxFrom { get; set; }

        [NumericPropertyCompare("MaxTo", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinTo { get; set; }

        [NumericPropertyCompare("MinTo", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxTo { get; set; }

        [InEnumerationsArray(new object[] { StatusAppointment.Pending, StatusAppointment.Active, StatusAppointment.Cancelled, StatusAppointment.Done }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public StatusAppointment? Status { get; set; }

        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public double? MinLastModified { get; set; }

        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public double? MaxLastModified { get; set; }

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPageIndex")]
        public int Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPageRecords")]
        public int Records { get; set; } = FieldLength.RecordMax;
    }
}