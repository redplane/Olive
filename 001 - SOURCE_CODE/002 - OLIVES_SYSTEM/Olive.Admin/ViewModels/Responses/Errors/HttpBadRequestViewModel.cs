using System.Collections.Generic;

namespace OliveAdmin.ViewModels.Responses.Errors
{
    public class HttpBadRequestViewModel
    {
        /// <summary>
        /// List of validation errors.
        /// </summary>
        public IEnumerable<string> Errors { get; set; }

        /// <summary>
        /// General error message.
        /// </summary>
        public string Message { get; set; } = "Request parameters are invalid";
    }
}