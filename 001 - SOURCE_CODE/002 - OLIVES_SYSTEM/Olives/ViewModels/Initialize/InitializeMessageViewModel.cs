using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeMessageViewModel
    {
        /// <summary>
        /// Who should receive the message.
        /// </summary>
        public int Recipient { get; set; }

        /// <summary>
        /// Content of message.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(Values.MaxMessageContentLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Content { get; set; }
    }
}