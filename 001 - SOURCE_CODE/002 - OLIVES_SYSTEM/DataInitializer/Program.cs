using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
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
        private static string[] _firstNameCollection;

        // Collection of last name.
        private static string[] _lastNameCollection;

        // Collection of specialties.
        private static string[] _specialties;

        // Collection of medical categories.
        private static string[] _medicalCategories;

        public Program()
        {
            _firstNameCollection = new[] {"Nguyen", "Pham", "Bui", "Tran", "Dao", "Luu", "Vu"};
            _lastNameCollection = new[]
            {"Duong", "Tuan", "Hung", "Thang", "Linh", "Trong", "Nguyen", "Ngoc", "Viet Anh", "Dung"};
            _specialties = new[]
            {
                "Internal medicine", "Surgery", "Plastic surgery", "Oncology", "Ophthalmology", "Urology",
                "Dietetics (and nutrition)", "Cardiology"
            };
            _medicalCategories = new[]
            {
                "Blood sugar", "Blood pressure", "Internal medicine", "Nutrition", "Dental", "Mental", "In-Patient"
            };
        }

        private static void Main()
        {
            // Initialize places list.
            InitializePlaces().Wait();

            // Initialize specialties.
            InitializeSpecialties().Wait();

            // Initialize medical categories.
            InitializeMedicalCategories().Wait();

            // Initialize a list of patients.
            InitializePatient(50).Wait();

            // Initialize a list of doctors.
            InitializeDoctor(50).Wait();

            // Initialize a list of admins.
            InitializeAdmin(50).Wait();

            // Initialize relationship.
            InitializeRelationships().Wait();

            // Initialize personal records.
            InitializePersonalRecords().Wait();
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

        #region Relationship

        /// <summary>
        ///     Initialize relationship.
        /// </summary>
        /// <returns></returns>
        private static async Task InitializeRelationships()
        {
            // Find the database context.
            var context = new OlivesHealthEntities();

            // Find the active patients.
            var patients =
                await
                    context.People.Where(x => x.Role == (byte) Role.Patient && x.Status == (byte) StatusAccount.Active)
                        .ToListAsync();

            // Find the active doctors.
            var doctors =
                await
                    context.People.Where(x => x.Role == (byte) Role.Doctor && x.Status == (byte) StatusAccount.Active)
                        .ToListAsync();

            // Calculate the date time.
            var unix = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            foreach (var patient in patients)
            {
                foreach (var doctor in doctors)
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
            foreach (var specialtyName in _specialties)
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
            var firstNameMaxElements = _firstNameCollection.Length;
            var lastNameMaxElements = _lastNameCollection.Length;

            // Gender section.
            var genderMaximumValue = (byte) ((byte) FindEnumerationHighestValue<Gender>() + 1);

            // Current account status.
            var maxAccountStatus = (byte) ((byte) FindEnumerationHighestValue<StatusAccount>() + 1);

            var accountStatus = 0;

            for (var i = 0; i < max; i++)
            {
                // Generate the first name.
                var firstName = _firstNameCollection[numberGenerator.Next(firstNameMaxElements)];
                var lastName = _lastNameCollection[numberGenerator.Next(lastNameMaxElements)];

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
            var firstNameMaxElements = _firstNameCollection.Length;
            var lastNameMaxElements = _lastNameCollection.Length;

            for (var i = 0; i < max; i++)
            {
                // Generate the first name.
                var firstName = _firstNameCollection[numberGenerator.Next(firstNameMaxElements)];
                var lastName = _lastNameCollection[numberGenerator.Next(lastNameMaxElements)];

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
            var firstNameMaxElements = _firstNameCollection.Length;
            var lastNameMaxElements = _lastNameCollection.Length;

            for (var i = 0; i < max; i++)
            {
                // Generate the first name.
                var firstName = _firstNameCollection[firstNameGenerator.Next(firstNameMaxElements)];
                var lastName = _lastNameCollection[lastNameGenerator.Next(lastNameMaxElements)];

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

        private static async Task InitializePersonalRecords()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find all active patients.
            var patients =
                await context.People.Where(x => x.Status != (byte) StatusAccount.Active && x.Role == (byte) Role.Patient).ToListAsync();

            foreach (var patient in patients)
            {
                await InitializeHeartbeatNote(patient, 10);
                await InitializeBloodSugarNote(patient, 10);
                await InitializeBloodPressureNote(patient, 10);
                await InitializeAllergyNote(patient, 10);

                await InitializeMedicalRecord(patient, 10);
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

        #endregion

        #region Medical

        private static async Task InitializeMedicalCategories()
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Calculate the current time.
            var unix = TimeService.DateTimeUtcToUnix(DateTime.UtcNow);

            foreach (var category in _medicalCategories)
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
        /// <param name="patient"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private static async Task InitializeMedicalRecord(Person patient, int record)
        {
            var context = new OlivesHealthEntities();
            var medicalCategories = await context.MedicalCategories.ToListAsync();

            if (medicalCategories == null || medicalCategories.Count < 1)
                throw new Exception("Medical category is not found");
            var numberGenerator = new Random();

            var date = DateTime.UtcNow;
            for (var medicalRecordIndex = 0; medicalRecordIndex < record; medicalRecordIndex++)
            {
                var created = date.Subtract(TimeSpan.FromDays(medicalRecordIndex));

                var unix = TimeService.DateTimeUtcToUnix(created);
                var selectedMedicalCategory = medicalCategories[numberGenerator.Next(medicalCategories.Count)];
                var medicalRecord = new MedicalRecord();
                medicalRecord.Owner = patient.Id;
                medicalRecord.Creator = patient.Id;
                medicalRecord.Category = selectedMedicalCategory.Id;
                medicalRecord.Name = $"This is medical record {medicalRecordIndex}";
                medicalRecord.Time = unix;
                medicalRecord.Created = unix;

                context.MedicalRecords.Add(medicalRecord);
            }

            await context.SaveChangesAsync();
        }

        #endregion
    }
}