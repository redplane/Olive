namespace Shared.ViewModels
{
    public class FilterDoctorViewModel : FilterPersonViewModel
    {
        public string Speciality { get; set; }

        public double? MinRank { get; set; }

        public double? MaxRank { get; set; }
    }
}