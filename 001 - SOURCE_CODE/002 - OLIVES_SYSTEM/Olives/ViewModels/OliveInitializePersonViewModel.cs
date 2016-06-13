﻿using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;
using Shared.ViewModels;

namespace Olives.ViewModels
{
    public class OliveInitializePersonViewModel : InitializePersonViewModel
    {
        /// <summary>
        /// Role of account.
        /// </summary>
        [InNumericArray(new [] {Roles.Patient, Roles.Doctor}, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidRole")]
        public byte Role { get; set; }
    }
}