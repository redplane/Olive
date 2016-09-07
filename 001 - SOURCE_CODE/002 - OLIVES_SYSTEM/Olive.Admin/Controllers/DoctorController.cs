using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Olive.Admin.Attributes;
using Olive.Admin.Interfaces;
using Olive.Admin.ViewModels.Filter;
using Shared.Enumerations;
using Shared.Resources;

namespace Olive.Admin.Controllers
{
    [MvcAuthorize(new[] { Role.Admin })]
    public class DoctorController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of DoctorController
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="log"></param>
        public DoctorController(
            IRepositoryAccountExtended repositoryAccountExtended,
            ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _log = log;
        }

        #endregion

        /// <summary>
        /// This function is for rendering doctor management page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        ///// <summary>
        /////     Access role : Admin
        /////     Description : Retrieve a doctor by using specific id
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<HttpResponseMessage> Get(int id)
        //{
        //    try
        //    {
        //        // Retrieve filtered result asynchronously.
        //        var account = await _repositoryAccountExtended.FindPersonAsync(id, null, null, (byte)Role.Doctor, null);

        //        // No result has been found.
        //        if (account == null)
        //        {
        //            // Log error.
        //            _log.Error($"Cannot find the doctor [Id : {id}]");
        //            return Request.CreateResponse(HttpStatusCode.NotFound, new
        //            {
        //                Error = $"{Language.WarnRecordNotFound}"
        //            });
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            Doctor = new
        //            {
        //                account.Id,
        //                account.FirstName,
        //                account.LastName,
        //                account.Email,
        //                account.Password,
        //                account.Birthday,
        //                account.Gender,
        //                account.Address,
        //                account.Phone,
        //                account.Role,
        //                Photo = account.PhotoUrl,
        //                account.Doctor.Rank,
        //                Specialty = new
        //                {
        //                    account.Doctor.Specialty.Id,
        //                    account.Doctor.Specialty.Name
        //                },
        //                Place = new
        //                {
        //                    account.Doctor.Place.Id,
        //                    account.Doctor.Place.City,
        //                    account.Doctor.Place.Country
        //                },
        //                account.Doctor.Voters,
        //                account.Created,
        //                account.LastModified,
        //                Profile = account.Doctor.ProfileUrl,
        //                account.Status
        //            }
        //        });
        //    }
        //    catch (Exception exception)
        //    {
        //        // Log the exception before telling client.
        //        _log.Error(exception.Message, exception);

        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }
        //}

        ///// <summary>
        /////     Filter doctors by using specific conditions.
        ///// </summary>
        ///// <param name="filter"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<HttpResponseMessage> Filter(FilterDoctorViewModel filter)
        //{
        //    #region Request parameters validation

        //    // Request parameters haven't been initialized.
        //    if (filter == null)
        //    {
        //        // Initialize the parameters and do validation.
        //        filter = new FilterDoctorViewModel();
        //        Validate(filter);
        //    }

        //    // Invalid data validation.
        //    if (!ModelState.IsValid)
        //    {
        //        _log.Error("Request parameters are invalid. Errors sent to client");
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
        //    }

        //    #endregion

        //    #region Result filtering & handling

        //    try
        //    {
        //        // Retrieve result from server.
        //        var result = await _repositoryAccountExtended.FilterDoctorsAsync(filter);

        //        // TODO: Update doctor profile PDF
        //        return Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            Doctors = result.Doctors.Select(x => new
        //            {
        //                x.Id,
        //                x.Person.FirstName,
        //                x.Person.LastName,
        //                x.Person.Email,
        //                x.Person.Password,
        //                x.Person.Birthday,
        //                x.Person.Gender,
        //                x.Person.Address,
        //                x.Person.Phone,
        //                x.Person.Role,
        //                Photo = x.Person.PhotoUrl,
        //                x.Rank,
        //                Specialty = new
        //                {
        //                    x.Specialty.Id,
        //                    x.Specialty.Name
        //                },
        //                Place = new
        //                {
        //                    x.Place.Id,
        //                    x.Place.City,
        //                    x.Place.Country
        //                },
        //                x.Voters,
        //                Profile = "",
        //                x.Person.Status,
        //                x.Person.Created,
        //                x.Person.LastModified
        //            }),
        //            result.Total
        //        });
        //    }
        //    catch (Exception exception)
        //    {
        //        _log.Error(exception.Message, exception);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }

        //    #endregion
        //}

        #region Properties

        /// <summary>
        ///     Instance of repository account.
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Instance for logging management.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}