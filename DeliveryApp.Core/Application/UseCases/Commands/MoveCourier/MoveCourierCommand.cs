using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCourier
{
    public class MoveCourierCommand : IRequest<UnitResult<Error>>
    {
    }
}
