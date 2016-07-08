﻿using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializeAllergyViewModel
    {
        /// <summary>
        /// Name of allergy.
        /// </summary>
        [StringLength(32, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        /// <summary>
        /// Cause of allergy.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Cause { get; set; }

        /// <summary>
        /// Note of allergy.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}