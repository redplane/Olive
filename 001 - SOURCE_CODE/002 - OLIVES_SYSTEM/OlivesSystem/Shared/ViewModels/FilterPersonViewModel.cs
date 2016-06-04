using Shared.Interfaces;

namespace Shared.ViewModels
{
    public class FilterPersonViewModel : IPagination
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long? BirthdayFrom { get; set; }

        public long? BirthdayTo { get; set; }

        public byte? Gender { get; set; }

        public long? MoneyFrom { get; set; }

        public long? MoneyTo { get; set; }

        public long? CreatedFrom { get; set; }

        public long? CreatedTo { get; set; }

        public byte? Role { get; set; }

        public int Page { get; set; }

        public int Records { get; set; }
    }
}