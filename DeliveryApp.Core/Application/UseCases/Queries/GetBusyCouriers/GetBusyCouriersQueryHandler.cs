using CSharpFunctionalExtensions;
using Dapper;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    public class GetBusyCouriersQueryHandler : IRequestHandler<GetBusyCouriersQuery, Maybe<GetBusyCouriersResponse>>
    {
        private readonly ICourierRepository _courierRepository;
        public GetBusyCouriersQueryHandler(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
        }


        public async Task<Maybe<GetBusyCouriersResponse>> Handle(GetBusyCouriersQuery request, CancellationToken cancellationToken)
        {
            var result = await _courierRepository.GetAllBusyCouriersAsync();

            if (result.Value.Count == 0)
                return null;

            return new GetBusyCouriersResponse(MapCouriers(result.Value));
        }

        private List<CourierDto> MapCouriers(List<Courier> result)
        {
            var couriers = new List<CourierDto>();
            foreach (var courier in result)
            {
                var locationDto = new LocationDto(courier.Location.X, courier.Location.Y);
                var courierDto = new CourierDto(courier.Id, courier.Name, locationDto);
                couriers.Add(courierDto);
            }

            return couriers;
        }
    }
}
