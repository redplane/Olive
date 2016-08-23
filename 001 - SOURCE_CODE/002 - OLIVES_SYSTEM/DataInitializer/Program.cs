using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Olives.ViewModels;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Services;

namespace DataInitializer
{
    internal class Program
    {
        // Time service.
        private static readonly ITimeService TimeService = new TimeService();

        // Collection of first names.
        private static readonly string[] FirstNameCollection = {"Nguyen", "Pham", "Bui", "Tran", "Dao", "Luu", "Vu"};

        // Collection of last name.
        private static readonly string[] LastNameCollection =
        {"Duong", "Tuan", "Hung", "Thang", "Linh", "Trong", "Nguyen", "Ngoc", "Viet Anh", "Dung"};

        // Collection of specialties.
        private static readonly string[] Specialties =
        {
            "Internal medicine", "Surgery", "Plastic surgery", "Oncology", "Ophthalmology", "Urology",
            "Dietetics (and nutrition)", "Cardiology"
        };

        // Collection of medical categories.
        private static readonly string[] MedicalCategories =
        {
            "Blood sugar", "Blood pressure", "Internal medicine", "Nutrition", "Dental", "Mental", "In-Patient"
        };

        // List of prescription time.
        private static readonly string[] PrescriptionTime =
        {
            "Before meal", "After meal", "Before sleep",
            "Before breakfast", "After breakfast"
        };

        /// <summary>
        ///     Medical notes
        /// </summary>
        private static readonly string[] _medicalNotes =
        {
            "The situation is good", "The treatment is wrong",
            "Patient is getting better", "Patient is getting worse", "Patient is good", "The treatment can be end soon"
        };

        /// <summary>
        ///     Appointment notes
        /// </summary>
        private static readonly string[] _appointmentNotes =
        {
            "We should meet again", "Re-exam health status",
            "Daily exam", "Weekly exam"
        };

        /// <summary>
        /// What cause addiction
        /// </summary>
        private static readonly string[] _addictionNotes = {"Tobaco", "Alcohol", "Cigarette", "Internet", "Tea"};

        private static void Main()
        {
            //// Initialize places list.
            //Console.WriteLine("Initialize places");
            //InitializePlaces().Wait();

            //// Initialize specialties.
            //Console.WriteLine("Initialize specialties");
            //InitializeSpecialties().Wait();

            //// Initialize medical categories.
            //Console.WriteLine("Initialize medical categories");
            //InitializeMedicalCategories().Wait();

            //// Initialize a list of patients.
            //Console.WriteLine("Initialize patients");
            //InitializePatient(15).Wait();

            //// Initialize a list of doctors.
            //InitializeDoctor(15).Wait();

            //// Initialize a list of admins.
            //InitializeAdmin(15).Wait();

            //InitializePairValues().Wait();
            //InitializeMedicalRecordsInfo().Wait();
        }

        #region Private methods

        /// <summary>
        ///     Find the highest value in an enumeration.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        private static TEnum FindEnumerationHighestValue<TEnum>()
        {
            return Enum.GetValues(typeof (TEnum)).Cast<TEnum>().Max();
        }

        #endregion

        #region Doctor specific information

