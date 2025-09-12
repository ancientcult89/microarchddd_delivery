using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface IOrderRepository : IRepository<Order>
    {
        /// <summary>
        ///     Добавить
        /// </summary>
        /// <param name="order">Заказ</param>
        /// <returns>Заказ</returns>
        Task AddAsync(Order order);

        /// <summary>
        ///     Обновить
        /// </summary>
        /// <param name="order">Заказ</param>
        void Update(Order order);

        /// <summary>
        ///     Получить
        /// </summary>
        /// <param name="orderId">Идентификатор</param>
        /// <returns>Заказ</returns>
        Task<Maybe<Order>> GetAsync(Guid orderId);

        /// <summary>
        ///     Получить 1 новый заказ
        /// </summary>
        /// <returns>Заказы</returns>
        Task<Maybe<Order>> GetFirstInCreatedStatusAsync();

        /// <summary>
        ///     Получить все назначенные заказы
        /// </summary>
        /// <returns>Заказы</returns>
        IEnumerable<Order> GetAllInAssignedStatus();

    }
}
