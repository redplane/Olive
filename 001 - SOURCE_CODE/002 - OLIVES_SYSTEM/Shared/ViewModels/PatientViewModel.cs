using Shared.Interfaces;

namespace Shared.ViewModels
{
    public class PatientViewModel : IPerson, IPatient
    {
        public int Money { get; set; }

        public double? Weight { get; set; }

        public double? Height { get; set; }
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public long? Birthday { get; set; }

        public byte? Gender { get; set; }

        public string Phone { get; set; }

        public byte Role { get; set; }

        public long Created { get; set; }

        public long? LastModified { get; set; }

        public byte Status { get; set; }

        public string Address { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        public string Photo { get; set; }
    }
}