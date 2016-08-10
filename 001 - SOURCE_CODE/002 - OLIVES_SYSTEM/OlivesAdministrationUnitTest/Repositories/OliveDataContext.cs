using System;
using Effort;
using Effort.DataLoaders;
using OlivesAdministration.Test.Helpers;
using Shared.Interfaces;
using Shared.Models;

namespace OlivesAdministration.Test.Repositories
{
    public class OliveDataContext : IOliveDataContext
    {
        #region Properties

        private readonly string _olivesHealthEntities = "name=OlivesHealthEntities";

        public OlivesHealthEntities Context { get; }

        #endregion

        #region Constructors

        public OliveDataContext()
        {
            var connection = EntityConnectionFactory.CreateTransient(_olivesHealthEntities);
            Context = new OlivesHealthEntities(connection);
        }

        public OliveDataContext(OlivesHealthEntities context)
        {
            Context = context;
        }
        
        #endregion
    }
}