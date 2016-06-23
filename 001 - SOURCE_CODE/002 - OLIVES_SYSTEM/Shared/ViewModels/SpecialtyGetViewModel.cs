using Shared.Constants;
using Shared.Interfaces;

namespace Shared.ViewModels
{
    public class SpecialtyGetViewModel : IPagination
    {
        /// <summary>
        /// Id of specialty.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Name of specialty.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Index of page.
        /// </summary>
        public int Page { get; set; } = 0;

        /// <summary>
        /// Number of records a page can display.
        /// </summary>
        public int Records { get; set; } = FieldLength.RecordMax;
    }
}