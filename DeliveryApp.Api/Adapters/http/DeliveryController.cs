using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
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
        public override Task<IActionResult> CreateCourier([FromBody] NewCourier newCourier)
        {
            throw new NotImplementedException();
        }

        public override async Task<IActionResult> CreateOrder()
        {
            var orderId = Guid.NewGuid();
            var street = "Несуществующая";
            var createOrderCommand = CreateOrderCommand.Create(orderId, street, 5).Value;
            var response = await _mediator.Send(createOrderCommand);
            if (response.IsSuccess) return Ok();
            return Conflict();
        }

        public override Task<IActionResult> GetCouriers()
        {
            throw new NotImplementedException();
        }

        public override Task<IActionResult> GetOrders()
        {
            throw new NotImplementedException();
        }
    }
}
