using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace Shared.ViewModels
{
    public class GetPersonViewModel
    {
        /// <summary>
        ///     Doctor GUID.
        /// </summary>
        [Required(ErrorMessage = ErrorCodes.DoctorIdIsRequired)]
        public string Id { get; set; }
    }
}