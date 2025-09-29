using DeliveryApp.Core.Application.UseCases.Commands.AssignOrderToCourier;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class AssignOrdersJob : IJob
{
    private readonly IMediator _mediator;

    public AssignOrdersJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var assignOrdersCommand = new AssignOrderToCourierCommand();
        await _mediator.Send(assignOrdersCommand);
    }
}