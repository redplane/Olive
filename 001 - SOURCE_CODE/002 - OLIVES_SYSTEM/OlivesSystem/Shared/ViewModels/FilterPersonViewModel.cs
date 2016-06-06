using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Models;

namespace Shared.ViewModels
{
    public class FilterPersonViewModel
    {
        /// <summary>
        /// Id of person
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Person first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Person last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Date after that person was born
        /// </summary>
        public long? BirthdayFrom { get; set; }

        /// <summary>
        /// Date before which that person had been born
        /// </summary>
        public long? BirthdayTo { get; set; }

        /// <summary>
        /// Gender of person
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// Email of person
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone number of person
        /// </summary>
        public string Phone { get; set; }
        
        public long? MoneyFrom { get; set; }

        public long? MoneyTo { get; set; }

        [CompareLong(Times.MinimumSelectionTime, Comparision = -1, ErrorMessageEqualSmaller = ErrorCodes.InvalidBirthday)]
        public long? CreatedFrom { get; set; }

        [CompareLong(Times.MinimumSelectionTime, Comparision = -1, ErrorMessageEqualSmaller = ErrorCodes.InvalidBirthday)]
        public long? CreatedTo { get; set; }
        
        [IntCompare(FieldLength.PageIndexMin, Comparision = 1, ErrorMessageEqualHigher = ErrorCodes.InvalidPageIndex)]
        public int? Page { get; set; }

        [Range(FieldLength.RecordMin, FieldLength.RecordMax, ErrorMessage = ErrorCodes.InvalidPageRecords)]
        public int? Records { get; set; }

        public byte Role { get; set; }
    }
}