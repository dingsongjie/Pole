using Microsoft.EntityFrameworkCore;
using Product.Api.Domain.AggregatesModel.ProductTypeAggregate;
using Product.Api.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure
{
    public class ProductDbContext : DbContext
    {
        //public DbSet<Product.Api.Domain.ProductAggregate.Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public ProductDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.ApplyConfiguration(new ProductEntityTypeEntityTypeConfiguration());
            builder.ApplyConfiguration(new ProductTypeEntityTypeConfiguration());
        }

    }
}
