using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QuantityMeasurementApp.RepositoryLayer.Context
{
    public class QuantityDbContextFactory : IDesignTimeDbContextFactory<QuantityDbContext>
    {
        public QuantityDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection not found.");

            var optionsBuilder = new DbContextOptionsBuilder<QuantityDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new QuantityDbContext(optionsBuilder.Options);
        }
    }
}
