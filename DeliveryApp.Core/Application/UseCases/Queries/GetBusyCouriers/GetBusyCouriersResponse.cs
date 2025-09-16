namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    public class GetBusyCouriersResponse
    {
        private GetBusyCouriersResponse() { }
        public GetBusyCouriersResponse(List<CourierDto> couriers)
        {
            Couriers.AddRange(couriers);
        }

        public List<CourierDto> Couriers { get; set; } = new();
    }
    public class CourierDto
    {
        private CourierDto(){ }
        public CourierDto(Guid id, string name, LocationDto location)
        {
            Id = id;
            Name = name;
            Location = location;
        }
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Имя
        /// </summary>
        public string Name { get; set; }

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
