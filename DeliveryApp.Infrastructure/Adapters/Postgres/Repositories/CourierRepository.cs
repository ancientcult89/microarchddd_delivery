using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CourierRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task AddAsync(Courier courier)
        {
            await _dbContext.Couriers.AddAsync(courier);
        }

        public async Task<Maybe<Courier>> GetAsync(Guid courierId)
        {
            var courier = await _dbContext
                .Couriers
                .SingleOrDefaultAsync(c => c.Id == courierId);
            return courier;
        }

        public void Update(Courier courier)
        {
            _dbContext.Couriers.Update(courier);
        }
        public async Task<Maybe<List<Courier>>> GetAllFreeCouriers()
        {
            var couriersWithStorage = await _dbContext
                .Couriers
                .Include(c => c.StoragePlaces)
                .ToListAsync();

            var freeCouriers = couriersWithStorage
                .Where(c => c.StoragePlaces.All(sp => !sp.IsOccupied()))
                .ToList();

            return freeCouriers;
        }
    }
}
