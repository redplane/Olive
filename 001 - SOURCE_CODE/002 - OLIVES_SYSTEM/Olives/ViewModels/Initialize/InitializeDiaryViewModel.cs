﻿using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;
using OlivesValues = Olives.Constants.OlivesValues;

namespace Olives.ViewModels.Initialize
{
    public class InitializeDiaryViewModel
    {
        /// <summary>
        /// Time when the diary is noted.
        /// </summary>
        [EpochTimeCompare(Shared.Constants.Values.MinimumAllowedYear, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }

        /// <summary>
        /// Note of diary.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(OlivesValues.MaxDiaryLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}