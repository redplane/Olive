using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/place")]
    [OlivesAuthorize(new[] {Role.Admin})]
    public class PlaceController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of PlaceController.
        /// </summary>
        /// <param name="repositoryPlace"></param>
        /// <param name="log"></param>
        public PlaceController(IRepositoryPlace repositoryPlace, ILog log)
        {
            _repositoryPlace = repositoryPlace;
            _log = log;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository places DI
        /// </summary>
        private readonly IRepositoryPlace _repositoryPlace;

        /// <summary>
        ///     Logger module DI , this is used for writing log.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Methods
        
        /// <summary>
        /// Find a place by using id asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> FindPlace([FromUri] int id)
        {
            try
            {
                // Find the place by using id.
                var place = await _repositoryPlace.FindPlaceAsync(id, null, null, null, null);

                // Place is not found.
                if (place == null)
                {
                    // Log the error.
                    _log.Error($"Place [Id: {id}] is not found");

                    // Respond status to client.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }

                // Tell client result has been found.    
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Place = new
                    {
                        place.Id,
                        place.City,
                        place.Country
                    }
                });
            }
            catch (Exception exception)
            {
                // Unexpected error happens, log the error.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }

        }
        
        /// <summary>
        /// Filter place by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/place/filter")]
        [HttpPost]
        public async Task<HttpResponseMessage> FilterPlace([FromBody] FilterPlaceViewModel filter)
        {
            // Filter hasn't been initialized.
            if (filter == null)
            {
                // Initialize the filter and do validation.
                filter = new FilterPlaceViewModel();
                Validate(filter);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error to file.
                _log.Error("Request parameters are invalid. Errors sent to client");

                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            try
            {
                // Filter and retrieve the result.
                var result = await _repositoryPlace.FilterPlacesAsync(filter);

                // Tell the client about the result.
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Places = result.Places.Select(x => new
                    {
                        x.Id,
                        x.City,
                        x.Country
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // Log the exception.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        #endregion
    }
}