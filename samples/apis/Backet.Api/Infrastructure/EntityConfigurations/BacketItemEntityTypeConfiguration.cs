using Backet.Api.Domain.AggregatesModel.BacketAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Infrastructure.EntityConfigurations
{
    public class BacketItemEntityTypeConfiguration : IEntityTypeConfiguration<BacketItem>
    {
        public void Configure(EntityTypeBuilder<BacketItem> builder)
        {
            builder.ToTable(nameof(BacketItem));

            builder.Property(m => m.Id).HasMaxLength(32);
            builder.Property(m => m.ProductId).HasMaxLength(32);
            builder.Property(m => m.ProductName).HasMaxLength(256).IsRequired();
            builder.Property(m => m.BacketId).HasMaxLength(32).IsRequired();

            builder.Ignore(m => m.DomainEvents);
            builder.Ignore(m => m.IsPersisted);

            builder.HasKey(m => m.Id);
            builder.HasIndex(m => m.ProductId);
        }
    }
}
