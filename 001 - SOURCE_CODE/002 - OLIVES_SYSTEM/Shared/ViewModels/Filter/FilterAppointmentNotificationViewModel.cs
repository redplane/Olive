using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterAppointmentNotificationViewModel : IPagination
    {
        /// <summary>
        /// Id of appointment notification.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Who sent the filter request.
        /// </summary>
        public int Requester { get; set; }

        /// <summary>
        /// Who is included in medical record.
        /// </summary>
        public int? Partner { get; set; }

        /// <summary>
        /// Filtering mode.
        /// </summary>
        public RecordFilterMode? Mode { get; set; }

        /// <summary>
        /// Id of appointment.
        /// </summary>
        public int? Appointment { get; set; }

        /// <summary>
        /// Whether appointment notification has been seen or not.
        /// </summary>
        public bool? IsSeen { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        public float? MinCreated { get; set; }

        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public float? MaxCreated { get; set; }

        /// <summary>
        ///     Which property should be used for sorting.
        /// </summary>
        [InEnumerationsArray(new object[] { AppointmentNotificationFilterSort.Appointment, AppointmentNotificationFilterSort.Created, AppointmentNotificationFilterSort.Seen, AppointmentNotificationFilterSort.Type }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public AppointmentNotificationFilterSort Sort { get; set; } = AppointmentNotificationFilterSort.Created;

        /// <summary>
        ///     Whether record should be sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] { SortDirection.Ascending, SortDirection.Decending }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        /// <summary>
        /// Index of page.
        /// </summary>
        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = 0;

        /// <summary>
        /// Record number per page.
        /// </summary>
        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}