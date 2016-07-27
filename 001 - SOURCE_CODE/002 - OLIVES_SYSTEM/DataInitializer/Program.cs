using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Permissions;
using Newtonsoft.Json;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repositories;
using Shared.ViewModels;

namespace DataInitializer
{
    internal class Program
    {
        private static readonly IRepositoryAccount RepositoryAccount = new RepositoryAccount();
        private static readonly IRepositoryMedicalRecord RepositoryMedical = new RepositoryMedicalRecord();
        private static readonly IRepositoryPrescription RepositoryPrescription = new RepositoryPrescription();
        private static readonly IRepositoryRelation RepositoryRelation = new RepositoryRelation();

        private static int MaxRecord = 50;

        private static void Main(string[] args)
        {
            Console.WriteLine("Intialize category");
            InitializeCategory(MaxRecord);

            Console.WriteLine("Initialize places");
            InitializePlaces(MaxRecord);

            Console.WriteLine("Initialize specialty");
            InitializeSpecialties(MaxRecord);

            Console.WriteLine("Initialize doctors");
            InitializeDoctor(MaxRecord);

            Console.WriteLine("Initialize patients");
            InitializePatient(MaxRecord);

            Console.WriteLine("Initialize admins");
            InitializeAdmin(MaxRecord);
            
            // Find the patient 26.
            var patient = RepositoryAccount.FindPerson(null, "patient26@gmail.com", null, (byte)Role.Patient, null);

            // Find the doctor 26.
            var doctor = RepositoryAccount.FindPerson(null, "doctor26@gmail.com", null, (byte)Role.Doctor, null);

            // Initialize medical records collection.
            InitializeMedicalRecord(patient, doctor, 2);

            // Initialize personal notes.
            
            Console.WriteLine("Initialize heartbeat notes");
            InitializeHeartbeatNote(patient.Patient, 90);

            Console.WriteLine("Initialize sugar bloods");
            InitializeSugarbloodNote(patient.Patient, 90);

            Console.WriteLine("Initialize blood pressures");
            InitializeBloodPressureNote(patient.Patient, 90);

            Console.WriteLine("Initialize allergy notes");
            InitializeAllergyNote(patient.Patient, 90);

            #region Relationship create

            for (var i = 26; i < 50; i++)
            {
                patient = RepositoryAccount.FindPerson(null, $"patient{i}@gmail.com", null, (byte)Role.Patient, null);
                doctor = RepositoryAccount.FindPerson(null, $"doctor{i}@gmail.com", null, (byte)Role.Doctor, null);

                if (patient != null)
                    Console.WriteLine($"Found {patient.Email} for creating relationship");
                else
                {
                    Console.WriteLine($"Cannot find {patient.Email} for creating relationship");
                    Console.WriteLine("---");
                    continue;
                }

                if (doctor != null)
                    Console.WriteLine($"Found {doctor.Email} for creating relationship");
                else
                {
                    Console.WriteLine($"Cannot find {doctor.Email} for creating relationship");
                    Console.WriteLine("---");
                    continue;
                }

                var relationship = new Relation();
                relationship.Source = patient.Id;
                relationship.SourceFirstName = patient.FirstName;
                relationship.SourceLastName = patient.LastName;

                relationship.Target = doctor.Id;
                relationship.TargetFirstName = doctor.FirstName;
                relationship.TargetLastName = doctor.LastName;

                relationship.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

                if (i > 40)
                    relationship.Status = (byte)StatusRelation.Pending;
                else
                    relationship.Status = (byte)StatusRelation.Active;

                relationship = RepositoryRelation.InitializeRelationAsync(relationship).Result;
                Console.WriteLine($"Created relationship. Id : {relationship.Id}");
            }

            #endregion

            #region Appointment create

            InitializeAppointment(patient, doctor, 50);

            #endregion
        }

