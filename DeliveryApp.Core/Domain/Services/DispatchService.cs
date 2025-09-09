using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;


namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchService : IDispatchService
    {
        public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
        {
            if (order == null)
                return OrderErrors.OrderIsNotExists();
            if(couriers == null || couriers.Count <= 0)
                return Errors.CouriersIsNotExists();

            List<Courier> freeCouriers = couriers.Where(c => c.CanTakeOrder(order)).ToList();

            if(freeCouriers  == null || freeCouriers.Count <= 0)
                return Errors.FreeCourierIsNotExists();

            if (freeCouriers.Count == 1)
                return AssignOrderToCourier(order, freeCouriers.First());

            Courier fastestCourier = freeCouriers.First();

            freeCouriers.ForEach(c => {
                if (c.CalculateTimeToLocation(order.Location).Value < fastestCourier.CalculateTimeToLocation(order.Location).Value)
                    fastestCourier = c;
            });

            return AssignOrderToCourier(order, fastestCourier);
        }

        private Courier AssignOrderToCourier(Order order, Courier courier)
        {
            order.Assign(courier);
            courier.TakeOrder(order);

            return courier;
        }


        public static class Errors
        {
            public static Error CouriersIsNotExists()
            {
                return new Error("couriers.is.not.exists", "Couriers is not exists");
            }

            public static Error FreeCourierIsNotExists()
            {
                return new Error("free.courier.is.not.exists", "Free courier is not exists");
            }
        }
    }
}
