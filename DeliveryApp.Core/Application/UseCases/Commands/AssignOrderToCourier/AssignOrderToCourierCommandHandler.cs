using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier
{
    public class AssignOrderToCourierCommandHandler : IRequestHandler<AssignOrderToCourierCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourierRepository _courierRepository;
        private readonly IDispatchService _dispatcherService;
        public AssignOrderToCourierCommandHandler(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ICourierRepository courierRepository,
            IDispatchService dispatchService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _courierRepository = courierRepository;
            _dispatcherService = dispatchService;
        }
        public async Task<UnitResult<Error>> Handle(AssignOrderToCourierCommand request, CancellationToken cancellationToken)
        {
            var freeCouriers = await _courierRepository.GetAllFreeCouriersAsync();

            var firstCreatedOrders = await _orderRepository.GetFirstInCreatedStatusAsync();

            var dispatchResult = _dispatcherService.Dispatch(firstCreatedOrders.Value, freeCouriers.Value);

            if(!dispatchResult.IsSuccess)
                return dispatchResult.Error;

            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }
    }
}
