namespace Shared.Interfaces
{
    public interface IPerson
    {
        int Id { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string Email { get; set; }

        string Password { get; set; }

        double? Birthday { get; set; }

        byte? Gender { get; set; }

        string Phone { get; set; }

        byte Role { get; set; }

        double Created { get; set; }

        double? LastModified { get; set; }

        byte Status { get; set; }

        string Address { get; set; }

        double? Longitude { get; set; }

        double? Latitude { get; set; }

        string Photo { get; set; }
    }
}