using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels;
using OlivesAdministration.ViewModels.Edit;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Controllers
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
        /// Initialize a place asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> InitializePlace([FromBody] InitializePlaceViewModel initializer)
        {
            #region Parameters validation

            // Initializer hasn't been initialized.
            if (initializer == null)
            {
                // Initialize it and do validation.
                initializer = new InitializePlaceViewModel();
                Validate(initializer);
            }

            // Request paramters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Place validation

            // Find the place with the city and country to prevent duplicates.
            var place =
                await
                    _repositoryPlace.FindPlaceAsync(null, initializer.City, StringComparison.InvariantCultureIgnoreCase,
                        initializer.Country, StringComparison.InvariantCultureIgnoreCase);

            // Place already exists.
            if (place != null)
            {
                // Log the error.
                _log.Error($"Place [City : {initializer.City} | Country : {initializer.Country}] already exists");

                // Tell the client the request is conflicted.
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error = $"{Language.WarnRecordConflict}"
                });
            }

            #endregion

            #region Data initialization

            // Initialize a new place.
            place = new Place();
            place.City = initializer.City;
            place.Country = initializer.Country;

            try
            {
                // Try to initialize a new place to database.
                place = await _repositoryPlace.InitializePlaceAsync(place);

                // Log the result.
                _log.Info($"Created place [Id: {place.Id} | City: {place.City} | Country: {place.Country}] successfully");

                // Tell the client about the result.
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
                // Log the error.
                _log.Error(exception.Message, exception);

                // Tell the client about the error.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        /// Modify a place asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="editor"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<HttpResponseMessage> ModifyPlace([FromUri] int id, [FromBody] EditPlaceViewModel editor)
        {
            // Editor hasn't been initialized.
            if (editor == null)
            {
                editor = new EditPlaceViewModel();
                Validate(editor);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log error.
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Find the place by id.
            var place = await _repositoryPlace.FindPlaceAsync(id, null, null, null, null);
            
            // Place is invalid.
            if (place == null)
            {
                // Log the error.
                _log.Error($"Place [Id: {id}] isn't found");

                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Find the place by city & country.
            var targetPlace = await _repositoryPlace.FindPlaceAsync(null, editor.City,
                StringComparison.InvariantCultureIgnoreCase, editor.Country, StringComparison.InvariantCultureIgnoreCase);

            // The place already exists.
            if (targetPlace != null)
            {
                // Log the error.
                _log.Error($"Place [City : {editor.City} | Country : {editor.Country}] already exists.");

                // Tell the client about result.
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error = $"{Language.WarnRecordConflict}"
                });
            }

            // Edit the information.
            place.City = editor.City;
            place.Country = editor.Country;

            try
            {
                // Modify place information.
                place = await _repositoryPlace.ModifyPlaceAsync(place.Id, place);

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
                // Log the error and respond information to client.
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