        /// <summary>
        ///     Initialize a list of places.
        /// </summary>
        private static async Task InitializePlaces()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            var countries = new[] {"Vietnam", "Another Vietnam"};
            var cities = new[]
            {
                "An Giang",
                "Bà Rịa - Vũng Tàu", "Bắc Giang", "Bắc Kạn", "Bạc Liêu", "Bắc Ninh", "Bến Tre", "Bình Định",
                "Bình Dương", "Bình Phước", "Bình Thuận",
                "Cà Mau", "Cao Bằng", "Cần Thơ",
                "Đắk Lắk", "Đắk Nông", "Điện Biên", "Đồng Nai", "Đồng Tháp", "Đà Nẵng",
                "Gia Lai",
                "Hà Giang", "Hà Nam", "Hà Tĩnh", "Hải Dương", "Hậu Giang", "Hòa Bình", "Hưng Yên", "Hải Phòng", "Hà Nội",
                "Khánh Hòa", "Kiên Giang", "Kon Tum",
                "Lai Châu", "Lâm Đồng", "Lạng Sơn", "Lào Cai", "Long An",
                "Nam Định", "Nghệ An", "Ninh Bình", "Ninh Thuận",
                "Phú Thọ", "Phú Yên",
                "Quảng Bình", "Quảng Nam", "Quảng Ngãi", "Quảng Ninh", "Quảng Trị",
                "Sóc Trăng", "Sơn La",
                "Tây Ninh", "Thái Bình", "Thái Nguyên", "Thanh Hóa", "Thừa Thiên Huế", "Tiền Giang", "Trà Vinh",
                "Tuyên Quang",
                "Vĩnh Long", "Vĩnh Phúc", "Yên Bái",
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
            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of specialty.
        /// </summary>
        private static async Task InitializeSpecialties()
        {
            var context = new OlivesHealthEntities();
            foreach (var specialtyName in Specialties)
            {
                var specialty = new Specialty();
                specialty.Name = specialtyName;

                context.Specialties.Add(specialty);
            }

            await context.SaveChangesAsync();
        }

        #endregion

        #region Accounts

        /// <summary>
        ///     Initialize a number of doctor.
        /// </summary>
        /// <param name="max"></param>
        private static async Task InitializeDoctor(int max)
        {
            // Database context generator.
            var context = new OlivesHealthEntities();

            // Count the total place.
            var places = await context.Places.ToListAsync();

            // Specialty of doctor.
            var specialties = await context.Specialties.ToListAsync();

            if (places == null || places.Count < 1)
                throw new Exception("No city is available.");

            if (specialties == null || specialties.Count < 1)
                throw new Exception("No specialty has been found");

            // Number generator.
            var numberGenerator = new Random();

            // Name section.
            var firstNameMaxElements = FirstNameCollection.Length;
            var lastNameMaxElements = LastNameCollection.Length;

            // Gender section.
            var genderMaximumValue = (byte) ((byte) FindEnumerationHighestValue<Gender>() + 1);

            // Current account status.
            var maxAccountStatus = (byte) ((byte) FindEnumerationHighestValue<StatusAccount>() + 1);

            var accountStatus = 0;

            for (var i = 0; i < max; i++)
            {
                // Generate the first name.
                var firstName = FirstNameCollection[numberGenerator.Next(firstNameMaxElements)];
                var lastName = LastNameCollection[numberGenerator.Next(lastNameMaxElements)];

                // Generated place
                var place = places[numberGenerator.Next(places.Count)];

                // Generated specialty.
                var specialty = specialties[numberGenerator.Next(specialties.Count)];

                var person = new Person();
                person.Email = $"doctor{i}@gmail.com";
                person.Password = "doctor199x";
                person.FirstName = firstName;
                person.LastName = lastName;
                person.FullName = $"{firstName} {lastName}";
                person.Gender = (byte) numberGenerator.Next(genderMaximumValue);
                person.Role = (byte) Role.Doctor;
                person.Created = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                person.Address = $"{place.City}, {place.Country}";
                person.Birthday = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                person.Status = (byte) accountStatus;

                var doctor = new Doctor();
                doctor.SpecialtyId = 1;
                doctor.Person = person;
                doctor.PlaceId = place.Id;
                doctor.Rank = numberGenerator.Next(Values.MinDoctorRank, Values.MaxDoctorRank);
                doctor.SpecialtyId = specialty.Id;
                context.Doctors.Add(doctor);

                if (accountStatus >= maxAccountStatus)
                    accountStatus = 0;
                else
                    accountStatus++;
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a number of patients.
        /// </summary>
        /// <param name="max"></param>
        private static async Task InitializePatient(int max)
        {
            // Context generator.
            var context = new OlivesHealthEntities();

            // Maximum status.
            var maximumStatus = (byte) FindEnumerationHighestValue<StatusAccount>() + 1;

            // Current account status.
            byte currentStatus = 0;

            var numberGenerator = new Random();
            var firstNameMaxElements = FirstNameCollection.Length;
            var lastNameMaxElements = LastNameCollection.Length;

            for (var i = 0; i < max; i++)
            {
                // Generate the first name.
                var firstName = FirstNameCollection[numberGenerator.Next(firstNameMaxElements)];
                var lastName = LastNameCollection[numberGenerator.Next(lastNameMaxElements)];

                // General information.
                var person = new Person();
                person.Email = $"patient{i}@gmail.com";
                person.Password = "patient199x";
                person.FirstName = firstName;
                person.LastName = lastName;
                person.FullName = $"{firstName} {lastName}";
                person.Gender = 0;
                person.Role = (byte) Role.Patient;
                person.Created = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                person.Status = currentStatus;

                // Specific information.
                var patient = new Patient();
                patient.Height = i;
                patient.Weight = i;
                patient.Person = person;

                if (currentStatus >= maximumStatus)
                    currentStatus = 0;
                else
                    currentStatus++;

                // Initialize or update patient information.
                context.Patients.AddOrUpdate(patient);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a number of patients.
        /// </summary>
        /// <param name="max"></param>
        private static async Task InitializeAdmin(int max)
        {
            // Context generator.
            var context = new OlivesHealthEntities();

            // Maximum status.
            var maximumStatus = (byte) FindEnumerationHighestValue<StatusAccount>() + 1;

            // Current account status.
            byte currentStatus = 0;

            var firstNameGenerator = new Random();
            var lastNameGenerator = new Random();
            var firstNameMaxElements = FirstNameCollection.Length;
            var lastNameMaxElements = LastNameCollection.Length;

            for (var i = 0; i < max; i++)
            {
                // Generate the first name.
                var firstName = FirstNameCollection[firstNameGenerator.Next(firstNameMaxElements)];
                var lastName = LastNameCollection[lastNameGenerator.Next(lastNameMaxElements)];

                // General information.
                var person = new Person();
                person.Email = $"admin{i}@gmail.com";
                person.Password = "admin199x";
                person.FirstName = firstName;
                person.LastName = lastName;
                person.FullName = $"{firstName} {lastName}";
                person.Gender = 0;
                person.Role = (byte) Role.Admin;
                person.Created = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);
                person.Status = currentStatus;

                if (currentStatus >= maximumStatus)
                    currentStatus = 0;
                else
                    currentStatus++;

                // Initialize or update patient information.
                context.People.AddOrUpdate(person);
            }

            await context.SaveChangesAsync();
        }

        #endregion

        #region Personal records

        private static async Task InitializePersonalRecords(List<Person> patients)
        {
            foreach (var patient in patients)
            {
                await InitializeHeartbeatNote(patient, 10);
                await InitializeBloodSugarNote(patient, 10);
                await InitializeBloodPressureNote(patient, 10);
                await InitializeAllergyNote(patient, 10);
                await InitializeAddictionNote(patient, 10);
            }
        }

        /// <summary>
        ///     Initialize a list of heartbeat note.
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="records"></param>
        private static async Task InitializeHeartbeatNote(Person patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // The current time when loop starts.
            var unixTime = DateTime.UtcNow;
            var unixCurrentTime = TimeService.DateTimeUtcToUnix(unixTime);

            var random = new Random();
            var iMinHeartRate = (int) Values.MinHeartRate;
            var iMaxHeartRate = (int) Values.MaxHeartRate;

            for (var i = 0; i < records; i++)
            {
                // Note time.
                var createdTime = unixTime.Subtract(TimeSpan.FromDays(i));

                var heartbeatNote = new Heartbeat();
                heartbeatNote.Owner = patient.Id;
                heartbeatNote.Created = unixCurrentTime;
                heartbeatNote.Time = TimeService.DateTimeUtcToUnix(createdTime);
                heartbeatNote.Rate = random.Next(iMinHeartRate, iMaxHeartRate);

                context.Heartbeats.Add(heartbeatNote);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of sugar blood notes.
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="records"></param>
        private static async Task InitializeBloodSugarNote(Person patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Calculate the current unix time.
            var time = DateTime.UtcNow;

            // Random number generator.
            var random = new Random();
            var iMinSugarMol = (int) Values.MinSugarBloodMmol;
            var iMaxSugarMol = (int) Values.MaxSugarBloodMmol;

            for (var i = 0; i < records; i++)
            {
                var createdTime = time.Subtract(TimeSpan.FromDays(i));

                var sugarblood = new BloodSugar();
                sugarblood.Owner = patient.Id;
                sugarblood.Created = TimeService.DateTimeUtcToUnix(createdTime);
                sugarblood.LastModified = TimeService.DateTimeUtcToUnix(createdTime);
                sugarblood.Time = TimeService.DateTimeUtcToUnix(createdTime);
                sugarblood.Value = random.Next(iMinSugarMol, iMaxSugarMol);

                context.BloodSugars.Add(sugarblood);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of sugar blood notes.
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="records"></param>
        private static async Task InitializeBloodPressureNote(Person patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Random number generator initialization.
            var random = new Random();

            // Time
            var time = DateTime.UtcNow;

            for (var i = 0; i < records; i++)
            {
                var unixCreated = TimeService.DateTimeUtcToUnix(time.Subtract(TimeSpan.FromDays(i)));

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
        /// <param name="patient"></param>
        /// <param name="records"></param>
        private static async Task InitializeAllergyNote(Person patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Current UTC time of system.
            var time = DateTime.UtcNow;

            for (var i = 0; i < records; i++)
            {
                // Time when record is created.
                var unixCreated = TimeService.DateTimeUtcToUnix(time.Subtract(TimeSpan.FromDays(i)));

                var allergy = new Allergy();
                allergy.Owner = patient.Id;
                allergy.Created = unixCreated;
                allergy.Cause = $"Allergy cause of {i}";
                allergy.Name = $"Allergy note {i}";
                allergy.Note = $"Note of allergy {i}";

                context.Allergies.Add(allergy);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of notes of Allergy
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="records"></param>
        private static async Task InitializeAddictionNote(Person patient, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Current UTC time of system.
            var time = DateTime.UtcNow;

            var random = new Random();

            for (var i = 0; i < records; i++)
            {
                // Time when record is created.
                var unixCreated = TimeService.DateTimeUtcToUnix(time.Subtract(TimeSpan.FromDays(i)));

                var addiction = new Addiction();
                addiction.Owner = patient.Id;
                addiction.Cause = _addictionNotes[random.Next(_addictionNotes.Length)];
                addiction.Note = $"Note of {addiction.Cause}";
                addiction.Created = unixCreated;
                context.Addictions.Add(addiction);
            }

            await context.SaveChangesAsync();
        }

        #endregion

        #region Pair values

        private static async Task InitializePairValues()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            var patients =
                await
                    context.People.Where(x => x.Role == (byte) Role.Patient && x.Status == (byte) StatusAccount.Active)
                        .ToListAsync();

            var doctors =
                await
                    context.People.Where(x => x.Role == (byte) Role.Doctor && x.Status == (byte) StatusAccount.Active)
                        .ToListAsync();

            await InitializeDiaries(doctors, patients, 5);
            await InitializeRelationships(doctors, patients);
            await InitializeMedicalRecord(doctors, patients, 5);
            await InitializeAppointments(doctors, patients, 10);
            await InitializePersonalRecords(patients);
        }

        // Initialize diaries.
        private static async Task InitializeDiaries(List<Person> doctors, List<Person> patients, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Current time
            var currentTime = DateTime.UtcNow;

            foreach (var doctor in doctors)
            {
                foreach (var patient in patients)
                {
                    for (var i = 0; i < records; i++)
                    {
                        currentTime = currentTime.AddDays(i);
                        var unix = TimeService.DateTimeUtcToUnix(currentTime);
                        var diary = new Diary();
                        diary.Owner = doctor.Id;
                        diary.Target = patient.Id;
                        diary.Time = unix;
                        diary.Note = $"This note position is : {i}";
                        diary.Created = unix;

                        context.Diaries.Add(diary);
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize relationship.
        /// </summary>
        /// <returns></returns>
        private static async Task InitializeRelationships(List<Person> doctors, List<Person> patients)
        {
            // Find the database context.
            var context = new OlivesHealthEntities();

            // Calculate the date time.
            var unix = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            foreach (var doctor in doctors)
            {
                foreach (var patient in patients)
                {
                    var relationship = new Relationship();
                    relationship.Source = patient.Id;
                    relationship.Target = doctor.Id;
                    relationship.Created = unix;

                    context.Relationships.Add(relationship);
                }
            }
            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize medical records.
        /// </summary>
        /// <param name="doctors"></param>
        /// <param name="patients"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private static async Task InitializeMedicalRecord(List<Person> doctors, List<Person> patients, int record)
        {
            var context = new OlivesHealthEntities();
            var medicalCategories = await context.MedicalCategories.ToListAsync();

            if (medicalCategories == null || medicalCategories.Count < 1)
                throw new Exception("Medical category is not found");
            var numberGenerator = new Random();

            var date = DateTime.UtcNow;

            foreach (var doctor in doctors)
            {
                foreach (var patient in patients)
                {
                    for (var medicalRecordIndex = 0; medicalRecordIndex < record; medicalRecordIndex++)
                    {
                        var created = date.Subtract(TimeSpan.FromDays(medicalRecordIndex));

                        var unix = TimeService.DateTimeUtcToUnix(created);
                        var selectedMedicalCategory = medicalCategories[numberGenerator.Next(medicalCategories.Count)];
                        var medicalRecord = new MedicalRecord();
                        medicalRecord.Owner = patient.Id;
                        medicalRecord.Creator = doctor.Id;
                        medicalRecord.Category = selectedMedicalCategory.Id;
                        medicalRecord.Name = $"This is medical record {medicalRecordIndex}";
                        medicalRecord.Time = unix;
                        medicalRecord.Created = unix;

                        context.MedicalRecords.Add(medicalRecord);
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize medical records.
        /// </summary>
        /// <param name="doctors"></param>
        /// <param name="patients"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private static async Task InitializeAppointments(List<Person> doctors, List<Person> patients, int record)
        {
            // Initialize database context.
            var context = new OlivesHealthEntities();

            // Initialize number generator.
            var numberGenerator = new Random();

            // Find the current time
            var date = DateTime.UtcNow;
            var unix = TimeService.DateTimeUtcToUnix(date);

            var status = 0;
            foreach (var doctor in doctors)
            {
                foreach (var patient in patients)
                {
                    for (var appointmentIndex = 0; appointmentIndex < record; appointmentIndex++)
                    {
                        // When the appointment created
                        var from = date.AddDays(appointmentIndex);
                        var to = from.AddDays(1);

                        var appointment = new Appointment();
                        appointment.Maker = doctor.Id;
                        appointment.MakerFirstName = doctor.FirstName;
                        appointment.MakerLastName = doctor.LastName;
                        appointment.Dater = patient.Id;
                        appointment.DaterFirstName = patient.FirstName;
                        appointment.DaterLastName = patient.LastName;

                        appointment.From = TimeService.DateTimeUtcToUnix(from);
                        appointment.To = TimeService.DateTimeUtcToUnix(to);
                        appointment.Note = _appointmentNotes[numberGenerator.Next(_appointmentNotes.Length)];
                        appointment.Created = unix;

                        if (status > 2)
                            status = 0;

                        appointment.Status = (byte) status;

                        context.Appointments.Add(appointment);
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        #endregion

        #region Medical

        /// <summary>
        ///     Initialize a list of medical categories.
        /// </summary>
        /// <returns></returns>
        private static async Task InitializeMedicalCategories()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Calculate the current time.
            var unix = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            foreach (var category in MedicalCategories)
            {
                var medicalCategory = new MedicalCategory();
                medicalCategory.Name = category;
                medicalCategory.Created = unix;

                context.MedicalCategories.Add(medicalCategory);
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize medical records.
        /// </summary>
        /// <returns></returns>
        private static async Task InitializeMedicalRecordsInfo()
        {
            var context = new OlivesHealthEntities();
            var medicalRecords = await context.MedicalRecords.ToListAsync();
            await InitializePrescription(medicalRecords, 10);
            await InitializeMedicalNote(medicalRecords, 10);
            await InitializeExperimentNote(medicalRecords, 10);
        }

        /// <summary>
        ///     Initialize prescription for a specific medical record.
        /// </summary>
        /// <param name="medicalRecords"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private static async Task InitializePrescription(List<MedicalRecord> medicalRecords, int record)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize the current time.
            var currentTime = DateTime.UtcNow;

            // Calculate current time unix.
            var unix = TimeService.DateTimeUtcToUnix(currentTime);

            var drugs = new Dictionary<string, string>();
            drugs = new Dictionary<string, string>();
            drugs.Add("Prednisolon", "Tablet");
            drugs.Add("Amoxicillin", "Tablet");
            drugs.Add("Citalopram", "mL");
            drugs.Add("Meloxicam", "mg");

            var random = new Random();
            foreach (var medicalRecord in medicalRecords)
            {
                for (var i = 0; i < record; i++)
                {
                    var unixEnd = TimeService.DateTimeUtcToUnix(currentTime.AddDays(i));

                    var prescription = new Prescription();
                    prescription.Creator = medicalRecord.Creator;
                    prescription.Owner = medicalRecord.Owner;
                    prescription.MedicalRecordId = medicalRecord.Id;
                    prescription.From = unix;
                    prescription.To = unixEnd;
                    prescription.Name = $"Name of prescription {i}";
                    prescription.Note = $"Note of prescription {i}";

                    var medicinesList = new Dictionary<string, MedicineInfoViewModel>();
                    foreach (var drug in drugs.Keys)
                    {
                        var medicineInfo = new MedicineInfoViewModel();
                        medicineInfo.Quantity = random.Next(1, 10);
                        medicineInfo.Note = PrescriptionTime[random.Next(PrescriptionTime.Length)];
                        medicineInfo.Unit = drugs[drug];
                    }

                    prescription.Medicine = JsonConvert.SerializeObject(medicinesList);
                    prescription.Created = unix;

                    context.Prescriptions.Add(prescription);
                }
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize prescription for a specific medical record.
        /// </summary>
        /// <param name="medicalRecords"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private static async Task InitializeMedicalNote(List<MedicalRecord> medicalRecords, int record)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Initialize the current time.
            var currentTime = DateTime.UtcNow;

            // Initialize number generator.
            var numberGenerator = new Random();

            foreach (var medicalRecord in medicalRecords)
            {
                for (var i = 0; i < record; i++)
                {
                    var unixEnd = TimeService.DateTimeUtcToUnix(currentTime.AddDays(i));

                    var medicalNote = new MedicalNote();
                    medicalNote.MedicalRecordId = medicalRecord.Id;
                    medicalNote.Creator = medicalRecord.Creator;
                    medicalNote.Owner = medicalRecord.Owner;
                    medicalNote.Note = _medicalNotes[numberGenerator.Next(_medicalNotes.Length)];
                    medicalNote.Time = unixEnd;
                    medicalNote.Created = unixEnd;

                    context.MedicalNotes.Add(medicalNote);
                }
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize experiment notes.
        /// </summary>
        /// <param name="medicalRecords"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private static async Task InitializeExperimentNote(List<MedicalRecord> medicalRecords, int record)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Current time
            var currentTime = DateTime.UtcNow;

            // Initialize number generator.
            var numberGenerator = new Random();

            foreach (var medicalRecord in medicalRecords)
            {
                for (var i = 0; i < record; i++)
                {
                    currentTime = currentTime.AddDays(1);
                    var unixCurrentTime = TimeService.DateTimeUtcToUnix(currentTime);

                    var experimentNote = new ExperimentNote();
                    experimentNote.MedicalRecordId = medicalRecord.Id;
                    experimentNote.Owner = medicalRecord.Owner;
                    experimentNote.Creator = medicalRecord.Creator;
                    experimentNote.Name = $"Experiment note {i}";

                    var notes = new Dictionary<string, double>();
                    notes.Add("Protein", numberGenerator.NextDouble()*10);
                    notes.Add("Calcium", numberGenerator.NextDouble()*10);
                    notes.Add("Lipid", numberGenerator.NextDouble()*10);
                    notes.Add("Glucid", numberGenerator.NextDouble()*10);

                    experimentNote.Time = unixCurrentTime;
                    experimentNote.Created = unixCurrentTime;

                    context.ExperimentNotes.Add(experimentNote);
                }
            }

            await context.SaveChangesAsync();
        }

        #endregion
    }
}