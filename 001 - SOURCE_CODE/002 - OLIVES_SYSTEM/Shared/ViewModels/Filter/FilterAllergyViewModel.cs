﻿using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Enumerations.Filter;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.ViewModels.Filter
{
    public class FilterAllergyViewModel : IPagination
    {
        [NumericCompare(1, ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int? Owner { get; set; }

        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Cause { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }

        [NumericPropertyCompare("MaxCreated", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MinCreated { get; set; }

        [NumericPropertyCompare("MinCreated", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MaxCreated { get; set; }

        [NumericPropertyCompare("MaxLastModified", Comparision = Comparision.LowerEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MinLastModified { get; set; }

        [NumericPropertyCompare("MinLastModified", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? MaxLastModified { get; set; }

        /// <summary>
        ///     Which property should be used for sorting
        /// </summary>
        [InEnumerationsArray(new object[] {NoteResultSort.LastModified, NoteResultSort.Created},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public NoteResultSort Sort { get; set; } = NoteResultSort.LastModified;

        /// <summary>
        ///     Whether the record should be sorted ascendingly or decendingly.
        /// </summary>
        [InEnumerationsArray(new object[] {SortDirection.Ascending, SortDirection.Decending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public SortDirection Direction { get; set; } = SortDirection.Decending;

        [NumericCompare(FieldLength.PageIndexMin, Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Page { get; set; } = FieldLength.PageIndexMin;

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int? Records { get; set; }
    }
}