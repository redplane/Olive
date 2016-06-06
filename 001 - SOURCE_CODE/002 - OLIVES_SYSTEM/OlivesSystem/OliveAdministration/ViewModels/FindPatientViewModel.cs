using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Shared.Attributes;
using Shared.Constants;

namespace DotnetSignalR.ViewModels
{
    public class FindPatientViewModel
    {
        [Required(ErrorMessage = ErrorCodes.PatientIdIsRequired)]
        [RegexMatch(Regexes.PersonalId, ErrorMessage = ErrorCodes.InvalidPatientId, Options = RegexOptions.IgnoreCase)]
        public string Id { get; set; } 
    }
}