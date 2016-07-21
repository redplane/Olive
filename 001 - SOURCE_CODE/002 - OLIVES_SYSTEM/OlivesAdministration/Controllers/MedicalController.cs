using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels.Initialize;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace OlivesAdministration.Controllers
{
    [OlivesAuthorize(new[] { Role.Admin })]
    public class MedicalController : ApiParentController
    {
        #region Properties

        /// <summary>
        /// Repository of account.
        /// </summary>
        private readonly IRepositoryMedical _repositoryMedical;

        /// <summary>
        /// Instance for logger.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize an instance of MedicalController with dependencies.
        /// </summary>
        /// <param name="repositoryMedical"></param>
        /// <param name="log"></param>
        public MedicalController(IRepositoryMedical repositoryMedical, ILog log)
        {
            _repositoryMedical = repositoryMedical;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize a medical category asynchronously.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/medical/category")]
        [HttpGet]
        public async Task<HttpResponseMessage> InitializeMedicalCategory([FromBody] InitializeMedicalCategoryViewModel initializer)
        {
            // Initializer hasn't been initialized.
            if (initializer == null)
            {
                initializer = new InitializeMedicalCategoryViewModel();
                Validate(initializer);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Find the category by using name.
            var medicalCategory = await _repositoryMedical.FindMedicalCategoryAsync(null, initializer.Name,
                StringComparison.CurrentCultureIgnoreCase);

            // Record is duplicated.
            if (medicalCategory != null)
            {
                // Log the error.
                _log.Error($"Category [Name: {initializer.Name}] has already been created.");

                // Respond to client.
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error = $"{Language.WarnRecordConflict}"
                });
            }

            medicalCategory = new MedicalCategory();
            medicalCategory.Name = initializer.Name;

            // Initialize the category into database.
            medicalCategory = await _repositoryMedical.InitializeMedicalCategoryAsync(medicalCategory);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalCategory = new
                {
                    medicalCategory.Id,
                    medicalCategory.Name,
                    medicalCategory.Created
                }
            });
        }

        /// <summary>
        /// Modify a medical category asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        [Route("api/medical/category")]
        [HttpPost]
        public async Task<HttpResponseMessage> ModifyMedicalCategory([FromUri] int id,
            [FromBody] InitializeMedicalCategoryViewModel modifier)
        {
            // Modifier hasn't been initialized.
            if (modifier == null)
            {
                modifier = new InitializeMedicalCategoryViewModel();
                Validate(modifier);
            }
            
            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Medical category.
            var medicalCategory = await _repositoryMedical.FindMedicalCategoryAsync(id, null, null);
            
            // Medical category is invalid.
            if (medicalCategory == null)
            {
                // Log the error and respond to client.
                _log.Error($"Medical record [Id: {id}] is not found.");

                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Check for duplicates.
            var dupplicate =
                await
                    _repositoryMedical.FindMedicalCategoryAsync(null, modifier.Name,
                        StringComparison.InvariantCultureIgnoreCase);

            if (dupplicate != null)
            {
                // Log the error and respond to client.
                _log.Error($"Medical record [Name: {modifier.Name}] does exist.");

                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Error = $"{Language.WarnRecordConflict}"
                });
            }

            // Update information.
            medicalCategory.Name = modifier.Name;
            medicalCategory.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);

            // Save changes.
            await _repositoryMedical.InitializeMedicalCategoryAsync(medicalCategory);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalCategory = new
                {
                    medicalCategory.Id,
                    medicalCategory.Name,
                    medicalCategory.Created,
                    medicalCategory.LastModified
                }
            });
        }

        /// <summary>
        /// Filter medical categories asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/category/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] { Role.Doctor, Role.Patient })]
        public async Task<HttpResponseMessage> FilterCategories([FromBody] FilterMedicalCategoryViewModel filter)
        {
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

            // Do the filter.
            var result = await _repositoryMedical.FilterMedicalCategoryAsync(filter);

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
        
        #endregion
    }
}