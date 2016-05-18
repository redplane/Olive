using System.ComponentModel.DataAnnotations;

namespace ApiLayer.ViewModel
{
    public class MessageSendViewModel
    {
        /// <summary>
        /// Sender name
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Sender { get; set; }
        
        /// <summary>
        /// Recipient name
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Recipient { get; set; }
        
        /// <summary>
        /// Message content.
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string Content { get; set; }
    }
}