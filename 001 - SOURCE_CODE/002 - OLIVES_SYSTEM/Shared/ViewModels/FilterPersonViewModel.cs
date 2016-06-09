using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Models.Nodes;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class FilterPersonViewModel : Person
    {
        /// <summary>
        ///     Time after which account was modified.
        /// </summary>
        public long? MinLastModified { get; set; }

        /// <summary>
        ///     Time before which account had been modified.
        /// </summary>
        public long? MaxLastModified { get; set; }
#pragma warning disable 108, 114
        /// <summary>
        ///     Person first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Person last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Date after that person was born
        /// </summary>
        [CompareLong(Values.MinimumSelectionTime, Comparision = -1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMinBirthday")]
        public long? MinBirthday { get; set; }

        /// <summary>
        ///     Date before which that person had been born
        /// </summary>
        [CompareLong(Values.MinimumSelectionTime, Comparision = -1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMaxBirthday")]
        public long? MaxBirthday { get; set; }

        /// <summary>
        ///     Gender of person
        /// </summary>
        public byte? Gender { get; set; }

        public long? MoneyFrom { get; set; }

        public long? MoneyTo { get; set; }

        [CompareLong(Values.MinimumSelectionTime, Comparision = -1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMinDate")]
        public long? MinCreated { get; set; }

        [CompareLong(Values.MinimumSelectionTime, Comparision = -1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMaxDate")]
        public long? MaxCreated { get; set; }

        [IntCompare(FieldLength.PageIndexMin, Comparision = 1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidPageIndex")]
        public int Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidPageRecords")]
        public int Records { get; set; }

        public byte? Role { get; set; }

#pragma warning restore 108, 114
    }
}