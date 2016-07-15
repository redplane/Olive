using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseMedicalNoteFilter
    {
        /// <summary>
        /// List of filtered medical notes.
        /// </summary>
        public IList<MedicalNote> MedicalNotes { get; set; }

        /// <summary>
        /// Total results matched with conditions.
        /// </summary>
        public int Total { get; set; }
    }
}