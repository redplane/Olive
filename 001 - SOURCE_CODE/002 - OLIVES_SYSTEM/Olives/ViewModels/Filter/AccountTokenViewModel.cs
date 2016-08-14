using System;
using Shared.Interfaces;

namespace Olives.ViewModels.Filter
{
    public class FilterAccountTokenViewModel : IPagination
    {
        /// <summary>
        ///     Owner of account token.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        ///     Code of filter account.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Type of token.
        /// </summary>
        public byte? Type { get; set; }

        /// <summary>
        ///     When the token should be expired.
        /// </summary>
        public DateTime Expired { get; set; }

        /// <summary>
        ///     Index of result page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        ///     Number of record per page.
        /// </summary>
        public int? Records { get; set; }
    }
}