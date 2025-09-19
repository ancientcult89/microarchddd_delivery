using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers
{
    public class GetAllCouriersQueryHandler : IRequestHandler<GetAllCouriersQuery, Maybe<GetAllCouriersResponse>>
    {
        private readonly ICourierRepository _courierRepository;
        public GetAllCouriersQueryHandler(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
        }

        public async Task<Maybe<GetAllCouriersResponse>> Handle(GetAllCouriersQuery request, CancellationToken cancellationToken)
        {
            var result = await _courierRepository.GetAllCouriersAsync();

            if (result.Value.Count == 0)
                return null;

            return new GetAllCouriersResponse(MapCouriers(result.Value));
        }

        private List<CourierDto> MapCouriers(List<Courier> result)
        {
            var couriers = new List<CourierDto>();
            foreach (var courier in result)
            {
                var locationDto = new LocationDto(courier.Location.X, courier.Location.Y);
                var courierDto = new CourierDto(courier.Id, courier.Name, locationDto, courier.Speed);
                couriers.Add(courierDto);
            }

            return couriers;
        }
    }
}
