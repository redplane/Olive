using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseDoctorFilter
    {
        public IList<Doctor> Doctors { get; set; }

        public int Total { get; set; }
    }
}