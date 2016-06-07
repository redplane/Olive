namespace Shared.ViewModels
{
    public class FilterDoctorViewModel : FilterPersonViewModel
    {
        public string Specialization { get; set; }

        public double? MinRank { get; set; }

        public double? MaxRank { get; set; }

        public string IdentityCardNo { get; set; }
    }
}