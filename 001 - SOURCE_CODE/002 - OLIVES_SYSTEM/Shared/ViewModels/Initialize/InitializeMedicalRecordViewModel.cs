﻿using System.Collections.Generic;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializeMedicalRecordViewModel
    {
        /// <summary>
        ///     Owner of medical record
        ///     As no owner is defined, the requester will be the owner of medical record.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        ///     List of noticed information.
        /// </summary>
        [DictionaryLength(FieldLength.MaxDictionaryLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainKey")]
        [DictionaryKeyValueLength(FieldLength.MaxDictionaryKeyLength, FieldLength.MaxDictionaryValueLength,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public Dictionary<string, string> Infos { get; set; }

        /// <summary>
        ///     Time when the record is created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }
    }
}