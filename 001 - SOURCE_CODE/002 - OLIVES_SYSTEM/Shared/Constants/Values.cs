namespace Shared.Constants
{
    public class Values
    {
        /// <summary>
        ///     Minimum body temperature (celsius degrees)
        /// </summary>
        public const int MinBodyTemperature = 20;

        /// <summary>
        ///     Maximum body temperature (celsius degrees)
        /// </summary>
        public const int MaxBodyTemperature = 45;

        /// <summary>
        /// By default, time cannot be earlier than 1916.
        /// </summary>
        public const int MinimumAllowedYear = 1916;

        /// <summary>
        ///     Minimum body weight
        /// </summary>
        public const int MinBodyWeight = 1;

        /// <summary>
        ///     Maximum body weight
        /// </summary>
        public const int MaxBodyWeight = 500;

        /// <summary>
        ///     The lowest height body can be.
        /// </summary>
        public const int MinBodyHeight = 30;

        /// <summary>
        ///     The longest height body can be.
        /// </summary>
        public const int MaxBodyHeight = 400;

        /// <summary>
        ///     The shortest length password can be.
        /// </summary>
        public const int MinPasswordLength = 8;

        /// <summary>
        ///     The longest length password can be.
        /// </summary>
        public const int MaxPasswordLength = 16;
        
        /// <summary>
        /// Minimum heart rate.
        /// </summary>
        public const double MinHeartRate = 0;

        /// <summary>
        /// Maximum heart rate.
        /// </summary>
        public const double MaxHeartRate = 208;

        /// <summary>
        /// Maximum length of a note.
        /// </summary>
        public const int NoteMaxLength = 128;

        /// <summary>
        /// Minimum mol of sugar in one 1 litre of blood
        /// </summary>
        public const double MinSugarBloodMmol = 3;

        /// <summary>
        /// Maximum mol of sugar in one 1 litre of blood
        /// </summary>
        public const double MaxSugarBloodMmol = 20;

        /// <summary>
        /// Activation code will last for 24 hours.
        /// </summary>
        public const int ActivationCodeHourDuration = 24;

        #region Blood pressure

        /// <summary>
        /// Minimum value that diastolic can be assigned.
        /// </summary>
        public const int MinDiastolic = 60;

        /// <summary>
        /// Maximum value that diastolic can be assigned.
        /// </summary>
        public const int MaxDiastolic = 140;

        /// <summary>
        /// Minimum value that systolic can be assigned.
        /// </summary>
        public const int MinSystolic = 90;

        /// <summary>
        /// Maximum value that systolic can be assigned.
        /// </summary>
        public const int MaxSystolic = 250;

        #endregion
    }
}