using DataInitializer.Models;
using Shared.Enumerations;

namespace DataInitializer
{
    internal class Program
    {
        private static readonly OlivesHealthEntities Context = new OlivesHealthEntities();

        private static void Main(string[] args)
        {
            InitializeDoctor(0, 50);
            InitializePatient(0, 50);
            InitializeAdmin(0, 50);
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

                if (i >= 25)
                    person.Status = AccountStatus.Active;
                else
                    person.Status = AccountStatus.Inactive;

                var doctor = new Doctor();
                doctor.Email = person.Email;
                doctor.Specialty = $"Specialty[{pIndex}]";

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

                if (i >= 25)
                    person.Status = AccountStatus.Active;
                else
                    person.Status = AccountStatus.Pending;

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

                if (i >= 25)
                    person.Status = 2;
                else
                    person.Status = 0;

                var patient = new Patient();
                patient.Email = person.Email;

                Context.People.Add(person);
                Context.Patients.Add(patient);
            }

            Context.SaveChanges();
        }
    }
}