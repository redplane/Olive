using System.Collections.Generic;

namespace Shared.Models.Nodes
{
    public class Doctor : Person
    {
        public string Specialization { get; set; }

        public string[] SpecializationAreas { get; set; }

        public double Rank { get; set; }

        public IList<MedicalExamPackage> Packages { get; set; }
    }
}