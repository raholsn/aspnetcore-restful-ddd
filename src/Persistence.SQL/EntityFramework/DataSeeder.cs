using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoFixture;

using Domain;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.EntityFramework
{
    public class DataSeeder
    {
        public static void SeedCountries(DataContext context)
        {
            var fixture = new Fixture();

            if (!context.Computers.Any())
            {
                var computers = fixture.CreateMany<Computer>();

                context.AddRange(computers);
                context.SaveChanges();
            }
        }
    }
}