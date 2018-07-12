using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain;

using Microsoft.EntityFrameworkCore;

using Shared;

namespace Infrastructure.SQL.EntityFramework.Repositories
{
    public class ComputerRepository : IComputerRepository
    {
        private readonly DataContext _context;
        public ComputerRepository(DataContext context)
        {
            _context = context;
        }

        public Task<Computer> Get(Guid id)
        {
            return _context.Computers.Include(g => g.GraphicCards).Include(r => r.Rams).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<Computer>> Get(int pageNumber, int pageSize, string filter,string searchQuery)
        {
            var collection = _context.Computers
                                 .Include(g => g.GraphicCards)
                                 .Include(r => r.Rams)
                                 .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                collection = collection.Where(x => string.Equals(x.Name, filter.Trim(), StringComparison.InvariantCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                collection = collection.Where(x => x.Name.ToLowerInvariant().Contains(searchQuery.Trim().ToLowerInvariant()));
            }

            return PagedList<Computer>.Create(collection, pageNumber, pageSize);
        }

        public async Task Create(Computer computer)
        {
            await _context.AddAsync(computer);

            await _context.SaveChangesAsync();
        }

        public Task Create(IEnumerable<Computer> obj)
        {
            throw new NotSupportedException();
        }

        public async Task<bool> Exists(Guid computerId)
        {
            return await _context.Computers.AnyAsync(x => x.Id == computerId);
        }

        public async Task<bool> ComputerRamExists(Guid computerId, Guid ramId)
        {
            return await _context.Computers.AnyAsync(x => x.Id == computerId && x.Rams.Any(r => r.Id == ramId));
        }

        public async Task<bool> ComputerGraphicsCardExists(Guid computerId, Guid graphicsCardId)
        {
            return await _context.Computers.AnyAsync(x => x.Id == computerId && x.GraphicCards.Any(g => g.Id == graphicsCardId));
        }

        public async Task Delete(Computer computer)
        {
            _context.Remove(computer);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Computer computer)
        {
            _context.Computers.Update(computer);
            await _context.SaveChangesAsync();
        }

        public async Task<GraphicsCard> GetGraphicsCardForComputer(Guid computerId, Guid graphicCardId)
        {
            return await _context.GraphicsCards.FirstOrDefaultAsync(x => x.ComputerId == computerId && x.Id == graphicCardId);
        }

        public async Task<IEnumerable<GraphicsCard>> GetGraphicsCardsForComputer(Guid computerId)
        {
            var computer = await _context.Computers.Include(g => g.GraphicCards).FirstOrDefaultAsync(x => x.Id == computerId);
            return computer.GraphicCards;
        }

        public async Task<Ram> GetRamForComputer(Guid computerId, Guid ramId)
        {
            return await _context.Rams.FirstOrDefaultAsync(x => x.ComputerId == computerId && x.Id == ramId);
        }

        public async Task<IEnumerable<Ram>> GetRamsForComputer(Guid computerId)
        {
            var computer = await _context.Computers.Include(r => r.Rams).FirstOrDefaultAsync(x => x.Id == computerId);
            return computer.Rams;
        }



    }
}