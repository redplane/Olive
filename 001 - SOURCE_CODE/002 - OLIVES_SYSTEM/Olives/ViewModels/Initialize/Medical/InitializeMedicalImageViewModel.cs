using System.ComponentModel.DataAnnotations;
using MultipartFormDataMediaFormatter.Attributes;
using MultipartFormDataMediaFormatter.Models;
using Shared.Resources;

namespace Olives.ViewModels.Initialize.Medical
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
        [HttpFileImageValidate(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsInvalid"
            )]
        public HttpFileModel File { get; set; }
    }
}