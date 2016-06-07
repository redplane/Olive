using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Shared.Models.Nodes
{
    public class PersonalNote
    {
        [RegexMatch(Regexes.PersonalNoteId, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidNoteId")]
        public string Id { get; set; }

        /// <summary>
        ///     Time when note was created.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        ///     Temperature of patient when this note was created.
        /// </summary>
        [Range(Values.MinBodyTemperature, Values.MaxBodyTemperature, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidTemperature")]
        public uint Temperature { get; set; }

        /// <summary>
        ///     Pulse of patient when this note was created.
        /// </summary>
        public uint Pulse { get; set; }

        /// <summary>
        ///     Minimum value of blood pressure of patient when this note was created.
        /// </summary>
        public uint BloodPressureMin { get; set; }

        /// <summary>
        ///     Maximum value of blood pressure of patient when this note was created.
        /// </summary>
        public uint BloodPressureMax { get; set; }
    }
}