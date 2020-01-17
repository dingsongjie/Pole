using Backet.Api.Domain.AggregatesModel.BacketAggregate;
using Backet.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Infrastructure
{
    public class BacketDbContext : DbContext
    {
        public BacketDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet> Backets { get; set; }
        public DbSet<BacketItem> BacketItems { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new BacketItemEntityTypeConfiguration());
            builder.ApplyConfiguration(new BacketEntityTypeConfiguration());
        }
    }
}