        /// <summary>
        ///     Initialize a list of places.
        /// </summary>
        /// <param name="max"></param>
        private static void InitializePlaces(int max)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            for (var i = 0; i < max; i++)
            {
                var place = new Place();

                for (var j = 0; j < 10; j++)
                {
                    place.City = $"City[{j}]";
                    place.Country = $"Country[{j}]";
                }

                context.Places.Add(place);
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
                var places = context.Places.ToList();
                var specialty = context.Specialties.FirstOrDefault();

                if (places.Count < 1)
                    throw new Exception("No city is available.");

                if (specialty == null)
                    throw new Exception("No specialty has been found");

                var random = new Random();
                for (var i = 0; i < max; i++)
                {
                    var person = new Person();
                    person.Email = $"doctor{i}@gmail.com";
                    person.Password = "doctor199x";
                    person.FirstName = $"FirstName[{i}]";
                    person.LastName = $"LastName[{i}]";
                    person.FullName = person.FirstName + " " + person.LastName;
                    person.Gender = 0;
                    person.Role = (byte)Role.Doctor;
                    person.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
                    person.Address = "New York, NY, USA";
                    person.Birthday = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
                    person.Photo = $"{random.Next(1, 4)}";
                    if (i > 25)
                        person.Status = (byte)StatusAccount.Active;
                    else if (i == 25)
                        person.Status = (byte)StatusAccount.Pending;
                    else
                        person.Status = (byte)StatusAccount.Inactive;

                    var place = places[random.Next(places.Count)];

                    var doctor = new Doctor();
                    doctor.SpecialtyId = 1;
                    doctor.Person = person;
                    doctor.PlaceId = place.Id;
                    doctor.City = place.City;
                    doctor.Country = place.Country;
                    doctor.Rank = random.Next(0, 10);
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
                person.FirstName = $"FirstName[{i}]";
                person.LastName = $"LastName[{i}]";
                person.FullName = person.FirstName + " " + person.LastName;
                person.Photo = $"{random.Next(1, 4)}";
                person.Gender = 0;
                person.Role = (byte)Role.Patient;
                person.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

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
                person.FirstName = $"FirstName[{i}]";
                person.LastName = $"LastName[{i}]";
                person.FullName = person.FirstName + " " + person.LastName;
                person.Gender = 0;
                person.Role = (byte)Role.Admin;
                person.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

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

        /// <summary>
        /// Initialize medical record.
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="doctor"></param>
        /// <param name="records"></param>
        private static async void InitializeMedicalRecord(Person patient, Person doctor,int records)
        {
            // Calculate the current date time to current UTC time.
            var unix = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
            
            // Initialize random number generator.
            var random = new Random();
            
            for (var i = 0; i < records; i++)
            {
                // From time calculate.
                var fromTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(i));
                var unixFromTime = EpochTimeHelper.Instance.DateTimeToEpochTime(fromTime);

                #region Medical record

                var medicalRecord = new MedicalRecord();
                medicalRecord.Category = random.Next(1, MaxRecord);
                medicalRecord.Creator = doctor.Id;
                medicalRecord.Owner = patient.Id;

                var info = new Dictionary<string, string>();
                for (var key = 0; key < 5; key++)
                    info[$"Key[{i}][{key}]"] = $"Value[{i}][{key}]";

                medicalRecord.Info = JsonConvert.SerializeObject(info);
                medicalRecord.Time = unix;
                medicalRecord.Created = unix;
                medicalRecord.LastModified = null;

                // Initialize a new medical record to database.
                medicalRecord = await RepositoryMedical.InitializeMedicalRecordAsync(medicalRecord);

                #endregion

                #region Medical prescription

                // Each medical record contains 5 prescriptions.
                for (var p = 0; p < 5; p++)
                {
                    var prescription = new Prescription();
                    prescription.Owner = patient.Id;
                    prescription.Creator = doctor.Id;
                    prescription.MedicalRecordId = medicalRecord.Id;
                    prescription.From = unixFromTime;
                    prescription.To = unix;

                    var prescriptedMedicine = new Dictionary<string, MedicineInfoViewModel>();
                    for (var m = 0; m < 5; m++)
                    {
                        var medicine = new MedicineInfoViewModel();
                        medicine.Quantity = m;
                        medicine.Unit = $"Unit[{i}][{p}][{m}]";
                        medicine.Note = $"Note[{i}][{p}][{m}]";

                        prescriptedMedicine.Add($"Medicine[{i}][{p}][{m}]", medicine);
                    }

                    prescription.Medicine = JsonConvert.SerializeObject(prescriptedMedicine);
                    prescription.Note = $"Note[{i}]";
                    prescription.Created = unixFromTime;
                    
                    // Initialize a new medical prescription to database.
                    prescription = await RepositoryPrescription.InitializePrescriptionAsync(prescription);
                }

                #endregion

            }
        }

        /// <summary>
        /// Initialize a list of appointments.
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="doctor"></param>
        /// <param name="max"></param>
        private static void InitializeAppointment(Person patient, Person doctor, int max = 60)
        {
            var context = new OlivesHealthEntities();
            var half = max / 2;
            var quarter = max / 4;
            var secondQuater = quarter * 2;
            var thirdQuater = quarter * 3;

            var toTime = EpochTimeHelper.Instance.DateTimeToEpochTime(new DateTime(2016, 12, 31).ToUniversalTime());

            var month = 1;
            var year = 2016;

            for (var i = 0; i < max; i++)
            {
                var appointment = new Appointment();

                if (i < half)
                {
                    appointment.Maker = patient.Id;
                    appointment.MakerFirstName = patient.FirstName;
                    appointment.MakerLastName = patient.LastName;

                    appointment.Dater = doctor.Id;
                    appointment.DaterFirstName = doctor.FirstName;
                    appointment.DaterLastName = doctor.LastName;
                }
                else
                {
                    appointment.Dater = patient.Id;
                    appointment.DaterFirstName = patient.FirstName;
                    appointment.DaterLastName = patient.LastName;

                    appointment.Maker = doctor.Id;
                    appointment.MakerFirstName = doctor.FirstName;
                    appointment.MakerLastName = doctor.LastName;
                }

                

                var fromTime = new DateTime(year, month, 20);
                var epochFromTime = EpochTimeHelper.Instance.DateTimeToEpochTime(fromTime);
                
                appointment.From = epochFromTime;
                appointment.To = toTime;
                appointment.Note = $"Note[{i}]";
                appointment.Created = epochFromTime;

                if (i <= quarter)
                    appointment.Status = (byte)StatusAppointment.Cancelled;
                else if (quarter < i && i <= secondQuater)
                    appointment.Status = (byte)StatusAppointment.Pending;
                else if (secondQuater < i && i <= thirdQuater)
                    appointment.Status = (byte)StatusAppointment.Active;
                else
                    appointment.Status = (byte)StatusAppointment.Done;

                context.Appointments.Add(appointment);

                if (month >= 12)
                {
                    month = 1;
                    year++;
                }
            }

            context.SaveChanges();

        }
        
        /// <summary>
        /// Initialize a list of categories.
        /// </summary>
        /// <param name="records"></param>
        private static async void InitializeCategory(int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Calcuate the current unix time.
            var unix = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

            // Initialize a list of categories.
            for (var i = 0; i < records; i++)
            {
                var category = new MedicalCategory();
                category.Created = unix;
                category.Name = $"MedicalCategory[{i}]";
                context.MedicalCategories.Add(category);
            }

            // Save change asynchronously.
            await context.SaveChangesAsync();
        }
        
        #region Personal records

        /// <summary>
        ///     Initialize a list of heartbeat note.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
        private static async void InitializeHeartbeatNote(Patient patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // The current time when loop starts.
            var unixTime = DateTime.UtcNow;
            var unixCurrentTime = EpochTimeHelper.Instance.DateTimeToEpochTime(unixTime);

            var random = new Random();
            var iMinHeartRate = (int)Values.MinHeartRate;
            var iMaxHeartRate = (int)Values.MaxHeartRate;
            
            for (var i = 0; i < records; i++)
            {
                // Note time.
                var createdTime = unixTime.Subtract(TimeSpan.FromDays(i));

                var heartbeatNote = new Heartbeat();
                heartbeatNote.Owner = patient.Id;
                heartbeatNote.Created = unixCurrentTime;
                heartbeatNote.Time = EpochTimeHelper.Instance.DateTimeToEpochTime(createdTime);
                heartbeatNote.Rate = random.Next(iMinHeartRate, iMaxHeartRate);

                context.Heartbeats.Add(heartbeatNote);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of sugar blood notes.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
        private static async void InitializeSugarbloodNote(Patient patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
           
            // Calculate the current unix time.
            var unixTime = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
            var time = DateTime.UtcNow;

            // Random number generator.
            var random = new Random();
            var iMinSugarMol = (int)Values.MinSugarBloodMmol;
            var iMaxSugarMol = (int)Values.MaxSugarBloodMmol;
            
            for (var i = 0; i < records; i++)
            {
                var createdTime = time.Subtract(TimeSpan.FromDays(i));

                var sugarblood = new SugarBlood();
                sugarblood.Owner = patient.Id;
                sugarblood.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(createdTime);
                sugarblood.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(createdTime);
                sugarblood.Time = EpochTimeHelper.Instance.DateTimeToEpochTime(createdTime);
                sugarblood.Value = random.Next(iMinSugarMol, iMaxSugarMol);

                context.SugarBloods.Add(sugarblood);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of sugar blood notes.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
        private static async void InitializeBloodPressureNote(Patient patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
            
            // Random number generator initialization.
            var random = new Random();

            // Time
            var time = DateTime.UtcNow;

            for (var i = 0; i < records; i++)
            {
                var unixCreated = EpochTimeHelper.Instance.DateTimeToEpochTime(time.Subtract(TimeSpan.FromDays(i)));

                var bloodPressure = new BloodPressure();
                bloodPressure.Owner = patient.Id;
                bloodPressure.Created = unixCreated;
                bloodPressure.Time = unixCreated;
                bloodPressure.Diastolic = random.Next(Values.MinDiastolic, Values.MaxDiastolic);
                bloodPressure.Systolic = random.Next(Values.MinSystolic, Values.MaxSystolic);

                context.BloodPressures.Add(bloodPressure);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of notes of Allergy
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
        private static async void InitializeAllergyNote(Patient patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();
           
            // Random number generator.
            var random = new Random();

            // Current UTC time of system.
            var time = DateTime.UtcNow;

            for (var i = 0; i < records; i++)
            {
                // Time when record is created.
                var unixCreated = EpochTimeHelper.Instance.DateTimeToEpochTime(time.Subtract(TimeSpan.FromDays(i)));

                var allergy = new Allergy();
                allergy.Owner = patient.Id;
                allergy.Created = unixCreated;
                allergy.Cause = $"Cause{i}";
                allergy.Name = $"Name{i}";
                allergy.Note = $"Note{i}";

                context.Allergies.Add(allergy);
            }

            await context.SaveChangesAsync();
        }

        #endregion
    }
}