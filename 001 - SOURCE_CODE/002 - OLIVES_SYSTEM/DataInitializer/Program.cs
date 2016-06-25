using Shared.Enumerations;
using Shared.Models;

namespace DataInitializer
{
    internal class Program
    {
        private static readonly OlivesHealthEntities Context = new OlivesHealthEntities();

        private static void Main(string[] args)
        {
            InitializeSpecialties(0, 50);
            InitializeDoctor(0, 50);
            InitializePatient(0, 50);
            InitializeAdmin(0, 50);
        }

        private static void InitializeSpecialties(int startIndex, int max)
        {
            for (var i = 0; i < max; i++)
            {
                var specialty = new Specialty();
                specialty.Name = $"specialty[{i}]";

                Context.Specialties.Add(specialty);
            }

            Context.SaveChanges();
        }
        private static void InitializeDoctor(int startIndex, int max)
        {
            for (var i = 0; i < max; i++)
            {
                var pIndex = startIndex + i;
                var person = new Person();
                person.Email = $"doctor{pIndex}@gmail.com";
                person.Password = "doctor199x";
                person.FirstName = $"FirstName[{pIndex}]";
                person.LastName = $"LastName[{pIndex}]";
                person.Gender = 0;
                person.Role = AccountRole.Doctor;
                person.Created = 3;

                if (i > 25)
                    person.Status = (byte)AccountStatus.Active;
                else if (i == 25)
                    person.Status = (byte)AccountStatus.Pending;
                else
                    person.Status = (byte)AccountStatus.Inactive;

                var doctor = new Doctor();
                doctor.Email = person.Email;
                doctor.SpecialtyId = 1;

                Context.People.Add(person);
                Context.Doctors.Add(doctor);
            }

            Context.SaveChanges();
        }

        private static void InitializePatient(int startIndex, int max)
        {
            for (var i = 0; i < max; i++)
            {
                var pIndex = startIndex + i;
                var person = new Person();
                person.Email = $"patient{pIndex}@gmail.com";
                person.Password = "patient199x";
                person.FirstName = $"FirstName[{pIndex}]";
                person.LastName = $"LastName[{pIndex}]";
                person.Gender = 0;
                person.Role = AccountRole.Patient;
                person.Created = 3;

                if (i > 25)
                    person.Status = (byte)AccountStatus.Active;
                else if (i == 25)
                    person.Status = (byte)AccountStatus.Pending;
                else
                    person.Status = (byte)AccountStatus.Inactive;

                var patient = new Patient();
                patient.Email = person.Email;

                Context.People.Add(person);
                Context.Patients.Add(patient);
            }

            Context.SaveChanges();
        }

        private static void InitializeAdmin(int startIndex, int max)
        {
            for (var i = 0; i < max; i++)
            {
                var pIndex = startIndex + i;
                var person = new Person();
                person.Email = $"admin{pIndex}@gmail.com";
                person.Password = "admin199x";
                person.FirstName = $"FirstName[{pIndex}]";
                person.LastName = $"LastName[{pIndex}]";
                person.Gender = 0;
                person.Role = 1;
                person.Created = 3;

                if (i > 25)
                    person.Status = (byte)AccountStatus.Active;
                else if (i == 25)
                    person.Status = (byte)AccountStatus.Pending;
                else
                    person.Status = (byte)AccountStatus.Inactive;

                var patient = new Patient();
                patient.Email = person.Email;

                Context.People.Add(person);
            }

            Context.SaveChanges();
        }
    }
}