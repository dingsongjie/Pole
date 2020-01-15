using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure.EntityConfigurations
{
    public class ProductEntityTypeEntityTypeConfiguration : IEntityTypeConfiguration<Product.Api.Domain.ProductAggregate.Product>
    {
        public void Configure(EntityTypeBuilder<Product.Api.Domain.ProductAggregate.Product> builder)
        {
            builder.ToTable(nameof(Product));

            builder.Property(m => m.Id).HasMaxLength(32);
            builder.Property(m => m.Name).HasMaxLength(256).IsRequired();
            builder.Property(m => m.ProductTypeId).HasMaxLength(32).IsRequired();

            builder.Ignore(m => m.DomainEvents);

            builder.HasIndex(m => m.ProductTypeId);
            builder.HasKey(m => m.Id);
        }
    }
}
