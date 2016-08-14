using System.Collections.Generic;
using Newtonsoft.Json;
using Olives.Models.Emails;

namespace Olives.Models
{
    public class SendGridSetting
    {
        #region Methods

        /// <summary>
        ///     Check whether smtp configuration is valid or not.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // Host hasn't been defined.
            if (string.IsNullOrWhiteSpace(ApiKey))
                return false;
            
            return true;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// API key which is provided by SendGrid system.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        ///     Email configurations.
        /// </summary>
        public Dictionary<string, SendGridPreconfiguration> SendGridPreconfigurations { get; set; }

        /// <summary>
        ///     The address which email is sent from.
        /// </summary>
        public string From { get; set; }
        
        #endregion
    }
}