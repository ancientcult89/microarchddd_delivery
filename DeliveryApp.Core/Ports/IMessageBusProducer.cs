using DeliveryApp.Core.Domain.Model.OrderAggrerate.DomainEvents;

namespace DeliveryApp.Core.Ports
{
    public interface IMessageBusProducer
    {
        Task PublishOrderStatusCompletedDomainEvent(OrderCompletedDomainEvent notification, CancellationToken cancellationToken);
        Task PublishOrderStatusCreatedDomainEvent(OrderCreatedDomainEvent notification, CancellationToken cancellationToken);
    }
}
