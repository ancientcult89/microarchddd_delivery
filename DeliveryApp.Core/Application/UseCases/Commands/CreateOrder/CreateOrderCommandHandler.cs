using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) 
        { 
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<UnitResult<Error>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = Order.Create(request.OrderId, Location.CreateRandom().Value, request.Volume);
            if(newOrder.IsFailure) return newOrder.Error;

            await _orderRepository.AddAsync(newOrder.Value);
            await _unitOfWork.SaveChangesAsync();
            return UnitResult.Success<Error>();
        }
    }
}
