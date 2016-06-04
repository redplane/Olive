using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace DotnetSignalR.ViewModels
{
    public class GetDoctorViewModel
    {
        /// <summary>
        ///     Doctor GUID.
        /// </summary>
        [Required(ErrorMessage = ErrorCodes.DoctorIdIsRequired)]
        public string Id { get; set; }
    }
}