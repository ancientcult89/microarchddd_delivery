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
            var location = new Location(1, 5);

            //Assert
            location.Should().NotBeNull();
            location.X.Should().Be(1);
            location.Y.Should().Be(5);
        }

        [Fact]
        public void BeEqualSameLocation()
        {
            //Arrange

            //Act
            var firstLocation = new Location(1, 5);
            var secondLocation = new Location(1, 5);

            //Assert
            firstLocation.Should().Be(secondLocation);
        }

        [Fact]
        public void BeNotEqualDifferentLocation()
        {
            //Arrange

            //Act
            var firstLocation = new Location(1, 5);
            var secondLocation = new Location(1, 6);

            //Assert
            firstLocation.Should().NotBe(secondLocation);
        }

        [Theory]
        [InlineData(11, 1)]
        [InlineData(0, 10)]
        [InlineData(-1, 3)]
        public void ThrowExcceptionWhenXParamIsIncorrectAndYParamIsCorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            Action act = () => new Location(x, y);

            //Assert
            act.Should().Throw<ArgumentException>().WithMessage("X should be between 1 and 10");
        }

        [Theory]
        [InlineData(10, 0)]
        [InlineData(4, -8)]
        [InlineData(2, 11)]

        public void ThrowExcceptionWhenYParamIsIncorrectAndXParamIsCorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            Action act = () => new Location(x, y);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(11, 11)]
        [InlineData(-1, -1)]
        [InlineData(-2, 11)]
        [InlineData(11, -5)]
        public void ThrowExcceptionWhenParamsIsIncorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            Action act = () => new Location(x, y);

            //Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(2, 6, 4, 9, 5)]

        public void ReturnCorrectStepsToDestionationAndBackward(int x1, int y1, int x2, int y2, int expectedDistance)
        {
            //Arrange

            //Act
            var startLocation = new Location(x1, y1);
            var destination = new Location(x2, y2);

            var backwardStartLocation = new Location(x2, y2);
            var backwardDestination = new Location(x1, y1);

            //Assert
            startLocation.DistanceTo(destination).Should().Be(expectedDistance);
            backwardStartLocation.DistanceTo(backwardDestination).Should().Be(expectedDistance);
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
