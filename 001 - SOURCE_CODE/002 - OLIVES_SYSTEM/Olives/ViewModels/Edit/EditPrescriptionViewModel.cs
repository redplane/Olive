﻿using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Edit
{
    public class EditPrescriptionViewModel
    {
        [NumericPropertyCompare("To", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? From { get; set; }

        [NumericPropertyCompare("From", Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? To { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}