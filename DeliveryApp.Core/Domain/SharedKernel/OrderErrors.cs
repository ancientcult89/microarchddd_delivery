using Primitives;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    internal class OrderErrors
    {
        public static Error OrderIsAlreadyAssigned(Guid? courierId)
        {
            if (!courierId.HasValue) throw new ArgumentException(courierId.ToString());
            return new Error("order.is.already.assigned", $"Value is already assigned for {courierId.ToString()}");
        }

        public static Error CourierIsNeeded()
        {
            return new Error("courier.is.needed", $"Courier is needed");
        }

        public static Error OrderIsNotAssigned(Guid orderId)
        {
            if (orderId == Guid.Empty) throw new ArgumentException(orderId.ToString());
            return new Error("order.is.not.assigned", $"Order {orderId.ToString()} is not assigned. Its cant complete");
        }

        public static Error OrderIsCompleted(Guid orderId)
        {
            if (orderId == Guid.Empty) throw new ArgumentException(orderId.ToString());
            return new Error("order.is.completed", $"Order {orderId.ToString()} is already completed");
        }

        public static Error OrderIsNotExists()
        {
            return new Error("order.is.not.exists", $"Order is not exists");
        }
    }
}
