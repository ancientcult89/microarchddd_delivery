using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using System;
using System.Xml.Linq;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate
{
    public class OrderShould
    {
        [Fact]
        public void BeSuccessWhenParamsAreCorrect()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var location = Location.Create(5, 5).Value;
            var volume = 10;

            // Act
            var result = Order.Create(orderId, location, volume);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(orderId);
            result.Value.Location.Should().Be(location);
            result.Value.Volume.Should().Be(volume);
            result.Value.Status.Should().Be(OrderStatus.Created);
            result.Value.CourierId.Should().BeNull();
        }

        [Fact]
        public void ReturnCreateErrorCreateWithEmptyOrderId()
        {
            // Arrange
            var emptyOrderId = Guid.Empty;
            var location = Location.Create(5, 5).Value;
            var volume = 10;

            // Act
            var result = Order.Create(emptyOrderId, location, volume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("value.is.required");
            result.Error.Message.Should().Contain("Value is required for");
        }

        [Fact]
        public void ReturnCreateErrorWhenLocationIsNull()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            Location location = null;
            var volume = 10;

            // Act
            var result = Order.Create(orderId, location, volume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(LocationErrors.LocationNotSpecified());
            result.Error.Code.Should().Be("location.not.specified");
            result.Error.Message.Should().Contain("Location not specified");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(-90)]
        public void ReturnCreateErrorWhenVolumeIsInvalid(int invalidVolume)
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var location = Location.Create(5, 5).Value;

            // Act
            var result = Order.Create(orderId, location, invalidVolume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("value.is.invalid");
            result.Error.Message.Should().Contain("Value is invalid for");
        }

        [Fact]
        public void SuccessAssigningWhenOrderIsCreatedAndCourierIsValid()
        {
            // Arrange
            var order = CreateTestOrder();
            var courier = CreateTestCourier();

            // Act
            var result = order.Assign(courier);

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.CourierId.Should().Be(courier.Id);
            order.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public void FailAssigningWhenCourierIsNull()
        {
            // Arrange
            var order = CreateTestOrder();
            Courier courier = null;

            // Act
            var result = order.Assign(courier);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("courier.is.needed");
            result.Error.Message.Should().Contain("Courier is needed");
            order.CourierId.Should().BeNull();
            order.Status.Should().Be(OrderStatus.Created);
        }

        [Fact]
        public void FailAssigningWhenOrderIsAlreadyAssigned()
        {
            // Arrange
            var order = CreateTestOrder();
            var firstCourier = CreateTestCourier();
            order.Assign(firstCourier);

            var secondCourier = CreateTestCourier();

            // Act
            var result = order.Assign(secondCourier);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("Value is already assigned for");
            result.Error.Code.Should().Be("order.is.already.assigned");
            order.CourierId.Should().Be(firstCourier.Id);
            order.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public void FailAssigningWhenOrderIsCompleted()
        {
            // Arrange
            var order = CreateTestOrder();
            var courier = CreateTestCourier();
            order.Assign(courier);
            order.Complete();
            

            var anotherCourier = CreateTestCourier();

            // Act
            var result = order.Assign(anotherCourier);

            // Assert
            result.IsSuccess.Should().BeFalse();             
            result.Error.Message.Should().Contain("is already completed");
            result.Error.Code.Should().Be("order.is.completed");
            order.CourierId.Should().Be(courier.Id);
            order.Status.Should().Be(OrderStatus.Completed);
        }

        [Fact]
        public void CompletingOrderBeSuccessWhenOrderIsAssigned()
        {
            // Arrange
            var order = CreateTestOrder();
            var courier = CreateTestCourier();
            order.Assign(courier);

            // Act
            var result = order.Complete();

            // Assert
            result.IsSuccess.Should().BeTrue();
            order.Status.Should().Be(OrderStatus.Completed);
            order.CourierId.Should().Be(courier.Id);
        }

        [Fact]
        public void CompletingOrderReturnWhenOrderIsNotAssigned()
        {
            // Arrange
            var order = CreateTestOrder();

            // Act
            var result = order.Complete();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("is not assigned. Its cant complete");
            result.Error.Code.Should().Be("order.is.not.assigned");
            order.Status.Should().Be(OrderStatus.Created);
            order.CourierId.Should().BeNull();
        }

        [Fact]
        public void CompletingOrderReturnErrorWhenOrderIsAlreadyCompleted()
        {
            // Arrange
            var order = CreateTestOrder();
            var courier = CreateTestCourier();
            order.Assign(courier);
            order.Complete();

            // Act
            var result = order.Complete();

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("is already completed");
            result.Error.Code.Should().Be("order.is.completed");
            order.Status.Should().Be(OrderStatus.Completed);
            order.CourierId.Should().Be(courier.Id);
        }

        private Order CreateTestOrder()
        {
            var orderId = Guid.NewGuid();
            var location = Location.Create(5, 5).Value;
            var volume = 10;

            return Order.Create(orderId, location, volume).Value;
        }

        private Courier CreateTestCourier()
        {
            var courierId = Guid.NewGuid();
            var name = $"Test Courier {courierId}";
            var speed = 2;
            var location = Location.CreateRandom().Value;

            return Courier.Create(name, speed, location).Value;
        }
    }
}