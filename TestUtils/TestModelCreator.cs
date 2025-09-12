using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;

namespace TestUtils
{
    public static class TestModelCreator
    {
        public static Order CreateTestOrder()
        {
            var orderId = Guid.NewGuid();
            var location = Location.Create(5, 5).Value;
            var volume = 10;

            return Order.Create(orderId, location, volume).Value;
        }

        public static Courier CreateTestCourier()
        {
            var courierId = Guid.NewGuid();
            var name = $"Test Courier {courierId}";
            var speed = 2;
            var location = Location.CreateRandom().Value;

            return Courier.Create(name, speed, location).Value;
        }
    }
}
