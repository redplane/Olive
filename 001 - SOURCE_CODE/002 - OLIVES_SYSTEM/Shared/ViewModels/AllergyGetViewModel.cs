using System;
using Shared.Interfaces;

namespace Shared.ViewModels
{
    public class AllergyGetViewModel : IPagination
    {
        public int? Id { get; set; }
        
        public int? Owner { get; set; } 

        public string Name { get; set; }

        public string Cause { get; set; }

        public string Note { get; set; }

        public long? MinCreated { get; set; }

        public long? MaxCreated { get; set; }

        public long? MinLastModified { get; set; }

        public long? MaxLastModified { get; set; }

        public int Page { get; set; }

        public int Records { get; set; }
    }
}