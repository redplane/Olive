using System.Collections.Generic;

namespace Shared.ViewModels
{
    public class ResponsePatientFilter
    {
        public IList<PatientViewModel> Users { get; set; }

        public int Total { get; set; }
    }
}