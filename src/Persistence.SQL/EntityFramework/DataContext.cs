using System;
using System.Collections.Generic;
using System.Text;

using Domain;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.EntityFramework
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<GraphicsCard> GraphicsCards { get; set; }
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Ram> Rams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLOCALDB; Database = Templates; Trusted_Connection = True;");
        }
    }
}
