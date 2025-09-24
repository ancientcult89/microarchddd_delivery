using DeliveryApp.Core.Application.UseCases.Commands.CreateCourier;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetNotCompletedOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenApi.Controllers;
using OpenApi.Models;

namespace DeliveryApp.Api.Adapters.http
{
    public class DeliveryController : DefaultApiController
    {
        private readonly IMediator _mediator;
        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public override async Task<IActionResult> CreateCourier([FromBody] NewCourier newCourier)
        {
            var createCourierCommand = CreateCourierCommand.Create(newCourier.Name, newCourier.Speed);
            if (!createCourierCommand.IsSuccess)
                return Conflict();

            var response = await _mediator.Send(createCourierCommand.Value);

            if (response.IsSuccess)
                return Ok();

            return Conflict();
        }

        public override async Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();
            var street = "Несуществующая";
            var createOrderCommand = CreateOrderCommand.Create(orderId, street, 5).Value;
            var response = await _mediator.Send(createOrderCommand);

            if (response.IsSuccess) 
                return Ok();

            return Conflict(response.Error);
        }

        public override async Task<IActionResult> GetCouriers()
        {
            var getAllCouriersQuery = new GetAllCouriersQuery();
            var response = await _mediator.Send(getAllCouriersQuery);
            if (response.HasNoValue)
                return NotFound();

            return Ok(ToCouriersModelMapper(response.Value.Couriers));
        }

        public override async Task<IActionResult> GetOrders()
        {
            var getNotCompletedOrdersQuery = new GetNotCompletedOrdersQuery();
            var response = await _mediator.Send(getNotCompletedOrdersQuery);

            if (response.HasNoValue)
                return NotFound();

            return Ok(ToOrderModelMapper(response.Value.Orders));
        }

        private List<Order> ToOrderModelMapper(List<OrderDto> ordersDto)
        {

            var ordersModel = ordersDto.Select(orderDto => new Order
            {
                Id = orderDto.Id,
                Location = new Location 
                { 
                    X = orderDto.Location.X,
                    Y = orderDto.Location.Y
                },
            });
            return ordersModel.ToList();
        }

        private List<Courier> ToCouriersModelMapper(List<CourierDto> courierDto)
        {

            var couriersModel = courierDto.Select(courierDto => new Courier
            {
                Id = courierDto.Id,
                Location = new Location
                {
                    X = courierDto.Location.X,
                    Y = courierDto.Location.Y
                },
                Name = courierDto.Name
                
            });
            return couriersModel.ToList();
        }
    }
}
