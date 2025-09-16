using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using TestUtils;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryShould : IAsyncLifetime
    {
        private ApplicationDbContext _context;

        public CourierRepositoryShould() { }

        #region DB configuring
        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("courier")
            .WithUsername("username")
            .WithPassword("password")
            .WithCleanUp(true)
            .Build();

        public async Task InitializeAsync()
        {
            //стартует БД (библа TestContainers, запускает в докере контекнер ПГ)
            await _postgreSqlContainer.StartAsync();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); }
                ).Options;

            _context = new ApplicationDbContext(contextOptions);
            _context.Database.Migrate();
        }
        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync().AsTask();
        }
        #endregion DB configuring

        [Fact]
        //по сути двойная проверка: мы проверяем и возможность добавления и возможность получения по ИД курьера
        //, в теории можно было бы тестовую миграцию организовать и через неё отдельно проверять GetById но нет времени заморачиваться
        public async Task CanAddCourier()
        {
            //Arrange
            string name = "Test courier";
            int speed = 2;
            Location location = Location.CreateRandom().Value;

            var testCourier = Courier.Create(name, speed, location).Value;

            //Act
            var courierRepository = new CourierRepository(_context);
            await courierRepository.AddAsync(testCourier);
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveChangesAsync();
            var getCourierResult = await courierRepository.GetAsync(testCourier.Id);

            //Assert
            getCourierResult.HasValue.Should().BeTrue();
            var courierFromDb = getCourierResult.Value;
            testCourier.Should().BeEquivalentTo(courierFromDb);
        }

        [Fact]
        public async Task CanUpdateOrder()
        {
            //Arrange
            string name = "Test courier for update";
            int speed = 2;
            Location location = Location.CreateRandom().Value;

            var testCourier = Courier.Create(name, speed, location).Value;

            var courierRepository = new CourierRepository(_context);
            await courierRepository.AddAsync(testCourier);
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveChangesAsync();

            //Act
            Location changedLocation = Location.CreateRandom().Value;
            //единственное что можем поменять напрямую - позиция, проверяем это
            testCourier.Move(changedLocation);
            Location oneStepLocation = testCourier.Location;

            //проверяем добавление новых хранилищь
            string newStoragePlaceName = "new storage";
            int newStoragePlaceVolume = 5;
            StoragePlace addingStoragePlace = StoragePlace.Create(newStoragePlaceName, newStoragePlaceVolume).Value;
            testCourier.AddStoragePlace(newStoragePlaceName, newStoragePlaceVolume);
            Order testOrder = TestModelCreator.CreateTestOrder();
            testCourier.TakeOrder(testOrder);

            //можно
            courierRepository.Update(testCourier);
            await unitOfWork.SaveChangesAsync();
            var getCourierResult = await courierRepository.GetAsync(testCourier.Id);

            //Assert
            getCourierResult.HasValue.Should().BeTrue();
            var courierFromDb = getCourierResult.Value;
            courierFromDb.Location.Should().BeEquivalentTo(oneStepLocation);
            courierFromDb.StoragePlaces.Where(sp => sp.OrderId == testOrder.Id).First().Should().NotBeNull();
        }


        [Fact]
        public async Task GetAllFreeCouriersShouldReturnOnlyFreeCouriers()
        {
            // Arrange
            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            var freeCourier1 = Courier.Create("Free сourier 1", 2, Location.CreateRandom().Value).Value;
            var freeCourier2 = Courier.Create("Free сourier 2", 3, Location.CreateRandom().Value).Value;

            var busyCourier = Courier.Create("Busy сourier", 4, Location.CreateRandom().Value).Value;
            var testOrder = TestModelCreator.CreateTestOrder();
            busyCourier.TakeOrder(testOrder);

            await courierRepository.AddAsync(freeCourier1);
            await courierRepository.AddAsync(freeCourier2);
            await courierRepository.AddAsync(busyCourier);
            await unitOfWork.SaveChangesAsync();

            // Act
            var freeCouriersResult = await courierRepository.GetAllFreeCouriersAsync();
            var freeCouriers = freeCouriersResult.Value;

            // Assert
            freeCouriers.Should().NotBeNull();
            freeCouriers.Should().HaveCount(2);
            freeCouriers.Should().Contain(c => c.Id == freeCourier1.Id);
            freeCouriers.Should().Contain(c => c.Id == freeCourier2.Id);
            freeCouriers.Should().NotContain(c => c.Id == busyCourier.Id);
        }

        [Fact]
        public async Task GetAllFreeCourierReturnEmptyListWhenNoFreeCouriers()
        {
            // Arrange
            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            var busyCourier1 = Courier.Create("Busy сourier 1", 2, Location.CreateRandom().Value).Value;
            var busyCourier2 = Courier.Create("Busy сourier 2", 3, Location.CreateRandom().Value).Value;

            var testOrder1 = TestModelCreator.CreateTestOrder();
            var testOrder2 = TestModelCreator.CreateTestOrder();

            busyCourier1.TakeOrder(testOrder1);
            busyCourier2.TakeOrder(testOrder2);

            await courierRepository.AddAsync(busyCourier1);
            await courierRepository.AddAsync(busyCourier2);
            await unitOfWork.SaveChangesAsync();

            // Act
            var freeCouriersResult = await courierRepository.GetAllFreeCouriersAsync();
            var freeCouriers = freeCouriersResult.Value;

            // Assert
            freeCouriers.Should().NotBeNull();
            freeCouriers.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllFreeCouriersShouldReturnAllCouriersWhenAllAreFree()
        {
            // Arrange
            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            var freeCourier1 = Courier.Create("Free сourier 1", 2, Location.CreateRandom().Value).Value;
            var freeCourier2 = Courier.Create("Free сourier 2", 3, Location.CreateRandom().Value).Value;
            var freeCourier3 = Courier.Create("Free сourier 3", 4, Location.CreateRandom().Value).Value;

            await courierRepository.AddAsync(freeCourier1);
            await courierRepository.AddAsync(freeCourier2);
            await courierRepository.AddAsync(freeCourier3);
            await unitOfWork.SaveChangesAsync();

            // Act
            var freeCouriersResult = await courierRepository.GetAllFreeCouriersAsync();
            var freeCouriers = freeCouriersResult.Value;

            // Assert
            freeCouriers.Should().NotBeNull();
            freeCouriers.Should().HaveCount(3);
            freeCouriers.Should().Contain(c => c.Id == freeCourier1.Id);
            freeCouriers.Should().Contain(c => c.Id == freeCourier2.Id);
            freeCouriers.Should().Contain(c => c.Id == freeCourier3.Id);
        }

        [Fact]
        public async Task GetAllFreeCouriersShouldHandleMultipleStoragePlaces()
        {
            // Arrange
            var courierRepository = new CourierRepository(_context);
            var unitOfWork = new UnitOfWork(_context);

            var courier = Courier.Create("Multi Storage Courier", 2, Location.CreateRandom().Value).Value;
            courier.AddStoragePlace("Backpack", 5);
            courier.AddStoragePlace("Box", 10);

            var testOrder = TestModelCreator.CreateTestOrder();
            courier.TakeOrder(testOrder);

            await courierRepository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            // Act
            var freeCouriersResult = await courierRepository.GetAllFreeCouriersAsync();
            var freeCouriers = freeCouriersResult.Value;

            // Assert
            freeCouriers.Should().NotBeNull();
            freeCouriers.Should().BeEmpty();
        }
    }
}
