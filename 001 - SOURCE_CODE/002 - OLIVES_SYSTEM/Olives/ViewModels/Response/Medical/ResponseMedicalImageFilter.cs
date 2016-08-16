using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response.Medical
{
    public class ResponseMedicalImageFilter
    {
        public IList<MedicalImage> MedicalImages { get; set; }

        public int Total { get; set; }
    }
}