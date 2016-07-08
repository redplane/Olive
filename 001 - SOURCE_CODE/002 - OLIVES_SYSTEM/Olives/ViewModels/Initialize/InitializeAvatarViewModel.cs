using System.ComponentModel.DataAnnotations;
using MultipartDataMediaFormatter.Infrastructure;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeAvatarViewModel
    {
        /// <summary>
        ///     File which is used for being user's avatar.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsRequired")]
        public HttpFile Avatar { get; set; }
    }
}