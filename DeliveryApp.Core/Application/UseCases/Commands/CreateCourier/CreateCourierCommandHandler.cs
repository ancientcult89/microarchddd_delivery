using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateCourier
{
    public class CreateCourierCommandHandler : IRequestHandler<CreateCourierCommand, UnitResult<Error>>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCourierCommandHandler(ICourierRepository courierRepository, IUnitOfWork unitOfWork)
        {
            _courierRepository = courierRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<UnitResult<Error>> Handle(CreateCourierCommand request, CancellationToken cancellationToken)
        {
            var newCourier = Courier.Create(request.Name, request.Speed, Location.CreateRandom().Value);
            if (newCourier.IsFailure) return newCourier.Error;

            await _courierRepository.AddAsync(newCourier.Value);
            await _unitOfWork.SaveChangesAsync();
            return UnitResult.Success<Error>();
        }
    }
}
