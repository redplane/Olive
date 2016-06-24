using Shared.Enumerations;

namespace Shared.ViewModels.Response
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        
        public PersonViewModel Maker { get; set; }
        
        public PersonViewModel Dater { get; set; }
        
        public double From { get; set; }
        
        public double To { get; set; }
        
        public string Note { get; set; }
        
        public double Created { get; set; }

        public AccountStatus Status { get; set; }

        public double? LastModified { get; set; }
    }
}