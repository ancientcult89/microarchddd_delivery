namespace DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders
{
    public class GetNotCompletedOrdersResponse
    {
        public GetNotCompletedOrdersResponse(List<OrderDto> orders)
        {
            Orders.AddRange(orders);
        }

        public List<OrderDto> Orders { get; set; } = new();
    }

    public class OrderDto
    {
        private OrderDto(){ }

        public OrderDto(Guid orderId, LocationDto location)
        {
            Id = orderId;
            Location = location;
        }
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Геопозиция (X,Y)
        /// </summary>
        public LocationDto Location { get; set; }
    }

    public class LocationDto
    {
        private LocationDto() { }

        public LocationDto(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        /// <summary>
        ///     Горизонталь
        /// </summary>
        public int X { get; set; }

        /// <summary>
        ///     Вертикаль
        /// </summary>
        public int Y { get; set; }
    }
}
