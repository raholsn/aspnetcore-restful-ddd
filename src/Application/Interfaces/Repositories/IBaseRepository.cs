using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;

namespace Application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<PagedList<T>> Get(int pageNumber, int pageSize, string filter, string searchQuery);

        Task<T> Get(Guid id);

        Task Create(T obj);

        Task Create(IEnumerable<T> obj);

        Task Delete(T obj);

        Task Update(T obj);
    }
}
