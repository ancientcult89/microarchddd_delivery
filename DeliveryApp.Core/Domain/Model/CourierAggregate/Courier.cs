using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    public class Courier : Aggregate<Guid>
    {
        public string Name { get; private set; }
        public int Speed { get; private set; }
        public Location Location { get; private set; }
        public List<StoragePlace> StoragePlaces { get; private set; } = new List<StoragePlace>();
        private Courier() { }
        private Courier(string name, int speed, Location location) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Speed = speed;
            Location = location;

            StoragePlaces.Add(StoragePlace.Create("Сумка", 10).Value);
        }

        public static Result<Courier, Error> Create(string name, int speed, Location location)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));

            if (speed <= 0)
                return GeneralErrors.ValueIsRequired(nameof(speed));

            if (location == null)
                return LocationErrors.LocationNotSpecified();

            return new Courier(name, speed, location);
        }

        public UnitResult<Error> AddStoragePlace(string name, int volume)
        {
            var newStoragePlace = StoragePlace.Create(name, volume);

            if (!newStoragePlace.IsSuccess)
                return Errors.CantAddStorage(newStoragePlace.Error.Message);

            this.StoragePlaces.Add(newStoragePlace.Value);

            return UnitResult.Success<Error>();
        }

        public bool CanTakeOrder(Order order)
        {
            if (order == null)
                return false;

            return this.StoragePlaces.Exists(sp => sp.CanStore(order.Volume).Value);
        }

        public UnitResult<Error> TakeOrder(Order order)
        {
            if (order == null)
                return Errors.OrderIsNotSpecified();
            if (!CanTakeOrder(order))
                return Errors.CantTakeOrder(this.Id, order.Id);
            if (this.StoragePlaces.Count == 0)
                return Errors.StoragesAreNotSpecified();

            StoragePlace freeStoragePlace = StoragePlaces.Where(sp => sp.CanStore(order.Volume).Value).First();
            freeStoragePlace.Store(order.Id, order.Volume);
            return UnitResult.Success<Error>();
        }

        public UnitResult<Error> CompleteOrder(Order order)
        {
            if (order == null)
                return Errors.OrderIsNotSpecified();

            StoragePlace processingOrdersStorage = this.StoragePlaces.Where(sp => sp.OrderId == order.Id).FirstOrDefault();
            if (processingOrdersStorage == null)
                return Errors.NoStorageWithSuchOrder();

            var clearedOrderStorage = processingOrdersStorage.Clear(order.Id);
            order.Complete();

            return Result.Success<Error>();
        }

        public Result<double, Error> CalculateTimeToLocation(Location location)
        {
            if (location == null)
                return LocationErrors.LocationNotSpecified();

            return this.Location.DistanceTo(location).Value / this.Speed;
        }

        public UnitResult<Error> Move(Location target)
        {
            if (target == null) return GeneralErrors.ValueIsRequired(nameof(target));

            var difX = target.X - Location.X;
            var difY = target.Y - Location.Y;
            var cruisingRange = Speed;

            var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
            cruisingRange -= Math.Abs(moveX);

            var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);

            var locationCreateResult = Location.Create(Location.X + moveX, Location.Y + moveY);
            if (locationCreateResult.IsFailure) return locationCreateResult.Error;
            this.Location = locationCreateResult.Value;

            return UnitResult.Success<Error>();

        }

        public static class Errors
        {
            public static Error CantTakeOrder(Guid courierId, Guid orderId)
            {
                if (courierId == Guid.Empty) throw new ArgumentException(courierId.ToString());
                if (orderId == Guid.Empty) throw new ArgumentException(orderId.ToString());
                return new Error("cant.take.order", $"The courier {courierId.ToString()} have not enough storage space");
            }

            public static Error CantAddStorage(string message)
            {
                return new Error("cant.add.storage", message);
            }

            public static Error OrderIsNotSpecified()
            {
                return new Error("order.is.not.specified", "Order is not specified");
            }

            public static Error StoragesAreNotSpecified()
            {
                return new Error("ctorages.are.not.specified", "Storages are not specified");
            }

            public static Error NoStorageWithSuchOrder()
            {
                return new Error("no.storage.with.such.order", "There is no storage place with such Order");
            }
        }
    }
}
