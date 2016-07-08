using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseMedicalImageFilter
    {
        public IList<MedicalImage> MedicalImages { get; set; }
        
        public int Total { get; set; } 
    }
}