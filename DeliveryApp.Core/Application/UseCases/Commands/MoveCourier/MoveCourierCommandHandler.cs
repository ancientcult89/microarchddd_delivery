using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCourier
{
    public class MoveCourierCommandHandler : IRequestHandler<MoveCourierCommand, UnitResult<Error>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourierRepository _courierRepository;
        private readonly IDispatchService _dispatcherService;

        public MoveCourierCommandHandler(
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

        public async Task<UnitResult<Error>> Handle(MoveCourierCommand request, CancellationToken cancellationToken)
        {
            var allBusyCouriers = await _courierRepository.GetAllBusyCouriersAsync();

            if (allBusyCouriers.HasNoValue)
                return new Error("no.busy.couriers", "There are no busy couriers");

            foreach (var courier in allBusyCouriers.Value)
            {
                await MoveSingleCourierAsync(courier);
            }

            return UnitResult.Success<Error>();
        }

        public async Task<UnitResult<Error>> MoveSingleCourierAsync(Courier courier)
        {
            Guid courierOrderId = courier.StoragePlaces.Where(sp => sp.OrderId != null).First()?.Id ?? Guid.Empty;

            //если по какой-то причене в выборку занятых всё же попал незанятый (например,
            //в параллельной выборке его освободили, но выборка сработала до того как транзакцию закоммитили) - скипнем его
            if (courierOrderId == Guid.Empty)
                return UnitResult.Success<Error>();

            var processingOrderResult = await _orderRepository.GetAsync(courierOrderId);

            //аналогично могло быть, что в параллельном потоке заказ завершили, но не успели закоммитить,
            //в результате по курьеру была устаревшая информация о заказе
            if (processingOrderResult.HasNoValue)
                return UnitResult.Success<Error>();

            Order processingOrder = processingOrderResult.Value;

            courier.Move(processingOrder.Location);

            CompleteOrderIfItsDone(courier, processingOrder);

            _courierRepository.Update(courier);

            await _unitOfWork.SaveChangesAsync();

            return UnitResult.Success<Error>();
        }

        private void CompleteOrderIfItsDone(Courier courier, Order processingOrder)
        {
            if (courier.Location == processingOrder.Location)
            {
                courier.CompleteOrder(processingOrder);
                processingOrder.Complete();

                _orderRepository.Update(processingOrder);
            }
        }
    }
}
