using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using Newtonsoft.Json;
using Olives.Interfaces;
using Olives.Interfaces.Medical;
using Olives.Repositories;
using Olives.Repositories.Medical;
using Olives.ViewModels;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repositories;
using Shared.Services;

namespace DataInitializer
{
    internal class Program
    {
        private static ITimeService _timeService = new TimeService();

        private static void Main(string[] args)
        {
            // Initialize places.
            //InitializePlaces();

            InitializePatient(50);
        }

        /// <summary>
        ///     Initialize a list of places.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializePlaces()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            var countries = new [] {"Vietnam", "Another Vietnam"};
            var cities = new[]
            {
                "An Giang",
                "Bà Rịa - Vũng Tàu","Bắc Giang","Bắc Kạn","Bạc Liêu","Bắc Ninh","Bến Tre","Bình Định","Bình Dương","Bình Phước","Bình Thuận",
                "Cà Mau","Cao Bằng","Cần Thơ",
                "Đắk Lắk","Đắk Nông","Điện Biên","Đồng Nai","Đồng Tháp","Đà Nẵng",
                "Gia Lai",
                "Hà Giang","Hà Nam","Hà Tĩnh","Hải Dương","Hậu Giang","Hòa Bình","Hưng Yên","Hải Phòng","Hà Nội",
                "Khánh Hòa","Kiên Giang","Kon Tum",
                "Lai Châu","Lâm Đồng","Lạng Sơn","Lào Cai","Long An",
                "Nam Định","Nghệ An","Ninh Bình","Ninh Thuận",
                "Phú Thọ","Phú Yên",
                "Quảng Bình","Quảng Nam","Quảng Ngãi","Quảng Ninh","Quảng Trị",
                "Sóc Trăng","Sơn La",
                "Tây Ninh","Thái Bình","Thái Nguyên","Thanh Hóa","Thừa Thiên Huế","Tiền Giang","Trà Vinh","Tuyên Quang",
                "Vĩnh Long","Vĩnh Phúc","Yên Bái",
                "TP HCM"
            };

            foreach (var country in countries)
            {
                foreach (var city in cities)
                {
                    var place = new Place();
                    place.City = city;
                    place.Country = country;

                    context.Places.Add(place);
                }
            }
            
            // Save database changes.
            context.SaveChanges();
        }

