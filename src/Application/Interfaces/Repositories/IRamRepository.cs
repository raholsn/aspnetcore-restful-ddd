using System;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IRamRepository
    {
        Task<bool> RamExists(Guid computerId);
    }
}
