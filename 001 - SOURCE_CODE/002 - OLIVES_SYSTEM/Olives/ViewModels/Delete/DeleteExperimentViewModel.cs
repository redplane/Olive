using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Delete
{
    public class DeleteExperimentViewModel
    {
        /// <summary>
        ///     List of experiment information.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        public HashSet<string> Keys { get; set; }

        [InEnumerationsArray(new object[] { NoteDeleteMode.Note, NoteDeleteMode.KeyValue }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public NoteDeleteMode Mode { get; set; }
    }
}