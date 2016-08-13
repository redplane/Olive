namespace Olives.Constants
{
    public class OlivesValues
    {
        /// <summary>
        ///     Maximum characters length of doctor personal diary.
        /// </summary>
        public const int MaxDiaryLength = 512;

        /// <summary>
        ///     Maximum size of avatar
        /// </summary>
        public const int MaxAvatarSize = 2048;

        /// <summary>
        ///     Maximum character of relationship request content length.
        /// </summary>
        public const int MaxRelationshipRequestContentLength = 128;

        #region Email templates

        /// <summary>
        ///     Activation email template.
        /// </summary>
        public const string TemplateEmailActivationCode = "Activation";

        /// <summary>
        ///     Email find password.
        /// </summary>
        public const string TemplateEmailFindPassword = "FindPassword";

        #endregion
    }
}