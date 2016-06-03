using Shared.Models;

namespace DotnetSignalR.ViewModels
{
    public class EditPersonViewModel
    {
        public string Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public long Birthday { get; set; }

        public byte Gender { get; set; }

        public Coordinate Address { get; set; }
        
        public string Password { get; set; }

        public string Phone { get; set; }

        public double Money { get; set; }

        public long Created { get; set; }

        public byte Role { get; set; }

        public byte Status { get; set; }
    }
}