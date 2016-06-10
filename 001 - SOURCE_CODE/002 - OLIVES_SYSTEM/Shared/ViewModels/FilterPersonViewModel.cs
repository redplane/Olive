﻿using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class FilterPersonViewModel
    {
        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        [RegularExpression(Regexes.Email, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        public string Email { get; set; }
        
        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPhone")]
        public string Phone { get; set; }
        
        /// <summary>
        ///     Time after which account was modified.
        /// </summary>
        public long? MinLastModified { get; set; }

        /// <summary>
        ///     Time before which account had been modified.
        /// </summary>
        public long? MaxLastModified { get; set; }

        /// <summary>
        ///     Person first name.
        /// </summary>
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidLastName")]
        public string FirstName { get; set; }
        
        /// <summary>
        ///     Person last name.
        /// </summary>
        [MaxLength(FieldLength.FirstNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidFirstName")]
        public string LastName { get; set; }

        /// <summary>
        ///     Date after that person was born
        /// </summary>
        [TickToYearCompare(Values.MinimumAllowedYear, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidMinBirthday")]
        public long? MinBirthday { get; set; }

        /// <summary>
        ///     Date before which that person had been born
        /// </summary>
        [TickToYearCompare(Values.MinimumAllowedYear, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidMaxBirthday")]
        public long? MaxBirthday { get; set; }

        /// <summary>
        ///     Gender of person
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// Amount of money user's must be higher than.
        /// </summary>
        public double? MinMoney { get; set; }

        /// <summary>
        /// Amount of money user's must be lower than.
        /// </summary>
        public double? MaxMoney { get; set; }

        [CompareLong(Values.MinimumSelectionTime, Comparision = -1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMinDate")]
        public long? MinCreated { get; set; }

        [CompareLong(Values.MinimumSelectionTime, Comparision = -1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidMaxDate")]
        public long? MaxCreated { get; set; }

        /// <summary>
        ///     Status of account [0 - Disabled | 1 - Pending | 2 - Active].
        /// </summary>
        public byte Status { get; set; }
        
        [Range(Constants.Gender.Male, Constants.Gender.Female, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidGender")]
        public byte? Role { get; set; }

        [IntCompare(FieldLength.PageIndexMin, Comparision = 1, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPageIndex")]
        public int Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPageRecords")]
        public int Records { get; set; }
    }
}