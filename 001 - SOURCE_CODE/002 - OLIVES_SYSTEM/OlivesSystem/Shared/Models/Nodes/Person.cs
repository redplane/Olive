using Shared.Interfaces;

namespace Shared.Models.Nodes
{
    public class Person : IPerson
    {
        /// <summary>
        ///     Status of account.
        /// </summary>
        public byte Status { get; set; }
        
        /// <summary>
        ///     Person GUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Person last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Person first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Birthday (ticks).
        /// </summary>
        public long Birthday { get; set; }

        /// <summary>
        ///     Person gender.
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Password of this account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     Money in wallet.
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        ///     Time when account has been registered.
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        /// Latitude of place where person lives.
        /// </summary>
        public double? AddressLatitude { get; set; }

        /// <summary>
        /// Longitude of place where person lives.
        /// </summary>
        public double? AddressLongitude { get; set; }

        /// <summary>
        ///     Role of person [0 - Admin | 1 - Patient | 2 - Doctor]
        /// </summary>
        public byte Role { get; set; }
    }
}