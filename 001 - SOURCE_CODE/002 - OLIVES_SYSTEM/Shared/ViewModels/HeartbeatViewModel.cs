namespace Shared.ViewModels
{
    public class HeartbeatViewModel
    {
        public int Id { get; set; }
        
        public double Rate { get; set; }

        public string Note { get; set; }

        public double Time { get; set; }

        public double Created { get; set; }

        public double? LastModified { get; set; }
    }
}