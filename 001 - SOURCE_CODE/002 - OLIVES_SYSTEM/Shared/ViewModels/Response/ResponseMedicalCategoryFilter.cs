using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseMedicalCategoryFilter
    {
        public IEnumerable<MedicalCategory> MedicalCategories { get; set; }

        public int Total { get; set; }
    }
}