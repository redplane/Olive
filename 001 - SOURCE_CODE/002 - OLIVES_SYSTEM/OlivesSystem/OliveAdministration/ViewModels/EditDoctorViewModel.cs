using Shared.ViewModels;

namespace DotnetSignalR.ViewModels
{
    public class EditDoctorViewModel : EditPersonViewModel
    {
        public string Specialization { get; set; }

        public string [] SpecializationAreas { get; set; }
        
        public double Rank { get; set; }
    }
}