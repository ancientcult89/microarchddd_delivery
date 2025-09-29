using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggrerate.DomainEvents;

public sealed record OrderCompletedDomainEvent(Order Order) : DomainEvent;
