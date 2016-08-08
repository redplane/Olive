using System;
using System.Collections.Generic;
using System.Linq;

namespace MultipartFormDataMediaFormatter.Models
{
    public class MultipartFormData
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of FormData
        /// </summary>
        public MultipartFormData()
        {
            Files = new List<FileModel>();
            Fields = new List<PrimitiveModel>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     List of primitive parameters
        /// </summary>
        public List<FileModel> Files;

        /// <summary>
        ///     List of primitive fields.
        /// </summary>
        public List<PrimitiveModel> Fields;

        #endregion

        #region Methods

        /// <summary>
        ///     Retrieve list of every keys submitted to server.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> AllKeys()
        {
            return Files.Select(m => m.Name).Union(Files.Select(m => m.Name));
        }

        /// <summary>
        ///     Add primitive parameters to list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            Fields.Add(new PrimitiveModel {Name = name, Value = value});
        }

        /// <summary>
        ///     Add file parameters to list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, HttpFileModel value)
        {
            Files.Add(new FileModel {Name = name, Value = value});
        }

        /// <summary>
        ///     Parse data and retrieve primitive data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out string value)
        {
            var field =
                Fields.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (field != null)
            {
                value = field.Value;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        ///     Parse data and return HttpFile data.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out HttpFileModel value)
        {
            var field = Files.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (field != null)
            {
                value = field.Value;
                return true;
            }
            value = null;
            return false;
        }

        #endregion
    }
}