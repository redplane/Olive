using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseMedicalRecordFilter
    {
        public IList<MedicalRecord> MedicalRecords { get; set; }
        
        public int Total { get; set; } 
    }
}