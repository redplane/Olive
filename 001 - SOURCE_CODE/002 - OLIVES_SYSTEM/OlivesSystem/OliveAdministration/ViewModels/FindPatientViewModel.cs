using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace DotnetSignalR.ViewModels
{
    public class FindPatientViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "RequirePersonalId")]
        [RegexMatch(Regexes.PersonalId, Options = RegexOptions.IgnoreCase, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidPersonalId")]
        public string Id { get; set; } 
    }
}