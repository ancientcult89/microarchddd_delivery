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
        private readonly IGeoClient _geoClient;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IGeoClient geoClient) 
        { 
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _geoClient = geoClient;
        }
        public async Task<UnitResult<Error>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var getLocationResult = await _geoClient.GetLocationAsync(request.Street, cancellationToken);

            if (getLocationResult.IsFailure) 
                return getLocationResult.Error;

            Location locationFromGeo = getLocationResult.Value;

            var newOrder = Order.Create(request.OrderId, locationFromGeo, request.Volume);

            if(newOrder.IsFailure) 
                return newOrder.Error;

            await _orderRepository.AddAsync(newOrder.Value);
            await _unitOfWork.SaveChangesAsync();
            return UnitResult.Success<Error>();
        }
    }
}
