using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate
{
    public class StoragePlace : Entity<Guid>
    {
        public string Name { get; private set; }
        public int TotalVolume { get; private set; }
        public Guid? OrderId { get; private set; }

        private StoragePlace() { }

        private StoragePlace(string name, int volume)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.TotalVolume = volume;
        }

        public static Result<StoragePlace, Error> Create(string name, int volume)
        {
            if (string.IsNullOrWhiteSpace(name))
                return GeneralErrors.ValueIsInvalid(nameof(name));

            if (volume <= 0)
                return GeneralErrors.ValueIsRequired(nameof(volume));

            return new StoragePlace(name, volume);
        }

        public Result<bool, Error> CanStore(int volume)
        {
            if (volume <= 0)
                return GeneralErrors.ValueIsRequired(nameof(volume));

            if (IsOccupied())
                return false;

            return volume <= this.TotalVolume;
        }

        public UnitResult<Error> Store(Guid orderId, int volume)
        {
            if (!CanStore(volume).Value)
                return GeneralErrors.ValueIsInvalid(nameof(volume));

            this.OrderId = orderId;
            return UnitResult.Success<Error>();
        }
        public UnitResult<Error> Clear(Guid orderId)
        {
            if (!this.OrderId.HasValue)
                return GeneralErrors.ValueIsInvalid(nameof(orderId));
            if (this.OrderId != orderId)
                return GeneralErrors.ValueIsInvalid(nameof(orderId));

            this.OrderId = null;
            return UnitResult.Success<Error>();
        }

        private bool IsOccupied() =>
            OrderId.HasValue;
    }
}
