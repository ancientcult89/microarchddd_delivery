using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<UnitResult<Error>>
    {
        private CreateOrderCommand(Guid orderId, string street, int volume)
        {
            OrderId = orderId;
            Street = street;
            Volume = volume;
        }
        public static Result<CreateOrderCommand, Error> Create(Guid orderId, string street, int volume)
        {
            if(orderId == Guid.Empty) return GeneralErrors.ValueIsRequired(nameof(orderId));
            if(string.IsNullOrWhiteSpace(street)) return GeneralErrors.ValueIsRequired(nameof(street));
            if(volume <= 0) return GeneralErrors.ValueIsRequired(nameof(volume));

            return new CreateOrderCommand(orderId, street, volume);
        }

        /// <summary>
        ///     Идентификатор заказа
        /// </summary>
        public Guid OrderId { get; }

        /// <summary>
        ///     Улица
        /// </summary>
        /// <remarks>Корзина содержала полный Address, но для упрощения мы будем использовать только Street из Address</remarks>
        public string Street { get; }

        /// <summary>
        ///     Объем
        /// </summary>
        public int Volume { get; }
    }
}
