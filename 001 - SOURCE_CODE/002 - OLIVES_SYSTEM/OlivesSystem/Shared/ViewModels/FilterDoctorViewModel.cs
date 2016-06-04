namespace Shared.ViewModels
{
    public class FilterDoctorViewModel : FilterPersonViewModel
    {
        public string Specialization { get; set; }

        public string[] SpecializationAreas { get; set; }

        public double? RankFrom { get; set; }

        public double? RankTo { get; set; }
    }
}