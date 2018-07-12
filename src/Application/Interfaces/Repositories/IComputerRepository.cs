using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Domain;

namespace Application.Interfaces.Repositories
{
    public interface IComputerRepository : IBaseRepository<Computer>
    {
        Task<bool> Exists(Guid computerId);

        Task<IEnumerable<GraphicsCard>> GetGraphicsCardsForComputer(Guid computerId);

        Task<GraphicsCard> GetGraphicsCardForComputer(Guid computerId, Guid graphicCardId);

        Task<IEnumerable<Ram>> GetRamsForComputer(Guid computerId);

        Task<Ram> GetRamForComputer(Guid computerId, Guid ramId);

        Task<bool> ComputerRamExists(Guid computerId, Guid ramId);

        Task<bool> ComputerGraphicsCardExists(Guid computerId, Guid graphicsCardExists);

    }
}
