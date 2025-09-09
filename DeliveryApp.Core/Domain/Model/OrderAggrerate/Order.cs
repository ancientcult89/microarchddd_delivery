using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggrerate
{
    public class Order : Aggregate<Guid>
    {
        public Location Location { get; private set; }
        public int Volume { get; private set; }
        public OrderStatus Status { get; private set; }
        public Guid? CourierId { get; private set; }

        private Order() { }

        private Order(Guid orderId, Location location, int volume) : this()
        {
            Id = orderId;
            Location = location;
            Volume = volume;
        }

        public static Result<Order, Error> Create(Guid orderId, Location location, int volume)
        {
            if (orderId == Guid.Empty)
                return GeneralErrors.ValueIsRequired(nameof(orderId));

            if (location == null)
                return LocationErrors.LocationNotSpecified();

            if (volume <= 0)
                return GeneralErrors.ValueIsInvalid(nameof(volume));

            Order createdOrder = new Order(orderId, location, volume);
            createdOrder.Status = OrderStatus.Created;

            return createdOrder;
        }

        public UnitResult<Error> Assign(Courier courier)
        {
            if (courier == null)
                return OrderErrors.CourierIsNeeded();

            if (this.Status == OrderStatus.Completed)
                return OrderErrors.OrderIsCompleted(this.Id);

            if (this.CourierId != null && this.CourierId != Guid.Empty)
                return OrderErrors.OrderIsAlreadyAssigned(this.CourierId);

            this.CourierId = courier.Id;
            this.Status = OrderStatus.Assigned;

            return UnitResult.Success<Error>();
        }

        public UnitResult<Error> Complete()
        {
            if (!this.CourierId.HasValue)
                return OrderErrors.OrderIsNotAssigned(this.Id);

            if (this.Status == OrderStatus.Completed)
                return OrderErrors.OrderIsCompleted(this.Id);

            this.Status = OrderStatus.Completed;

            return UnitResult.Success<Error>();
        }
    }
}
