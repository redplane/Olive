using System.Collections.Generic;

namespace Shared.ViewModels.Response
{
    public class ResponseDoctorFilter
    {
        public IList<DoctorViewModel> Users { get; set; }

        public int Total { get; set; }
    }
}