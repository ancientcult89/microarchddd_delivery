using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;

namespace DeliveryApp.Core.Ports
{
    public interface ICourierRepository
    {
        /// <summary>
        ///     Добавить
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>Курьер</returns>
        Task AddAsync(Courier courier);

        /// <summary>
        ///     Обновить
        /// </summary>
        /// <param name="courier">Курьер</param>
        void Update(Courier courier);

        /// <summary>
        ///     Получить
        /// </summary>
        /// <param name="courierId">Идентификатор</param>
        /// <returns>Курьер</returns>
        Task<Maybe<Courier>> GetAsync(Guid courierId);

        /// <summary>
        ///  Получить всех незанятых
        /// </summary>
        /// <returns>Курьеры</returns>
        Task<Maybe<List<Courier>>> GetAllFreeCouriersAsync();

        /// <summary>
        /// возвращает всех занятых курьеров
        /// </summary>
        /// <returns></returns>
        Task<Maybe<List<Courier>>> GetAllBusyCouriersAsync();

        /// <summary>
        /// Возвращает всех курьеров, не зависимо от их занятости
        /// </summary>
        /// <returns></returns>
        Task<Maybe<List<Courier>>> GetAllCouriersAsync();
    }
}
