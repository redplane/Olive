using System;
using System.ComponentModel.DataAnnotations;

namespace ApiLayer.Models
{
    public class PrivateMessage
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Recipient { get; set; }

        [Required]
        public long Sent { get; set; }

        [Required]
        public bool IsReceived { get; set; } 
    }
}