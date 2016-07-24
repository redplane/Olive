using System.ComponentModel.DataAnnotations;
using System.Drawing;
using MultipartFormDataMediaFormatter.Models;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializePrescriptionImageViewModel
    {
        /// <summary>
        /// Id of prescription which images should belong to.
        /// </summary>
        [MinLength(1, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public int Prescription { get; set; }

        /// <summary>
        /// List of image which should be uploaded to server.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        public Image Image { get; set; }
    }
}