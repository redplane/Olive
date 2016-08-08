using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseMessageFilter
    {
        public IEnumerable<Message> Messages { get; set; }

        public int Total { get; set; }
    }
}