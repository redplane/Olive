using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Response
{
    public class ResponseExperimentNoteFilter
    {
        /// <summary>
        /// Experiment note which match with the conditions.
        /// </summary>
        public IList<ExperimentNote> ExperimentNotes { get; set; } 

        /// <summary>
        /// Total record number.
        /// </summary>
        public int Total { get; set; }
    }
}