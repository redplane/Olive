namespace Shared.Interfaces
{
    public interface IDoctor
    {
        double Rank { get; set; }
        
        string Specialty { get; set; }
        
        int Voters { get; set; }
        
        int Money { get; set; } 
    }
}