using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Shared.Models;

namespace OlivesAdministration.Controllers
{
    public class ApiParentController : ApiController
    {
        /// <summary>
        ///     Retrieve validation errors and bind 'em to list.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected object RetrieveValidationErrors(ModelStateDictionary modelState)
        {
            // Invalid model state.
            if (modelState == null)
                return null;

            var response = new ResponseErrror();
            response.Errors =
                new List<string>(
                    modelState.Keys.SelectMany(key => modelState[key].Errors.Select(error => error.ErrorMessage)));

            return response;
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            // By default, english will be used.
            var acceptLanguage = "en-US";

            // Check whether any language has been sent to server.
            var language = controllerContext.Request.Headers.AcceptLanguage.FirstOrDefault();

            if (language != null && !string.IsNullOrEmpty(language.Value))
                acceptLanguage = language.Value;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(acceptLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(acceptLanguage);
            }
            catch (Exception)
            {
                // Suppress exception
            }

            base.Initialize(controllerContext);
        }

        /// <summary>
        ///     Construct full url path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        protected string InitializeUrl(string path, string file, string extension)
        {
            // File or path is invalid
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(file))
                return null;

            if (!string.IsNullOrWhiteSpace(extension))
                file = $"{file}.{extension}";
            var fullPath = Path.Combine(path, file);
            return Url.Content(fullPath);
        }
    }
}