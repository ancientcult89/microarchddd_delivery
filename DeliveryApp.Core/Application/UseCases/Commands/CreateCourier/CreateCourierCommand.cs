using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateCourier
{
    public class CreateCourierCommand : IRequest<UnitResult<Error>>
    {
        private CreateCourierCommand(string name, int speed)
        {
            Name = name;
            Speed = speed;
        }

        public string Name { get; }
        public int Speed { get; }

        public static Result<CreateCourierCommand, Error> Create(string name, int speed)
        {
            if (string.IsNullOrWhiteSpace(name)) return GeneralErrors.ValueIsRequired(nameof(name));
            if (speed <= 0) return GeneralErrors.ValueIsRequired(nameof(speed));

            return new CreateCourierCommand(name, speed);
        }
    }
}
