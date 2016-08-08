using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repositories
{
    public class OliveDataContext : IOliveDataContext
    {
        public OlivesHealthEntities Context => new OlivesHealthEntities();
    }
}