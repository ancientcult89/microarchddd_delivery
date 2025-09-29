using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.CourierAggregate;
using DeliveryApp.Infrastructure.Adapters.Postgres.EntityConfigurations.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Courier> Couriers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply Configuration
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CourierEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StoragePlaceEntityTypeConfiguration());
        }
    }
}
