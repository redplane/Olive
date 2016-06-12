using Shared.Interfaces;
using Shared.Models;

namespace Shared.ViewModels
{
    public class ResponsePersonCode
    {
        public IPerson Person { get; set; }
        
        public ActivationCode Code { get; set; } 
    }
}