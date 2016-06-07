using System.Collections.Generic;
using Shared.Interfaces;

namespace Shared.ViewModels
{
    public class ResponsePersonFilter
    {
        public List<IPerson> Data { get; set; }

        public int Total { get; set; }
    }
}