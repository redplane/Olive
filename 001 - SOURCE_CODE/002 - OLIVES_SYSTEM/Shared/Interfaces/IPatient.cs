namespace Shared.Interfaces
{
    public interface IPatient
    {
        int Money { get; set; }

        double? Weight { get; set; }

        double? Height { get; set; }
    }
}