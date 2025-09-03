using DeliveryApp.Core.Domain.Model.CourierAggregate;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class StoragePlaceShould
    {
        [Fact]
        public void BeCorrectWhenParamsIsCorrect()
        {
            // Arrange
            var name = "Test Storage";
            var totalVolume = 100;

            // Act
            var result = StoragePlace.Create(name, totalVolume);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be(name);
            result.Value.TotalVolume.Should().Be(totalVolume);
            result.Value.OrderId.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ReturnErrorWhenNameIsIncorrect(string invalidName)
        {
            // Arrange
            var totalVolume = 100;

            // Act
            var result = StoragePlace.Create(invalidName, totalVolume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("Value is invalid for name");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void ReturnErrorWhenTotalVolumeIsIncorrect(int invalidTotalVolume)
        {
            // Arrange
            var name = "Test Storage";

            // Act
            var result = StoragePlace.Create(name, invalidTotalVolume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("Value is required for volume");
        }

        [Fact]
        public void CanStoreReturnTrueWhenTotalVolumeIsCorrect()
        {
            // Arrange
            int totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;

            var incomingVolime = 50;

            // Act
            var result = storage.CanStore(incomingVolime);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public void CanStoreReturnFalseWhenIncomingVolumeIsMoreThanTotalVolume()
        {
            // Arrange
            int totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;
            var incomingVolume = 150;

            // Act
            var result = storage.CanStore(incomingVolume);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();
        }

        [Fact]
        public void CanStoreReturnFalseIfItAlreadyHaveOrder()
        {
            // Arrange
            var storage = StoragePlace.Create("Test", 100).Value;
            int firstStoredVolume = 50;
            storage.Store(Guid.NewGuid(), firstStoredVolume);

            int secondStoredVolume = 50;
            // Act
            var result = storage.CanStore(secondStoredVolume);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void CanStoreReturnFalseWhenVolumeIsInCorrect(int invalidVolume)
        {
            // Arrange
            var totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;

            // Act
            var result = storage.CanStore(invalidVolume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("Value is required for volume");
        }

        [Fact]
        public void StoreOrderWithCorrectParameters()
        {
            // Arrange
            var totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;
            var orderId = Guid.NewGuid();
            var incomingVolume = 50;

            // Act
            var result = storage.Store(orderId, incomingVolume);

            // Assert
            result.IsSuccess.Should().BeTrue();
            storage.OrderId.Should().Be(orderId);
        }

        [Fact]
        public void ReturnErrorWhenItStoreOrderWithInvalidVolume()
        {
            // Arrange
            int totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;
            var orderId = Guid.NewGuid();
            var invalidVolume = 150;

            // Act
            var result = storage.Store(orderId, invalidVolume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            storage.OrderId.Should().BeNull();
        }

        [Fact]
        public void ReturnErrorWhenItTryToStoreOccupiedStoragePlace()
        {
            // Arrange
            int totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;
            var firstOrderId = Guid.NewGuid();
            int firstIncomingVolume = 50;
            storage.Store(firstOrderId, firstIncomingVolume);

            var secondOrderId = Guid.NewGuid();
            var secondIncomingVolume = 30;

            // Act
            var result = storage.Store(secondOrderId, secondIncomingVolume);

            // Assert
            result.IsSuccess.Should().BeFalse();
            storage.OrderId.Should().Be(firstOrderId);
        }

        [Fact]
        public void ClearWhenOrderIsCorrect()
        {
            // Arrange
            var storage = StoragePlace.Create("Test", 100).Value;
            var orderId = Guid.NewGuid();
            int incomingVolume = 50;
            storage.Store(orderId, incomingVolume);

            // Act
            var result = storage.Clear(orderId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            storage.OrderId.Should().BeNull();
        }

        [Fact]
        public void ClearSuccessWhenOrderIsCorrect()
        {
            // Arrange
            int totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;
            var orderId = Guid.NewGuid();

            // Act
            var result = storage.Clear(orderId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            storage.OrderId.Should().BeNull();
        }

        [Fact]
        public void ClearReturnErrorWhenOrderIsIncorrect()
        {
            // Arrange
            int totalVolume = 100;
            var storage = StoragePlace.Create("Test", totalVolume).Value;
            var storedOrderId = Guid.NewGuid();
            var differentOrderId = Guid.NewGuid();
            int incomingVolume = 50;
            storage.Store(storedOrderId, incomingVolume);

            // Act
            var result = storage.Clear(differentOrderId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("Value is invalid for orderId");
            storage.OrderId.Should().Be(storedOrderId);
        }

        [Fact]
        public void ClearReturnErrorWhenOrderIsNotExists()
        {
            // Arrange
            var storage = StoragePlace.Create("Test", 100).Value;
            var storedOrderId = Guid.NewGuid();

            // Act
            var result = storage.Clear(storedOrderId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Message.Should().Contain("Value is invalid for orderId");
            storage.OrderId.Should().BeNull();
        }
    }
}
