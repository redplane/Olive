using Shared.Interfaces;

namespace Shared.ViewModels
{
    public class PatientViewModel : IPerson, IPatient
    {
        /// <summary>
        /// How much money patient has.
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// Patient's weight.
        /// </summary>
        public double? Weight { get; set; }

        /// <summary>
        /// Patient height.
        /// </summary>
        public double? Height { get; set; }

        /// <summary>
        /// Patient id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Patient first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Patient last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Patient email.
        /// This is used as account name.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Patient account password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Time when patient was born.
        /// </summary>
        public double? Birthday { get; set; }

        /// <summary>
        /// Patient gender.
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// Patient phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Patient role.
        /// </summary>
        public byte Role { get; set; }

        /// <summary>
        /// Patient created time.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// When the account was lastly modified.
        /// </summary>
        public double? LastModified { get; set; }

        /// <summary>
        /// Status of account [Disabled | Pending | Active]
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Address of patient.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Address longitude of patient.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Address latitude of patient.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Patient's avatar
        /// </summary>
        public string Photo { get; set; }
    }
}