using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCourier
{
    public class MoveCourierCommandHandler : IRequestHandler<MoveCourierCommand, UnitResult<Error>>
    {
        public Task<UnitResult<Error>> Handle(MoveCourierCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
