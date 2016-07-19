namespace Shared.Interfaces
{
    public interface IPagination
    {
        int Page { get; set; }

        int? Records { get; set; }
    }
}