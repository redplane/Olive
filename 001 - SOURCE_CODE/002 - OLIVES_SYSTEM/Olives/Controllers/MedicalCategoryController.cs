using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces.Medical;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/medical/category")]
    public class MedicalCategoryController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryMedicalCategory"></param>
        /// <param name="log"></param>
        public MedicalCategoryController(IRepositoryMedicalCategory repositoryMedicalCategory,
            ILog log)
        {
            _repositoryMedicalCategory = repositoryMedicalCategory;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a medical note by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FindCategoryAsync([FromUri] int id)
        {
            // Find the category.
            var category = await _repositoryMedicalCategory.FindMedicalCategoryAsync(id, null, null);

            // Category is not found.
            if (category == null)
            {
                // Log the error.
                _log.Error($"Category [Id: {id}] is doesn't exist");

                // Respond client.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalCategory = new
                {
                    category.Id,
                    category.Name,
                    category.Created,
                    category.LastModified
                }
            });
        }

        /// <summary>
        ///     Filter medical categories asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/category/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> FilterCategoriesAsync([FromBody] FilterMedicalCategoryViewModel filter)
        {
            #region Paramters validation

            // Filter hasn't been initialized before.
            if (filter == null)
            {
                filter = new FilterMedicalCategoryViewModel();
                Validate(filter);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error and respond client.
                _log.Error("Request parameters are invalid. Errors sent to client.");

                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Filtering

            try
            {
                // Do the filter.
                var result = await _repositoryMedicalCategory.FilterMedicalCategoryAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalCategories = result.MedicalCategories.Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Created,
                        x.LastModified
                    }),
                    result.Total
                });
            }
            catch (Exception exception)
            {
                // Exception should be logged.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of medical category.
        /// </summary>
        private readonly IRepositoryMedicalCategory _repositoryMedicalCategory;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}