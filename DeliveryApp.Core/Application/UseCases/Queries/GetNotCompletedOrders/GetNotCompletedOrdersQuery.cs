using CSharpFunctionalExtensions;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders
{
    public class GetNotCompletedOrdersQuery : IRequest<Maybe<GetNotCompletedOrdersResponse>>
    {
    }
}
