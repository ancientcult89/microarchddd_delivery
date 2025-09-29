using CSharpFunctionalExtensions;
using Dapper;
using DeliveryApp.Core.Domain.Model.OrderAggrerate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders
{
    public class GetNotCompletedOrdersQueryHandler : IRequestHandler<GetNotCompletedOrdersQuery, Maybe<GetNotCompletedOrdersResponse>>
    {
        private readonly string _connectionString;
        public GetNotCompletedOrdersQueryHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }
        public async Task<Maybe<GetNotCompletedOrdersResponse>> Handle(GetNotCompletedOrdersQuery request, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var result = await connection.QueryAsync<dynamic>(
                @"SELECT o.id, o.location_x, o.location_y
                    FROM public.orders as o
                    where o.status != @completedStatus;"
            , new { completedStatus = OrderStatus.Completed.Name });

            if (result.AsList().Count == 0)
                return null;

            return new GetNotCompletedOrdersResponse(MapOrders(result));
        }

        private List<OrderDto> MapOrders(dynamic result)
        {
            var orders = new List<OrderDto>();
            foreach (var order in result)
            {
                var locationDto = new LocationDto(order.location_x, order.location_y);
                var orderDto = new OrderDto(order.id, locationDto);
                orders.Add(orderDto);
            }

            return orders;
        }
    }
}
