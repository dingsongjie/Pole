﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Infrastructure.EntityConfigurations
{
    public class BacketEntityTypeConfiguration : IEntityTypeConfiguration<Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet>
    {
        public void Configure(EntityTypeBuilder<Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet> builder)
        {
            builder.ToTable(nameof(Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet));

            builder.Property(m => m.Id).HasMaxLength(32);
            builder.Property(m => m.UserId).HasMaxLength(32).IsRequired();
            builder.HasMany(m => m.BacketItems).WithOne().IsRequired().OnDelete(DeleteBehavior.Cascade).HasForeignKey("BacketId");

            builder.Ignore(m => m.DomainEvents);
            builder.Ignore(m => m.IsPersisted);

            builder.HasKey(m => m.Id);
            builder.HasIndex(m => m.UserId);
        }
    }
}
