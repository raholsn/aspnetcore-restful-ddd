using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Domain;

namespace Application.Interfaces.Repositories
{
    public interface IGraphicsCardRepository : IBaseRepository<GraphicsCard>
    {
        Task<bool> Exists(Guid computerId,Guid graphicsCardId);
        Task<bool> Exists(Guid graphicsCardId);

        //Task<IEnumerable<GraphicsCard>> Get();

        //Task<GraphicsCard> Get(Guid id);

        //Task Create(IEnumerable<GraphicsCard> command);

        //Task Update(GraphicsCard graphicsCard);
        Task<IEnumerable<GraphicsCard>> Get(IEnumerable<Guid> graphicsCardId);
    }
}
