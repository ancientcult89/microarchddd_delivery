using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggrerate.DomainEvents;

public sealed record OrderCreatedDomainEvent(Order Order) : DomainEvent;

