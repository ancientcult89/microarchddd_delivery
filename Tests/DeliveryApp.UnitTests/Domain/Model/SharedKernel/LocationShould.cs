using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using NSubstitute.ExceptionExtensions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel
{
    public class LocationShould
    {

        [Fact]
        public void BeCorrectWhenParamsIsCorrectOnCreate()
        {
            //Arrange

            //Act
            var location = Location.Create(1, 5);

            //Assert
            location.IsSuccess.Should().BeTrue();
            location.Value.X.Should().Be(1);
            location.Value.Y.Should().Be(5);
        }

        [Fact]
        public void BeEqualSameLocation()
        {
            //Arrange

            //Act
            var firstLocation = Location.Create(1, 5);
            var secondLocation = Location.Create(1, 5);

            //Assert
            firstLocation.Value.Should().Be(secondLocation.Value);
        }

        [Fact]
        public void BeNotEqualDifferentLocation()
        {
            //Arrange

            //Act
            var firstLocation = Location.Create(1, 5);
            var secondLocation = Location.Create(1, 6);

            //Assert
            firstLocation.Value.Should().NotBe(secondLocation.Value);
        }

        [Theory]
        [InlineData(11, 1)]
        [InlineData(0, 10)]
        [InlineData(-1, 3)]
        public void ReturnErrorWhenXParamIsIncorrectAndYParamIsCorrectOnCreated(int x, int y)
        {
            //Arrange
            string errorString = "X should be between 1 and 10";

            //Act
            var location =  Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeFalse();
            location.Error.Message.Should().Be(errorString);
        }

        [Theory]
        [InlineData(10, 0)]
        [InlineData(4, -8)]
        [InlineData(2, 11)]

        public void ReturnErrorWhenYParamIsIncorrectAndXParamIsCorrectOnCreated(int x, int y)
        {
            //Arrange
            string errorString = "Y should be between 1 and 10";

            //Act
            var location = Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeFalse();
            location.Error.Message.Should().Be(errorString);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(11, 11)]
        [InlineData(-1, -1)]
        [InlineData(-2, 11)]
        [InlineData(11, -5)]
        public void ReturnErrorWhenParamsIsIncorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            var location = Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeFalse();
            location.Error.Should().NotBeNull();
        }

        [Theory]
        [InlineData(2, 6, 4, 9, 5)]

        public void ReturnCorrectStepsToDestionationAndBackward(int x1, int y1, int x2, int y2, int expectedDistance)
        {
            //Arrange

            //Act
            var startLocation = Location.Create(x1, y1);
            var destination = Location.Create(x2, y2);

            var backwardStartLocation = Location.Create(x2, y2);
            var backwardDestination = Location.Create(x1, y1);

            //Assert
            startLocation.Value.DistanceTo(destination.Value).Should().Be(expectedDistance);
            backwardStartLocation.Value.DistanceTo(backwardDestination.Value).Should().Be(expectedDistance);
        }


        [Fact]
        public void BeCorrectWhenItsRandom()
        {
            //Arrange

            //Act
            var location = Location.CreateRandom();

            //Assert
            location.Should().NotBeNull();
        }

        [Fact]
        public void BeCorrectWhenItsMultipleRandom()
        {
            for (int i = 0; i < 100; i++)
            {
                var location = Location.CreateRandom();

                location.Should().NotBeNull();
            }
        }
    }
}
