using System.Collections.Generic;
using System.Web;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repositories
{
    public class RepositoryStorage : IRepositoryStorage
    {
        #region Properties

        /// <summary>
        ///     List of storage which have been configured in configuration file.
        /// </summary>
        private readonly Dictionary<string, StorageModel> _storage;

        /// <summary>
        ///     Service http context.
        /// </summary>
        private readonly HttpContext _httpContext;

        #endregion

        #region Constructor

        public RepositoryStorage()
        {
        }

        /// <summary>
        ///     Initialize an instance with default settings.
        /// </summary>
        /// <param name="httpContext"></param>
        public RepositoryStorage(HttpContext httpContext)
        {
            _httpContext = httpContext;
            _storage = new Dictionary<string, StorageModel>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find the storage by using name from the configured list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public StorageModel FindStorage(string name)
        {
            // Storage name hasn't been configured.
            if (!_storage.ContainsKey(name))
                return null;

            return _storage[name];
        }

        /// <summary>
        ///     Configure a storage with relative path.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="relativePath"></param>
        public void InitializeStorage(string name, string relativePath)
        {
            // Create configuration first.
            var configuration = new StorageModel();
            configuration.Relative = relativePath;
            configuration.Absolute = _httpContext.Server.MapPath(relativePath);

            // Storage hasn't been configured before.
            if (!_storage.ContainsKey(name))
            {
                _storage.Add(name, configuration);
                return;
            }

            _storage[name] = configuration;
        }

        #endregion
    }
}