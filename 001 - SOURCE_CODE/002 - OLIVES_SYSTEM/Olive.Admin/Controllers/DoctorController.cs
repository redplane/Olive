using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Olive.Admin.Attributes;
using Olive.Admin.Interfaces;
using Olive.Admin.ViewModels.Filter;
using Shared.Enumerations;

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

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Filter(FilterDoctorViewModel filter)
        {
            // Invalid data validation.
            if (!ModelState.IsValid)
                return View(filter);

            try
            {
                // Retrieve result from server.
                var filteredResult = await _repositoryAccountExtended.FilterDoctorsAsync(filter);

                return Json(filteredResult);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

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