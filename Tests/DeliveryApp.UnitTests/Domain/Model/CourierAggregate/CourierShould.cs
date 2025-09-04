using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierShould
    {

        [Fact]
        public void BeSuccessWhenParamsAreCorrect()
        {
            // Arrange
            var name = "Test Courier";
            var speed = 5;
            var location = Location.Create(1, 1).Value;

            // Act
            var result = Courier.Create(name, speed, location);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(name);
            result.Value.Speed.Should().Be(speed);
            result.Value.Location.Should().Be(location);
            result.Value.StoragePlaces.Should().HaveCount(1);
            result.Value.StoragePlaces[0].Name.Should().Be("Сумка");
            result.Value.StoragePlaces[0].TotalVolume.Should().Be(10);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ReturnErrorWhenNameIsInvalid(string invalidName)
        {
            // Arrange
            var speed = 5;
            var location = Location.Create(1, 1).Value;

            // Act
            var result = Courier.Create(invalidName, speed, location);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public void ReturnErrorWhenSpeedIsInvalid(int invalidSpeed)
        {
            // Arrange
            var name = "Test Courier";
            var location = Location.Create(1, 1).Value;

            // Act
            var result = Courier.Create(name, invalidSpeed, location);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ReturnErrorWhenLocationIsNull()
        {
            // Arrange
            var name = "Test Courier";
            var speed = 5;
            Location location = null;

            // Act
            var result = Courier.Create(name, speed, location);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(LocationErrors.LocationNotSpecified());
        }

        [Fact]
        public void SuccessAddingStoragePlaceWhenParamsAreCorrect()
        {
            // Arrange
            var courier = CreateTestCourier();
            var storageName = "Дополнительная сумка";
            var volume = 15;

            // Act
            var result = courier.AddStoragePlace(storageName, volume);

            // Assert
            result.IsSuccess.Should().BeTrue();
            courier.StoragePlaces.Should().HaveCount(2);
            courier.StoragePlaces[1].Name.Should().Be(storageName);
            courier.StoragePlaces[1].TotalVolume.Should().Be(volume);
        }

        [Fact]
        public void FailAddingStoragePlaceWhenStorageCreationFails()
        {
            // Arrange
            var courier = CreateTestCourier();
            var invalidStorageName = "";
            var volume = 15;

            // Act
            var result = courier.AddStoragePlace(invalidStorageName, volume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("cant.add.storage");
            courier.StoragePlaces.Should().HaveCount(1);
        }

        [Fact]
        public void CanTakeOrderReturnTrueWhenOrderCanBeStored()
        {
            // Arrange
            var courier = CreateTestCourier();
            var order = CreateTestOrder(volume: 5);

            // Act
            var result = courier.CanTakeOrder(order);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CanTakeOrderReturnFalseWhenOrderVolumeTooLarge()
        {
            // Arrange
            var courier = CreateTestCourier();
            var order = CreateTestOrder(volume: 20);

            // Act
            var result = courier.CanTakeOrder(order);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CanTakeOrderReturnFalseWhenOrderIsNull()
        {
            // Arrange
            var courier = CreateTestCourier();
            Order order = null;

            // Act
            var result = courier.CanTakeOrder(order);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TakeOrderBeSuccessWhenOrderCanBeTaken()
        {
            // Arrange
            var courier = CreateTestCourier();
            var order = CreateTestOrder(volume: 5);

            // Act
            var result = courier.TakeOrder(order);

            // Assert
            result.IsSuccess.Should().BeTrue();
            courier.StoragePlaces[0].OrderId.Should().Be(order.Id);
        }

        [Fact]
        public void TakeOrderReturnErrorWhenOrderIsNull()
        {
            // Arrange
            var courier = CreateTestCourier();
            Order order = null;

            // Act
            var result = courier.TakeOrder(order);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("order.is.not.specified");
            result.Error.Message.Should().Contain("Order is not specified");
        }

        [Fact]
        public void TakeOrderReturnErrorWhenOrderIsTooLarge()
        {
            // Arrange
            var courier = CreateTestCourier();
            var order = CreateTestOrder(volume: 20);

            // Act
            var result = courier.TakeOrder(order);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("cant.take.order");
            result.Error.Message.Should().Contain("have not enough storage space");
        }

        [Fact]
        public void CompleteOrderBeSuccessWhenOrderExistsInStorage()
        {
            // Arrange
            var courier = CreateTestCourier();
            var order = CreateTestOrder(volume: 5);
            courier.TakeOrder(order);
            order.Assign(courier);
            StoragePlace assignedStoragePlace = courier.StoragePlaces.Where(sp => sp.OrderId == order.Id).First();
            OrderStatus sourceOrderStatus = order.Status;

            // Act
            var result = courier.CompleteOrder(order);

            // Assert
            result.IsSuccess.Should().BeTrue();
            assignedStoragePlace.OrderId.Should().BeNull();
            order.Status.Should().Be(sourceOrderStatus);
        }

        [Fact]
        public void CompleteOrderReturnErrorWhenOrderIsNull()
        {
            // Arrange
            var courier = CreateTestCourier();
            Order order = null;

            // Act
            var result = courier.CompleteOrder(order);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("order.is.not.specified");
            result.Error.Message.Should().Contain("Order is not specified");
        }

        [Fact]
        public void CompleteOrderReturnErrorWhenOrderNotInStorage()
        {
            // Arrange
            var courier = CreateTestCourier();
            var order = CreateTestOrder(volume: 5);
            // Don't take the order

            // Act
            var result = courier.CompleteOrder(order);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("no.storage.with.such.order");
            result.Error.Message.Should().Contain("There is no storage place with such Order");
        }

        [Fact]
        public void CalculateTimeToLocationReturnCorrectTimeWhenLocationIsValid()
        {
            // Arrange
            var courier = CreateTestCourier();
            var targetLocation = Location.Create(5, 5).Value;
            var expectedDistance = courier.Location.DistanceTo(targetLocation).Value;
            var expectedTime = expectedDistance / courier.Speed;

            // Act
            var result = courier.CalculateTimeToLocation(targetLocation);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(expectedTime);
        }

        [Fact]
        public void CalculateTimeToLocationReturnErrorWhenLocationIsNull()
        {
            // Arrange
            var courier = CreateTestCourier();
            Location targetLocation = null;

            // Act
            var result = courier.CalculateTimeToLocation(targetLocation);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Code.Should().Be("location.not.specified");
            result.Error.Message.Should().Contain("Location not specified");
        }

        [Fact]
        public void MoveUpdateLocationWhenTargetIsReachable()
        {
            // Arrange
            var courier = CreateTestCourier(speed: 3);
            var initialLocation = courier.Location;
            var targetLocation = Location.Create(4, 4).Value;

            // Act
            var result = courier.Move(targetLocation);

            // Assert
            result.IsSuccess.Should().BeTrue();
            courier.Location.Should().NotBe(initialLocation);
            courier.Location.X.Should().BeInRange(1, 4);
            courier.Location.Y.Should().BeInRange(1, 4);
        }

        [Fact]
        public void MoveReturnErrorWhenTargetIsNull()
        {
            // Arrange
            var courier = CreateTestCourier();
            Location targetLocation = null;

            // Act
            var result = courier.Move(targetLocation);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("value.is.required");
            result.Error.Message.Should().Contain("Value is required for");
        }

        [Fact]
        public void MoveReachExactTargetWhenWithinSpeedRange()
        {
            // Arrange
            var courier = CreateTestCourier(speed: 10);
            var targetLocation = Location.Create(3, 4).Value;

            // Act
            var result = courier.Move(targetLocation);

            // Assert
            result.IsSuccess.Should().BeTrue();
            courier.Location.X.Should().Be(3);
            courier.Location.Y.Should().Be(4);
        }

        private Courier CreateTestCourier(string name = "Test Courier", int speed = 5)
        {
            var location = Location.Create(1, 1).Value;
            return Courier.Create(name, speed, location).Value;
        }

        private Order CreateTestOrder(int volume = 5)
        {
            var orderId = Guid.NewGuid();
            var location = Location.Create(2, 2).Value;
            return Order.Create(orderId, location, volume).Value;
        }
    }
}