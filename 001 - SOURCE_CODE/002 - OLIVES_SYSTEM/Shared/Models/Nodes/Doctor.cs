using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Shared.Models.Nodes
{
    public class Doctor : Person
    {
        /// <summary>
        /// Speciality of doctor.
        /// </summary>
        [MaxLength(FieldLength.SpecializationMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidSpecializationLength")]
        public string Speciality { get; set; }

        /// <summary>
        /// Rank of doctor.
        /// </summary>
        public double Rank { get; set; }
        
        /// <summary>
        /// Number of people who vote this doctor.
        /// </summary>
        public int Voters { get; set; }
    }
}