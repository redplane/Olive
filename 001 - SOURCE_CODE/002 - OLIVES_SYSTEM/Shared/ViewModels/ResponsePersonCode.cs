using Shared.Models;
using Shared.Models.Nodes;

namespace Shared.ViewModels
{
    public class ResponsePersonCode
    {
        public Person Person { get; set; }

        public ActivationCode Code { get; set; }
    }
}