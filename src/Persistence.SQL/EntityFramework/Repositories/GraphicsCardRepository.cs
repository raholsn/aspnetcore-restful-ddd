using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain;

using Microsoft.EntityFrameworkCore;

using Shared;

namespace Infrastructure.SQL.EntityFramework.Repositories
{
    public class GraphicsCardRepository : IGraphicsCardRepository
    {
        private readonly DataContext _context;
        public GraphicsCardRepository(DataContext context)
        {
            _context = context;
        }

        public Task<bool> Exists(Guid computerId, Guid graphicsCardId)
        {
            return _context.GraphicsCards.AnyAsync(g => g.ComputerId == computerId && g.Id == graphicsCardId);
        }

        public Task<bool> Exists(Guid graphicsCardId)
        {
            return _context.GraphicsCards.AnyAsync(g => g.Id == graphicsCardId);
        }

        public async Task<IEnumerable<GraphicsCard>> Get(IEnumerable<Guid> graphicsCardId)
        {
            return await _context.GraphicsCards.Where(x => graphicsCardId.Contains(x.Id)).ToListAsync();
        }

        public async Task<PagedList<GraphicsCard>> Get(int pageNumber, int pageSize, string filter, string searchQuery)
        {
            var collection = _context.GraphicsCards.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                collection = collection.Where(x => string.Equals(x.Name, filter.Trim(), StringComparison.InvariantCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                collection = collection.Where(x => x.Name.ToLowerInvariant().Contains(searchQuery.Trim()));
            }

            return PagedList<GraphicsCard>.Create(collection, pageNumber, pageSize);
        }

        public async Task<GraphicsCard> Get(Guid id)
        {
            return await _context.GraphicsCards.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task Create(IEnumerable<GraphicsCard> command)
        {
            await _context.GraphicsCards.AddRangeAsync(command);

            await _context.SaveChangesAsync();
        }

        public Task Create(GraphicsCard obj)
        {
            throw new NotSupportedException();
        }

        public Task Delete(GraphicsCard obj)
        {
            throw new NotSupportedException();
        }

        public async Task Update(GraphicsCard command)
        {
            _context.GraphicsCards.Update(command);

            await _context.SaveChangesAsync();
        }
    }
}
