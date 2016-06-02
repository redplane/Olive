using Shared.Models;

namespace Shared.Interfaces
{
    public interface IPerson
    {
        /// <summary>
        ///     Person GUID.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        ///     Person last name.
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        ///     Person first name.
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        ///     Birthday (ticks).
        /// </summary>
        long Birthday { get; set; }

        /// <summary>
        ///     Person gender.
        /// </summary>
        byte Gender { get; set; }

        /// <summary>
        ///     Place where person is living at.
        /// </summary>
        Coordinate Address { get; set; }

        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        ///     Password of account.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        string Phone { get; set; }

        /// <summary>
        ///     Money in wallet.
        /// </summary>
        double Money { get; set; }

        /// <summary>
        ///     Time when account has been registered.
        /// </summary>
        long Created { get; set; }

        /// <summary>
        ///     Role of person [0 - Admin | 1 - Patient | 2 - Doctor]
        /// </summary>
        byte Role { get; set; }
    }
}