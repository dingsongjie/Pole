using MediatR;
using Microsoft.EntityFrameworkCore;
using Pole.EntityframeworkCore;
using Product.Api.Infrastructure.EntityConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure
{
    public class ProductDbContext : DbContext
    {
        public DbSet<Product.Api.Domain.ProductAggregate.Product> Products { get; set; }
        public DbSet<Product.Api.Domain.ProductTypeAggregate.ProductType> ProductTypes { get; set; }
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ProductEntityTypeEntityTypeConfiguration());
            builder.ApplyConfiguration(new ProductTypeEntityTypeConfiguration());
        }
    }
}
