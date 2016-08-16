using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response
{
    public class ResponseMessageFilter
    {
        public IEnumerable<Message> Messages { get; set; }

        public int Total { get; set; }
    }
}