using DeliveryApp.Core.Domain.Model.OrderAggrerate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.DomainEventHandlers.OrderEventHandlers
{
    public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly IMessageBusProducer _messageBusProducer;

        public OrderCreatedDomainEventHandler(IMessageBusProducer messageBusProducer)
        {
            _messageBusProducer = messageBusProducer;
        }

        public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _messageBusProducer.PublishOrderStatusCreatedDomainEvent(notification, cancellationToken);
        }
    }
}
