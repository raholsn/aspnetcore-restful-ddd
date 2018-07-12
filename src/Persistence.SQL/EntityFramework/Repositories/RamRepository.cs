using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.EntityFramework.Repositories
{
    public class RamRepository : IRamRepository
    {
        private readonly DataContext _context;
        public RamRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> RamExists(Guid computerId)
        {
            return await _context.Rams.AnyAsync();
        }
    }
}
