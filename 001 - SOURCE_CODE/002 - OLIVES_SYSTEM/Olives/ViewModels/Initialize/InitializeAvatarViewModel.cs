using System.ComponentModel.DataAnnotations;
using MultipartFormDataMediaFormatter.Attributes;
using MultipartFormDataMediaFormatter.Models;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeAvatarViewModel
    {
        /// <summary>
        ///     File which is used for being user's avatar.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsRequired")]
        [HttpFileImageValidate(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsInvalid"
            )]
        public HttpFileModel Avatar { get; set; }
    }
}