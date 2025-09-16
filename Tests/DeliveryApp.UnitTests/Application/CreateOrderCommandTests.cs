using CSharpFunctionalExtensions;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Primitives;
using System;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application
{
    public class CreateOrderCommandTests
    {
        private readonly IOrderRepository  _orderRepository = Substitute.For<IOrderRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        [Fact]
        public async Task CantCreateOrderCommandWithInvalidValue()
        {
            int invalidValue = 0;
            //arrange
            var orderId = Guid.NewGuid();
            _unitOfWork.SaveChangesAsync()
                .Returns(Task.FromResult(true));

            //act
            var createOrderCommandResult = CreateOrderCommand.Create(orderId, "random street", invalidValue);

            //Assert
            createOrderCommandResult.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task CantCreateOrderCommandWithInvalidOrderId()
        {
            int validValue = 5;
            //arrange
            var orderId = Guid.Empty;
            _unitOfWork.SaveChangesAsync()
                .Returns(Task.FromResult(true));
            _orderRepository.AddAsync(Arg.Any<Order>())
                .Returns(Task.FromResult(IncorrectOrder(orderId)));

            //act
            var createOrderCommandResult = CreateOrderCommand.Create(orderId, "random street", validValue);

            //Assert
            createOrderCommandResult.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task CanCreateOrder()
        {
            //arrange
            var orderId = Guid.NewGuid();
            _unitOfWork.SaveChangesAsync()
                .Returns(Task.FromResult(true));
            _orderRepository.AddAsync(Arg.Any<Order>())
                .Returns(Task.FromResult(CorrectOrder(orderId)));

            var createOrderCommandResult = CreateOrderCommand.Create(orderId, "random street", 6);
            createOrderCommandResult.IsSuccess.Should().BeTrue();
            var command = createOrderCommandResult.Value;

            var handler = new CreateOrderCommandHandler(_orderRepository, _unitOfWork);

            //act
            var result = await handler.Handle(command, new CancellationToken());

            //assert
            result.IsSuccess.Should().BeTrue();
            _orderRepository.Received(1);
        }

        [Fact]
        public async Task ThrowExceptionWhenRepositoryThrowsException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var validValue = 5;
            var createOrderCommandResult = CreateOrderCommand.Create(orderId, "random street", validValue);
            var command = createOrderCommandResult.Value;

            var location = Location.CreateRandom().Value;
            var order = Order.Create(orderId, location, validValue).Value;

            _orderRepository
                .When(x => x.AddAsync(Arg.Any<Order>()))
                .Do(x => throw new Exception("Database error"));

            var handler = new CreateOrderCommandHandler(_orderRepository, _unitOfWork);

            // Act
            var result = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenUnitOfWorkFails()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var validValue = 5;
            var createOrderCommandResult = CreateOrderCommand.Create(orderId, "random street", validValue);
            var command = createOrderCommandResult.Value;

            var location = Location.CreateRandom().Value;
            var order = Order.Create(orderId, location, validValue).Value;

            _orderRepository.AddAsync(Arg.Any<Order>()).Returns(Task.CompletedTask);
            _unitOfWork
                .When(x => x.SaveChangesAsync())
                .Do(x => throw new Exception("Unit of work error"));

            var handler = new CreateOrderCommandHandler(_orderRepository, _unitOfWork);

            // Act
            var result = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<Exception>();
        }

        private async Task<Maybe<Order>> CorrectOrder(Guid orderId)
        {
            var createOrderResult = Order.Create(orderId, Location.CreateRandom().Value, 5);
            createOrderResult.IsSuccess.Should().BeTrue();

            var order = createOrderResult.Value;
            return await Task.FromResult(order);
        }

        private async Task<Maybe<Order>> IncorrectOrder(Guid orderId)
        {
            var createOrderResult = Order.Create(orderId, Location.CreateRandom().Value, 0);
            createOrderResult.IsSuccess.Should().BeTrue();

            var order = createOrderResult.Value;
            return await Task.FromResult(order);
        }
    }
}
