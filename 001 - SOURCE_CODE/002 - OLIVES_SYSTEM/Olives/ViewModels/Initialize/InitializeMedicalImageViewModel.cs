﻿using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeMedicalImageViewModel
    {
        /// <summary>
        ///     Id of medical record.
        /// </summary>
        public int MedicalRecord { get; set; }

        /// <summary>
        ///     Image file.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsRequired")]
        public Image File { get; set; }
    }
}