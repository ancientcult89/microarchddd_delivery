using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services
{
    public class DispatcherServiceShould
    {
        private readonly Random _rand = new Random();

        [Fact]
        public void ReturnNearestFreeCourier()
        {
            //Arrange
            DispatchService dispatchService = new DispatchService();
            List<Courier> couriers = new List<Courier>();
            Courier nearestCourier = Courier.Create("Nearest courier", 2, Location.Create(2, 3).Value).Value;
            couriers.AddRange(GetBusyTestCourier());
            couriers.AddRange(new List<Courier>()
            {
                nearestCourier,
                Courier.Create("Far distance courier", 4, Location.Create(10, 10).Value).Value,
                Courier.Create("Middle distance courier", 3, Location.Create(5, 5).Value).Value
            });

            Order order = Order.Create(Guid.NewGuid(), Location.Create(2,2).Value, 4).Value;


            //Act
            Courier assignedCourier = dispatchService.Dispatch(order, couriers).Value;

            //Assert
            assignedCourier.Should().Be(nearestCourier);
            assignedCourier.CanTakeOrder(order).Should().BeFalse();
            order.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public void ReturnErrorWhenFreeCouriersIsNotExists()
        {
            //Arrange
            DispatchService dispatchService = new DispatchService();
            List<Courier> couriers = new List<Courier>();
            couriers.AddRange(GetBusyTestCourier());

            Order order = Order.Create(Guid.NewGuid(), Location.Create(2, 2).Value, 4).Value;


            //Act
            var assignedCourier = dispatchService.Dispatch(order, couriers);

            //Assert
            assignedCourier.IsFailure.Should().BeTrue();
            assignedCourier.Error.Message.Should().Contain("Free courier is not exists");
            assignedCourier.Error.Code.Should().Be("free.courier.is.not.exists");
            order.Status.Should().Be(OrderStatus.Created);
        }

        [Fact]
        public void ReturnErrorWhenOrderIsNotExists()
        {
            //Arrange
            DispatchService dispatchService = new DispatchService();
            List<Courier> couriers = new List<Courier>();
            couriers.AddRange(GetBusyTestCourier());

            Order order = null;

            //Act
            var assignedCourier = dispatchService.Dispatch(order, couriers);

            //Assert
            assignedCourier.IsFailure.Should().BeTrue();
            assignedCourier.Error.Message.Should().Contain("Order is not exists");
            assignedCourier.Error.Code.Should().Be("order.is.not.exists");
        }

        [Fact]
        public void ReturnErrorWhenCouriersIsNotExists()
        {
            //Arrange
            DispatchService dispatchService = new DispatchService();
            List<Courier> couriers = new List<Courier>();
            Order order = GetRandomOrder();

            List<Courier> couriers2 = null;
            Order order2 = GetRandomOrder();

            //Act
            var assignedCourier = dispatchService.Dispatch(order, couriers);
            var assignedCourier2 = dispatchService.Dispatch(order2, couriers2);

            //Assert
            assignedCourier.IsFailure.Should().BeTrue();
            assignedCourier.Error.Message.Should().Contain("Couriers is not exists");
            assignedCourier.Error.Code.Should().Be("couriers.is.not.exists");

            assignedCourier2.IsFailure.Should().BeTrue();
            assignedCourier2.Error.Message.Should().Contain("Couriers is not exists");
            assignedCourier2.Error.Code.Should().Be("couriers.is.not.exists");
        }


        private List<Courier> GetBusyTestCourier()
        {
            List<Courier> couriers = new List<Courier>();
            Courier busyCourier1 = Courier.Create("Test busy courier 1", 2, Location.CreateRandom().Value).Value;
            Order assignedOrder1 = GetRandomOrder();
            busyCourier1.TakeOrder(assignedOrder1);
            assignedOrder1.Assign(busyCourier1);
            couriers.Add(busyCourier1);

            Courier busyCourier2 = Courier.Create("Test busy courier 2", 2, Location.CreateRandom().Value).Value;
            Order assignedOrder2 = GetRandomOrder();
            busyCourier2.TakeOrder(assignedOrder2);
            assignedOrder2.Assign(busyCourier2);
            couriers.Add(busyCourier2);

            return couriers;
        }

        private Order GetRandomOrder()
        {
            return Order.Create(Guid.NewGuid(), Location.CreateRandom().Value, _rand.Next(1, 11)).Value;
        }
    }
}
