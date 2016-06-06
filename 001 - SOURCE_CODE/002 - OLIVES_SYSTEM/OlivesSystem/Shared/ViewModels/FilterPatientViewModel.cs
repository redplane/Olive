namespace Shared.ViewModels
{
    public class FilterPatientViewModel : FilterPersonViewModel
    {
     
        public float? MinWeight { get; set; }    
        
        public float? MaxWeight { get; set; }
        
        public float? MinHeight { get; set; }

        public float? MaxHeight { get; set; }
    }
}