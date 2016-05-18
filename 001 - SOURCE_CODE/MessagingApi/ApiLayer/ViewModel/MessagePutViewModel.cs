using System.ComponentModel.DataAnnotations;

namespace ApiLayer.ViewModel
{
    public class MessagePutViewModel
    {
        [Required]
        public int Id { get; set; } 
    }
}