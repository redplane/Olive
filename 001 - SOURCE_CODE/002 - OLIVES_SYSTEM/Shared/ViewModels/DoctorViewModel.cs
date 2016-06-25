using Shared.Interfaces;

namespace Shared.ViewModels
{
    public class DoctorViewModel : IPerson, IDoctor
    {
        public int Money { get; set; }

        public double Rank { get; set; }

        public string Specialty { get; set; }

        public int Voters { get; set; }
        public int Id { get; set; }

        public string Address { get; set; }

        public long? Birthday { get; set; }

        public long Created { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public byte? Gender { get; set; }

        public long? LastModified { get; set; }

        public string LastName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Photo { get; set; }

        public byte Role { get; set; }

        public byte Status { get; set; }
    }
}