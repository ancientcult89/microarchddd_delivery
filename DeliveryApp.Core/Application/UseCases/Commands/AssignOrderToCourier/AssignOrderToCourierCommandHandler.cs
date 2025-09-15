using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier
{
    public class AssignOrderToCourierCommandHandler : IRequestHandler<AssignOrderToCourierCommand, UnitResult<Error>>
    {
        public Task<UnitResult<Error>> Handle(AssignOrderToCourierCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
