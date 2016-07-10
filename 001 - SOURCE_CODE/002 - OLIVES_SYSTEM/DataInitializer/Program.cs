using System;
using System.Collections.Generic;
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
        private static RepositoryAccount _repositoryAccount = new RepositoryAccount();
        private static RepositoryMedical _repositoryMedical = new RepositoryMedical();

        private static void Main(string[] args)
        {

            
            InitializePlaces(50);
            InitializeSpecialties(50);
            InitializeDoctor(50);
            InitializePatient(50);
            InitializeAdmin(50);
            InitializeHeartbeatNote("patient26@gmail.com", 90);
            InitializeSugarbloodNote("patient26@gmail.com", 90);
            InitializeBloodPressureNote("patient26@gmail.com", 90);
            InitializeAllergyNote("patient26@gmail.com", 90);

            var sourcePatient = _repositoryAccount.FindPerson(null, "patient26@gmail.com", null, (byte)Role.Patient);
            InitializeMedicalRecord(sourcePatient, 2);

            #region Relationship create

            for (var i = 26; i < 50; i++)
            {
                var patient = _repositoryAccount.FindPerson(null, $"patient{i}@gmail.com", null, (byte)Role.Patient);
                var doctor = _repositoryAccount.FindPerson(null, $"doctor{i}@gmail.com", null, (byte)Role.Doctor);

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
                    relationship.Status = (byte) StatusRelation.Pending;
                else
                    relationship.Status = (byte) StatusRelation.Active;

                relationship = _repositoryAccount.InitializeRelationAsync(relationship).Result;
                Console.WriteLine($"Created relationship. Id : {relationship.Id}");
            }

            #endregion

            
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

        #region Personal records

        /// <summary>
        /// Initialize a list of heartbeat note.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
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
        /// Initialize a list of sugar blood notes.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
        private static void InitializeSugarbloodNote(string account, int records)
        {
            var context = new OlivesHealthEntities();
            var repositoryAccount = new RepositoryAccount();
            var person = repositoryAccount.FindPerson(null, account, null, (byte)Role.Patient);
            if (person == null)
                throw new Exception($"Cannot find {account}");

            Console.WriteLine("Found {0}", person.Email);
            var random = new Random();
            var iMinSugarMol = (int)Values.MinSugarBloodMmol;
            var iMaxSugarMol = (int)Values.MaxSugarBloodMmol;

            var currentTime = DateTime.Now;
            for (var i = 0; i < records; i++)
            {

                var subtractedTime = currentTime.Subtract(TimeSpan.FromDays(i));
                var sugarblood = new SugarBlood();
                sugarblood.Owner = person.Id;
                sugarblood.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                sugarblood.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                sugarblood.Time = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                sugarblood.Value = random.Next(iMinSugarMol, iMaxSugarMol);

                context.SugarBloods.Add(sugarblood);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Initialize a list of sugar blood notes.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
        private static void InitializeBloodPressureNote(string account, int records)
        {
            var context = new OlivesHealthEntities();
            var repositoryAccount = new RepositoryAccount();
            var person = repositoryAccount.FindPerson(null, account, null, (byte)Role.Patient);
            if (person == null)
                throw new Exception($"Cannot find {account}");

            Console.WriteLine("Found {0}", person.Email);
            var random = new Random();
           
            var currentTime = DateTime.Now;
            for (var i = 0; i < records; i++)
            {

                var subtractedTime = currentTime.Subtract(TimeSpan.FromDays(i));
                var bloodPressure = new BloodPressure();
                bloodPressure.Owner = person.Id;
                bloodPressure.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                bloodPressure.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                bloodPressure.Time = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                bloodPressure.Diastolic = random.Next(Values.MinDiastolic, Values.MaxDiastolic);
                bloodPressure.Systolic = random.Next(Values.MinSystolic, Values.MaxSystolic);

                context.BloodPressures.Add(bloodPressure);
            }

            context.SaveChanges();
        }

        /// <summary>
        /// Initialize a list of notes of Allergy
        /// </summary>
        /// <param name="account"></param>
        /// <param name="records"></param>
        private static void InitializeAllergyNote(string account, int records)
        {
            var context = new OlivesHealthEntities();
            var repositoryAccount = new RepositoryAccount();
            var person = repositoryAccount.FindPerson(null, account, null, (byte)Role.Patient);
            if (person == null)
                throw new Exception($"Cannot find {account}");

            Console.WriteLine("Found {0}", person.Email);
            var random = new Random();

            var currentTime = DateTime.Now;
            for (var i = 0; i < records; i++)
            {

                var subtractedTime = currentTime.Subtract(TimeSpan.FromDays(i));
                var allergy = new Allergy();
                allergy.Owner = person.Id;
                allergy.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                allergy.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                allergy.Cause = $"Cause{i}";
                allergy.Name = $"Name{i}";
                allergy.Note = $"Note{i}";

                context.Allergies.Add(allergy);
            }

            context.SaveChanges();
        }
        #endregion

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

                    var doctor = new Doctor();
                    doctor.SpecialtyId = 1;
                    doctor.Person = person;
                    doctor.City = city;
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
        /// Initialize a number of patients.
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

        private static void InitializeMedicalRecord(Person patient, int records)
        {
            var epochCurrentTime = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);
            for (var i = 0; i < records; i++)
            {
                var fromTime = DateTime.Now.Subtract(TimeSpan.FromDays(i));
                var epochFromTime = EpochTimeHelper.Instance.DateTimeToEpochTime(fromTime);

                var medicalRecord = new MedicalRecord();
                medicalRecord.Owner = patient.Id;
                medicalRecord.Summary = $"Summary[{i}]";
                medicalRecord.Tests = $"Tests[{i}]";
                medicalRecord.AdditionalMorbidities = $"AdditionalMorbidities[{i}]";
                medicalRecord.DifferentialDiagnosis = $"DifferentialDiagnosis[{i}]";
                medicalRecord.OtherPathologies = $"OtherPathologies[{i}]";
                medicalRecord.Time = epochCurrentTime;
                medicalRecord.Created = epochCurrentTime;
                medicalRecord.LastModified = epochCurrentTime;
                medicalRecord = _repositoryMedical.InitializeMedicalRecordAsync(medicalRecord).Result;
                
                Console.WriteLine($"Medical record [{i}] has been created");

                for (var p = 0; p < 5; p++)
                {
                    var prescription = new Prescription();
                    prescription.Owner = patient.Id;
                    prescription.MedicalRecordId = medicalRecord.Id;
                    prescription.From = epochFromTime;
                    prescription.To = epochCurrentTime;
                    prescription.Note = $"Note[{i}]";
                    prescription.Created = epochCurrentTime;
                    prescription = _repositoryMedical.InitializePrescriptionAsync(prescription).Result;

                    Console.WriteLine($"Prescription[{p}] has been created");

                    for (var m = 0; m < 5; m++)
                    {
                        var prescriptedMedicine = new PrescriptedMedicine();
                        prescriptedMedicine.PrescriptionId = prescription.Id;
                        prescriptedMedicine.Owner = prescription.Owner;
                        prescriptedMedicine.MedicineName = $"Medicine[{p}][{m}]]";
                        prescriptedMedicine.Quantity = m;
                        prescriptedMedicine.Unit = $"Unit[{p}][{m}]]";
                        prescriptedMedicine.Note = $"Note[{p}][{m}]]";
                        prescriptedMedicine.Expired = prescription.To;

                        prescriptedMedicine = _repositoryMedical.InitializePrescriptedMedicineAsync(prescriptedMedicine).Result;

                        Console.WriteLine($"Prescripted medicine [{prescriptedMedicine}] has been created");
                    }
                }

                for (var e = 0; e < 5; e++)
                {
                    var experimentNote = new ExperimentNote();
                    experimentNote.Owner = medicalRecord.Owner;
                    experimentNote.MedicalRecordId = medicalRecord.Id;
                    experimentNote.Created = epochCurrentTime;

                    var infos = new Dictionary<string, double>();
                    for (var d = 0; d < 5; d++)
                        infos.Add($"Key[{d}]", d);
                    
                    experimentNote = _repositoryMedical.InitializeExperimentNote(experimentNote, infos).Result;

                }
            }
        }

        private static void InitializeMedicalImage(string account, int records)
        {
            var context = new OlivesHealthEntities();
            var repositoryAccount = new RepositoryAccount();
            var person = repositoryAccount.FindPerson(null, account, null, (byte)Role.Patient);
            if (person == null)
                throw new Exception($"Cannot find {account}");

            Console.WriteLine("Found {0}", person.Email);
            var random = new Random();

            var currentTime = DateTime.Now;
            for (var i = 0; i < records; i++)
            {

                var subtractedTime = currentTime.Subtract(TimeSpan.FromDays(i));
                var medicalImage = new MedicalImage();
                medicalImage.Image = $"{random.Next(1, 4)}";
                medicalImage.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(subtractedTime);
                medicalImage.Owner = person.Id;
                
                context.MedicalImages.Add(medicalImage);
            }

            context.SaveChanges();
        }
    }
}