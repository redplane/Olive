﻿using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Olives.ViewModels.Edit.Personal
{
    public class EditAddictionViewModel
    {
        /// <summary>
        ///     What patient is addictive with.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Cause { get; set; }

        /// <summary>
        ///     Note about the addiction.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}