namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers
{
    public class GetAllCouriersResponse
    {
        private GetAllCouriersResponse() { }
        public GetAllCouriersResponse(List<CourierDto> couriers)
        {
            Couriers.AddRange(couriers);
        }

        public List<CourierDto> Couriers { get; set; } = new();
    }

    public class CourierDto
    {
        private CourierDto() { }
        public CourierDto(Guid id, string name, LocationDto location, int speed)
        {
            Id = id;
            Name = name;
            Location = location;
            Speed = speed;
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

        /// <summary>
        ///     Скорость
        /// </summary>
        public int Speed { get; set; }
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
