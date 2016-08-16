using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response.Medical
{
    public class ResponseMedicalRecordFilter
    {
        public IList<MedicalRecord> MedicalRecords { get; set; }

        public int Total { get; set; }
    }
}