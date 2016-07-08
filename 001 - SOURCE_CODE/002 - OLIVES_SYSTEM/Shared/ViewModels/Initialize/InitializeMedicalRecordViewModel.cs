using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializeMedicalRecordViewModel
    {
        /// <summary>
        /// Owner of medical record
        /// As no owner is defined, the requester will be the owner of medical record.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        /// Summary of medical record.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Summary { get; set; }

        /// <summary>
        /// Neccessary tests which are needed doing.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Tests { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string AdditionalMorbidities { get; set; }

        /// <summary>
        /// Another diagnosis
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string DifferentialDiagnosis { get; set; }

        /// <summary>
        /// Other pathologies which patient is currently having.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string OtherPathologies { get; set; }

        /// <summary>
        /// Time when the record is created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }
    }
}