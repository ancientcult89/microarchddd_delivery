using CSharpFunctionalExtensions;
using Dapper;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetBusyCouriers
{
    public class GetBusyCouriersQueryHandler : IRequestHandler<GetBusyCouriersQuery, Maybe<GetBusyCouriersResponse>>
    {
        private readonly string _connectionString;
        public GetBusyCouriersQueryHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }


        public async Task<Maybe<GetBusyCouriersResponse>> Handle(GetBusyCouriersQuery request, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT c.id, c.name, c.location_x, c.location_y, 
                    FROM public.couriers as c
                    JOIN public.storage_places sp on sp.courier_id = c.id
                    where sp.order_id is not null;"
            );

            if (result.AsList().Count == 0)
                return null;

            return new GetBusyCouriersResponse(MapCouriers(result));

        }

        private List<CourierDto> MapCouriers(dynamic result)
        {
            var couriers = new List<CourierDto>();
            foreach (var courier in result)
            {
                var locationDto = new LocationDto(result.location_x, result.location_y);
                var courierDto = new CourierDto(result.id, result.name, locationDto);
                couriers.Add(courierDto);
            }

            return couriers;
        }
    }
}
