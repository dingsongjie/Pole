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
    public class ProductDbContext : DbContextBase
    {
        public ProductDbContext(DbContextOptions options, IMediator mediator) : base(options, mediator)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ProductEntityTypeEntityTypeConfiguration());
            builder.ApplyConfiguration(new ProductTypeEntityTypeConfiguration());

            //builder.ApplyConfiguration(new ThinkNestDirectSalesManagerEntityTypeConfiguration());
            //builder.ApplyConfiguration(new ThinkNestInDirectSalesManagerEntityTypeConfiguration());

        }
    }
}
