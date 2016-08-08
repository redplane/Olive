using System.ComponentModel.DataAnnotations;
using MultipartFormDataMediaFormatter.Attributes;
using MultipartFormDataMediaFormatter.Models;
using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializePrescriptionImageViewModel
    {
        /// <summary>
        ///     Id of prescription which images should belong to.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public int Prescription { get; set; }

        /// <summary>
        ///     List of image which should be uploaded to server.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsRequired")]
        [HttpFileImageValidate(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsInvalid"
            )]
        public HttpFileModel Image { get; set; }
    }
}