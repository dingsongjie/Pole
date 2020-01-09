using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Product.Api.Domain.ProductTypeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure.EntityConfigurations
{
    public class ProductTypeEntityTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            builder.ToTable(nameof(ProductType));

            builder.Property(m => m.Id).HasMaxLength(32);
            builder.Property(m => m.Name).HasMaxLength(256).IsRequired();

            builder.Ignore(m => m.DomainEvents);

            builder.HasKey(m => m.Id);
        }
    }
}
