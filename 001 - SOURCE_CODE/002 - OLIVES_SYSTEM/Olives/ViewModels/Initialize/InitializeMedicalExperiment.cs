using System.Collections.Generic;
using Shared.ViewModels;

namespace Olives.ViewModels.Initialize
{
    public class InitializeMedicalExperiment
    {
        /// <summary>
        ///     Medical record experiment should belong to.
        /// </summary>
        public int MedicalRecord { get; set; }

        /// <summary>
        ///     Experiment information.
        /// </summary>
        public List<ExperimentInfoViewModel> Info { get; set; }
    }
}