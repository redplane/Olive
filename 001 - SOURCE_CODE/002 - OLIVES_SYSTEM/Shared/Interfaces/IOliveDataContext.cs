using Shared.Models;

namespace Shared.Interfaces
{
    public interface IOliveDataContext
    {
        OlivesHealthEntities Context { get; }
    }
}