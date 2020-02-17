using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
//using Pole.Domain.EntityframeworkCore.MediatR;
//using Product.Api.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api
{
    //public class ProductDbContextDesignFactory : IDesignTimeDbContextFactory<ProductDbContext>
    //{
    //    public ProductDbContext CreateDbContext(string[] args)
    //    {
    //        IConfigurationRoot configuration = new ConfigurationBuilder()
    //         .SetBasePath(Directory.GetCurrentDirectory())
    //         .AddJsonFile("appsettings.Development.json")
    //         .Build();
    //        var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>()
    //            .UseNpgsql(configuration["postgres:main"]);

    //        return new ProductDbContext(optionsBuilder.Options);
    //    }
    //}
}
