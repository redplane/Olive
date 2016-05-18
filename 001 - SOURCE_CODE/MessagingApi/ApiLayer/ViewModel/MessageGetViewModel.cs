using System;
using System.ComponentModel.DataAnnotations;

namespace ApiLayer.ViewModel
{
    public class MessageGetViewModel
    {
        /// <summary>
        /// Sender name.
        /// </summary>
        [Required(ErrorMessage = "Sender name is required.")]
        [StringLength(32, ErrorMessage = "Sender name cannot be more than 32 characters")]
        public string Sender { get; set; }
        
        /// <summary>
        /// Recipient name.
        /// </summary>
        [Required(ErrorMessage = "Recipient name is required.")]
        [StringLength(32, ErrorMessage = "Recipient name cannot be more than 32 characters")]
        public string Recipient { get; set; }
        
        public long? From { get; set; }
        
        public long? To { get; set; }

        /// <summary>
        /// Retrieved message are received or not.
        /// </summary>
        public bool IsReceived { get; set; }

        /// <summary>
        /// Whether time is ordered or not
        /// If so, what direction should be ordered.
        /// By default, no ordering.
        /// </summary>
        [Range(0, 2, ErrorMessage = "Order must be from 0 to 2")]
        public int Order { get; set; } = 0;

        /// <summary>
        /// How many records will be returned to client.
        /// </summary>
        [Range(5, 20)]
        public int Records { get; set; } = 10;
    }
}