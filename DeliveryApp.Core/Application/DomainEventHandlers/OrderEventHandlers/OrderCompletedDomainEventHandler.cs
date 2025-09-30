using DeliveryApp.Core.Domain.Model.OrderAggrerate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers.OrderEventHandlers
{
    public class OrderCompletedDomainEventHandler : INotificationHandler<OrderCompletedDomainEvent>
    {
        private readonly IMessageBusProducer _messageBusProducer;

        public OrderCompletedDomainEventHandler(IMessageBusProducer messageBusProducer)
        {
            _messageBusProducer = messageBusProducer;
        }

        public async Task Handle(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _messageBusProducer.PublishOrderStatusCompletedDomainEvent(notification, cancellationToken);
        }
    }
}
