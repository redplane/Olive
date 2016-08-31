using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Models;

namespace OlivesAdministration.Test.Helpers
{
    public class EnvironmentHelper
    {
        #region Properties

        private static EnvironmentHelper _instance;

        /// <summary>
        ///     Retrieve the static instance of EnvironmentHelper.
        /// </summary>
        public static EnvironmentHelper Instance => _instance ?? (_instance = new EnvironmentHelper());

        #endregion

        #region Methods

        /// <summary>
        ///     Forge
        /// </summary>
        /// <returns></returns>
        public HttpContext ForgeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://mySomething/", "");
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(), 10, true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null, CallingConventions.Standard,
                    new[] {typeof(HttpSessionStateContainer)},
                    null)
                .Invoke(new object[] {sessionContainer});

            return httpContext;
        }

        /// <summary>
        ///     Initialize http context for a controller
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public void InitializeController(ApiController controller)
        {
            var httpConfiguration = new HttpConfiguration();
            var httpRequestMessage = new HttpRequestMessage();
            controller.Request = httpRequestMessage;
            controller.Request.Properties["MS_HttpConfiguration"] = httpConfiguration;
        }

        #endregion

        #region Initializer

        /// <summary>
        ///     Initialize a list of places.
        /// </summary>
        /// <param name="olivesHealthEntities"></param>
        /// <param name="max"></param>
        public async Task InitializePlaces(OlivesHealthEntities olivesHealthEntities, int max)
        {
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
                foreach (var city in cities)
                {
                    var place = new Place();
                    place.City = city;
                    place.Country = country;

                    olivesHealthEntities.Places.Add(place);
                }

            // Save database changes.
            await olivesHealthEntities.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a list of specialty.
        /// </summary>
        /// <param name="olivesHealthEntities"></param>
        /// <param name="max"></param>
        public async Task InitializeSpecialties(OlivesHealthEntities olivesHealthEntities, int max)
        {
            for (var i = 0; i < max; i++)
            {
                var specialty = new Specialty();
                specialty.Name = $"specialty[{i}]";

                olivesHealthEntities.Specialties.Add(specialty);
            }

            await olivesHealthEntities.SaveChangesAsync();
        }


        /// <summary>
        ///     Initialize a number of admin.
        /// </summary>
        /// <param name="olivesHealthEntities"></param>
        /// <param name="max"></param>
        public async Task InitializeAdmin(OlivesHealthEntities olivesHealthEntities, int max)
        {
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
                person.Role = (byte) Role.Admin;
                person.Created = 1;

                if (i > 25)
                    person.Status = (byte) StatusAccount.Active;
                else if (i == 25)
                    person.Status = (byte) StatusAccount.Pending;
                else
                    person.Status = (byte) StatusAccount.Inactive;

                olivesHealthEntities.People.Add(person);
            }

            await olivesHealthEntities.SaveChangesAsync();
        }


        /// <summary>
        ///     Initialize a number of doctor.
        /// </summary>
        /// <param name="olivesHealthEntities"></param>
        /// <param name="max"></param>
        public async Task InitializeDoctor(OlivesHealthEntities olivesHealthEntities, int max)
        {
            var placesCounter = olivesHealthEntities.Places.Count();
            var specialty = olivesHealthEntities.Specialties.FirstOrDefault();

            if (placesCounter < 1)
                throw new Exception("No city is available.");

            if (specialty == null)
                throw new Exception("No specialty has been found");

            byte status = 0;
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
                person.Role = (byte) Role.Doctor;
                person.Created = 1;
                person.Address = "New York, NY, USA";
                person.Birthday = 1;
                person.PhotoUrl = $"{random.Next(1, 4)}";
                person.PhotoPhysicPath = "1111";

                if (status > 2)
                    status = 0;
                person.Status = status;

                var doctor = new Doctor();
                doctor.SpecialtyId = 1;
                doctor.PlaceId = placesCounter;
                doctor.Voters = 0;
                doctor.Rank = random.Next(Values.MinDoctorRank, Values.MaxDoctorRank);
                doctor.Specialty = specialty;

                doctor.Person = person;
                person.Doctor = doctor;
                olivesHealthEntities.Doctors.Add(doctor);
            }

            await olivesHealthEntities.SaveChangesAsync();
        }

        /// <summary>
        ///     Initialize a number of medical categories.
        /// </summary>
        /// <param name="olivesHealthEntities"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public async Task InitializeMedicalCategories(OlivesHealthEntities olivesHealthEntities, int max)
        {
            // Initialize a list of categories.
            for (var i = 0; i < max; i++)
            {
                var category = new MedicalCategory();
                category.Created = 1;
                category.Name = $"{i}";
                olivesHealthEntities.MedicalCategories.Add(category);
            }

            // Save change asynchronously.
            await olivesHealthEntities.SaveChangesAsync();
        }

        #endregion
    }
}