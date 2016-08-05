using System.Collections.Generic;
using Shared.Models;

namespace Olives.ViewModels.Response
{
    public class ResponseDiaryFilter
    {
        /// <summary>
        /// List of filtered diaries.
        /// </summary>
        public IEnumerable<Diary> Diaries { get; set; }  

        /// <summary>
        /// Total filtered records.
        /// </summary>
        public int Total { get; set; }
    }
}