using BasketApp.Infrastructure.Adapters.Postgres.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Primitives;
using Quartz;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class ProcessOutboxMessagesJob : IJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        public ProcessOutboxMessagesJob(ApplicationDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // Получаем все DomainEvents, которые еще не были отправлены (где ProcessedOnUtc == null)
            var outboxMessages = await _dbContext
                .Set<OutboxMessage>()
                .Where(m => m.ProcessedOnUtc == null)
                .OrderBy(o => o.OccurredOnUtc)
                .Take(20)
                .ToListAsync(context.CancellationToken);

            // Если такие есть, то перебираем их в цикле
            if (outboxMessages.Any())
            {
                foreach (var outboxMessage in outboxMessages)
                {
                    // Настройки сериализатора
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = new PrivateSetterContractResolver(),
                        TypeNameHandling = TypeNameHandling.All
                    };

                    // Десериализуем запись из OutboxMessages в DomainEvent
                    var domainEvent = JsonConvert.DeserializeObject<DomainEvent>(outboxMessage.Content, settings);

                    // Отправляем
                    await _mediator.Publish(domainEvent, context.CancellationToken);

                    // Если предыдущий метод не вернул ошибку, значит отправка была успешной
                    // Ставим дату отправки, это будет признаком, что сообщение отправлять больше не нужно 
                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
                    _dbContext.OutboxMessages.Update(outboxMessage);
                }

                // Сохраняем изменения
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