        /// <summary>
        ///     Initialize a list of specialty.
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
        ///     Initialize a number of doctor.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializeDoctor(int max)
        {
            try
            {
                var context = new OlivesHealthEntities();
                var placesCounter = context.Places.Count();
                var specialty = context.Specialties.FirstOrDefault();

                if (placesCounter < 1)
                    throw new Exception("No city is available.");

                if (specialty == null)
                    throw new Exception("No specialty has been found");

                var random = new Random();
                for (var i = 0; i < max; i++)
                {
                    var person = new Person();
                    person.Email = $"doctor{i}@gmail.com";
                    person.Password = "doctor199x";
                    person.FirstName = $"DF[{i}]";
                    person.LastName = $"DL[{i}]";
                    person.FullName = person.FirstName + " " + person.LastName;
                    person.Gender = 0;
                    person.Role = (byte)Role.Doctor;
                    person.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                    person.Address = "New York, NY, USA";
                    person.Birthday = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);
                    if (i > 25)
                        person.Status = (byte)StatusAccount.Active;
                    else if (i == 25)
                        person.Status = (byte)StatusAccount.Pending;
                    else
                        person.Status = (byte)StatusAccount.Inactive;
                    
                    var doctor = new Doctor();
                    doctor.SpecialtyId = 1;
                    doctor.Person = person;
                    doctor.PlaceId = placesCounter;
                    doctor.Rank = random.Next(Values.MinDoctorRank, Values.MaxDoctorRank);
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
        ///     Initialize a number of patients.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializePatient(int max)
        {
            var context = new OlivesHealthEntities();
            var random = new Random();
            for (var i = 0; i < max; i++)
            {
                // General information.
                var person = new Person();
                person.Email = $"patient{i}@gmail.com";
                person.Password = "patient199x";
                person.FirstName = $"PF[{i}]";
                person.LastName = $"PL[{i}]";
                person.FullName = person.FirstName + " " + person.LastName;
                person.Gender = 0;
                person.Role = (byte)Role.Patient;
                person.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

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
                patient.Person = person;

                // Initialize or update patient information.
                context.Patients.AddOrUpdate(patient);
            }

            context.SaveChanges();
        }

        /// <summary>
        ///     Initialize a number of admin.
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
                person.FirstName = $"AF[{i}]";
                person.LastName = $"AL[{i}]";
                person.FullName = person.FirstName + " " + person.LastName;
                person.Gender = 0;
                person.Role = (byte)Role.Admin;
                person.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

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
        
        #region Personal records

        ///// <summary>
        /////     Initialize a list of heartbeat note.
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="records"></param>
        //private static async void InitializeHeartbeatNote(Patient patient, int records)
        //{
        //    // Database context initialization.
        //    var context = new OlivesHealthEntities();

        //    // The current time when loop starts.
        //    var unixTime = DateTime.UtcNow;
        //    var unixCurrentTime = TimeService.DateTimeUtcToUnix(unixTime);

        //    var random = new Random();
        //    var iMinHeartRate = (int)Values.MinHeartRate;
        //    var iMaxHeartRate = (int)Values.MaxHeartRate;

        //    for (var i = 0; i < records; i++)
        //    {
        //        // Note time.
        //        var createdTime = unixTime.Subtract(TimeSpan.FromDays(i));

        //        var heartbeatNote = new Heartbeat();
        //        heartbeatNote.Owner = patient.Id;
        //        heartbeatNote.Created = unixCurrentTime;
        //        heartbeatNote.Time = TimeService.DateTimeUtcToUnix(createdTime);
        //        heartbeatNote.Rate = random.Next(iMinHeartRate, iMaxHeartRate);

        //        context.Heartbeats.Add(heartbeatNote);
        //    }

        //    await context.SaveChangesAsync();
        //}

        ///// <summary>
        /////     Initialize a list of sugar blood notes.
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="records"></param>
        //private static async void InitializeSugarbloodNote(Patient patient, int records)
        //{
        //    // Database context initialization.
        //    var context = new OlivesHealthEntities();

        //    // Calculate the current unix time.
        //    var unixTime = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
        //    var time = DateTime.UtcNow;

        //    // Random number generator.
        //    var random = new Random();
        //    var iMinSugarMol = (int)Values.MinSugarBloodMmol;
        //    var iMaxSugarMol = (int)Values.MaxSugarBloodMmol;

        //    for (var i = 0; i < records; i++)
        //    {
        //        var createdTime = time.Subtract(TimeSpan.FromDays(i));

        //        var sugarblood = new SugarBlood();
        //        sugarblood.Owner = patient.Id;
        //        sugarblood.Created = TimeService.DateTimeUtcToUnix(createdTime);
        //        sugarblood.LastModified = TimeService.DateTimeUtcToUnix(createdTime);
        //        sugarblood.Time = TimeService.DateTimeUtcToUnix(createdTime);
        //        sugarblood.Value = random.Next(iMinSugarMol, iMaxSugarMol);

        //        context.SugarBloods.Add(sugarblood);
        //    }

        //    await context.SaveChangesAsync();
        //}

        ///// <summary>
        /////     Initialize a list of sugar blood notes.
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="records"></param>
        //private static async void InitializeBloodPressureNote(Patient patient, int records)
        //{
        //    // Database context initialization.
        //    var context = new OlivesHealthEntities();

        //    // Random number generator initialization.
        //    var random = new Random();

        //    // Time
        //    var time = DateTime.UtcNow;

        //    for (var i = 0; i < records; i++)
        //    {
        //        var unixCreated = TimeService.DateTimeUtcToUnix(time.Subtract(TimeSpan.FromDays(i)));

        //        var bloodPressure = new BloodPressure();
        //        bloodPressure.Owner = patient.Id;
        //        bloodPressure.Created = unixCreated;
        //        bloodPressure.Time = unixCreated;
        //        bloodPressure.Diastolic = random.Next(Values.MinDiastolic, Values.MaxDiastolic);
        //        bloodPressure.Systolic = random.Next(Values.MinSystolic, Values.MaxSystolic);

        //        context.BloodPressures.Add(bloodPressure);
        //    }

        //    await context.SaveChangesAsync();
        //}

        ///// <summary>
        /////     Initialize a list of notes of Allergy
        ///// </summary>
        ///// <param name="account"></param>
        ///// <param name="records"></param>
        //private static async void InitializeAllergyNote(Patient patient, int records)
        //{
        //    // Database context initialization.
        //    var context = new OlivesHealthEntities();

        //    // Random number generator.
        //    var random = new Random();

        //    // Current UTC time of system.
        //    var time = DateTime.UtcNow;

        //    for (var i = 0; i < records; i++)
        //    {
        //        // Time when record is created.
        //        var unixCreated = TimeService.DateTimeUtcToUnix(time.Subtract(TimeSpan.FromDays(i)));

        //        var allergy = new Allergy();
        //        allergy.Owner = patient.Id;
        //        allergy.Created = unixCreated;
        //        allergy.Cause = $"Cause{i}";
        //        allergy.Name = $"Name{i}";
        //        allergy.Note = $"Note{i}";

        //        context.Allergies.Add(allergy);
        //    }

        //    await context.SaveChangesAsync();
        //}

        //private static async void InitializeMessageAsync(Person broadcaster, Person recipient)
        //{
        //    var context = new OlivesHealthEntities();
        //    for (var i = 0; i < 100; i++)
        //    {
        //        var message = new Message();
        //        message.Broadcaster = broadcaster.Id;
        //        message.Recipient = recipient.Id;
        //        message.Content = $"MessageContent [{i}]";
        //        message.Created = i;

        //        context.Messages.Add(message);

        //    }

        //    await context.SaveChangesAsync();
        //}

        #endregion
    }
}