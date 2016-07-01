using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Models;
using Shared.Repositories;

namespace DataInitializer
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            //InitializePlaces(50);
            //InitializeSpecialties(50);
            //InitializeDoctor(50);
            //InitializePatient(50);
            //InitializeAdmin(50);
            InitializeHeartbeatNote("patient26@gmail.com", 90);
        }

        /// <summary>
        /// Initialize a list of places.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializePlaces(int max)
        {
            try
            {
                // Data context initialization.
                var context = new OlivesHealthEntities();

                var country = new Country();
                country.Name = $"Country";
                context.Countries.Add(country);
                context.SaveChanges();

                for (var i = 0; i < max; i++)
                {
                    var city = new City();
                    city.Name = $"City[{i}]";
                    city.CountryId = country.Id;
                    city.CountryName = country.Name;

                    context.Cities.AddOrUpdate(city);
                }

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception)
            {
                
                throw;
            }
           
        }

        private static void InitializeHeartbeatNote(string account, int records)
        {
            var context = new OlivesHealthEntities();
            var repositoryAccount = new RepositoryAccount();
            var person = repositoryAccount.FindPerson(null, account, null, (byte) Role.Patient);
            if (person == null)
                throw new Exception($"Cannot find {account}");

            Console.WriteLine("Found {0}", person.Email);
            var random = new Random();
            var iMinHeartRate = (int) Values.MinHeartRate;
            var iMaxHeartRate = (int) Values.MaxHeartRate;

            var currentTime = DateTime.Now;
            for (var i = 0; i < records; i++)
            {

                var subtractedTime = currentTime.Subtract(TimeSpan.FromDays(i));
                var heartbeatNote = new Heartbeat();
                heartbeatNote.Owner = person.Id;
                heartbeatNote.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(currentTime);
                heartbeatNote.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(currentTime);
                heartbeatNote.Time = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                heartbeatNote.Rate = random.Next(iMinHeartRate, iMaxHeartRate);
                
                context.Heartbeats.Add(heartbeatNote);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Initialize a list of specialty.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializeSpecialties(int max)
        {
            var context = new OlivesHealthEntities();
            for (var i = 0; i < max; i++)
            {
                var specialty = new Specialty();
                specialty.Name = $"specialty[{i}]";

                context.Specialties.Add(specialty);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Initialize a number of doctor.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializeDoctor(int max)
        {
            try
            {
                var context = new OlivesHealthEntities();
                var city = context.Cities.FirstOrDefault();
                var specialty = context.Specialties.FirstOrDefault();

                if (city == null)
                    throw new Exception("No city is available.");

                if (specialty == null)
                    throw new Exception("No specialty has been found");

                for (var i = 0; i < max; i++)
                {
                    var person = new Person();
                    person.Email = $"doctor{i}@gmail.com";
                    person.Password = "doctor199x";
                    person.FirstName = $"FirstName[{i}]";
                    person.LastName = $"LastName[{i}]";
                    person.Gender = 0;
                    person.Role = (byte)Role.Doctor;
                    person.Created = 3;
                    person.Address = "New York, NY, USA";
                    person.Birthday = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

                    if (i > 25)
                        person.Status = (byte)StatusAccount.Active;
                    else if (i == 25)
                        person.Status = (byte)StatusAccount.Pending;
                    else
                        person.Status = (byte)StatusAccount.Inactive;

                    var doctor = new Doctor();
                    doctor.SpecialtyId = 1;
                    doctor.Person = person;
                    doctor.City = city;
                    doctor.SpecialtyName = specialty.Name;
                    doctor.Specialty = specialty;
                    context.Doctors.Add(doctor);
                }

                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Initialize a number of patients.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializePatient(int max)
        {
            var context = new OlivesHealthEntities();
            for (var i = 0; i < max; i++)
            {
                // General information.
                var person = new Person();
                person.Email = $"patient{i}@gmail.com";
                person.Password = "patient199x";
                person.FirstName = $"FirstName[{i}]";
                person.LastName = $"LastName[{i}]";
                person.Gender = 0;
                person.Role = (byte)Role.Patient;
                person.Created = 3;

                if (i > 25)
                    person.Status = (byte)StatusAccount.Active;
                else if (i == 25)
                    person.Status = (byte)StatusAccount.Pending;
                else
                    person.Status = (byte)StatusAccount.Inactive;
                
                // Specific information.
                var patient = new Patient();
                patient.Height = i;
                patient.Weight = i;
                patient.Money = i;
                patient.Person = person;
                
                // Initialize or update patient information.
                context.Patients.AddOrUpdate(patient);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Initialize a number of admin.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializeAdmin(int max)
        {
            var context = new OlivesHealthEntities();
            for (var i = 0; i < max; i++)
            {
                // General information.
                var person = new Person();
                person.Email = $"admin{i}@gmail.com";
                person.Password = "admin199x";
                person.FirstName = $"FirstName[{i}]";
                person.LastName = $"LastName[{i}]";
                person.Gender = 0;
                person.Role = (byte)Role.Admin;
                person.Created = 3;
                
                if (i > 25)
                    person.Status = (byte)StatusAccount.Active;
                else if (i == 25)
                    person.Status = (byte)StatusAccount.Pending;
                else
                    person.Status = (byte)StatusAccount.Inactive;

                context.People.Add(person);
            }

            context.SaveChanges();
        }
    }
}