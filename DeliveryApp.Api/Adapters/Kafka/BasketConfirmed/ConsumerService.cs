using BasketConfirmed;
using Confluent.Kafka;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DeliveryApp.Api.Adapters.Kafka.Checkout
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _topic;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ConsumerService(IServiceScopeFactory serviceScopeFactory, IOptions<Settings> settings)
        {
            _serviceScopeFactory = serviceScopeFactory;

            if (string.IsNullOrWhiteSpace(settings.Value.MessageBrokerHost))
                throw new ArgumentException(nameof(settings.Value.MessageBrokerHost));
            if (string.IsNullOrWhiteSpace(settings.Value.BasketConfirmedTopic))
                throw new ArgumentException(nameof(settings.Value.BasketConfirmedTopic));

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = settings.Value.MessageBrokerHost,
                GroupId = "DeliveryConsumerGroup",
                EnableAutoOffsetStore = false,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            _topic = settings.Value.BasketConfirmedTopic;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    var consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult.IsPartitionEOF) continue;

                    var basketConfirmedIntegrationEvent =
                        JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(consumeResult.Message.Value);

                    Guid orderId = Guid.NewGuid();
                    string steet = basketConfirmedIntegrationEvent.Address.Street;
                    int volume = basketConfirmedIntegrationEvent.Volume;

                    var createOrderCommandResult = CreateOrderCommand.Create(orderId, steet, volume);

                    if (createOrderCommandResult.IsFailure) Console.WriteLine(createOrderCommandResult.Error);

                    var sendResult = await mediator.Send(createOrderCommandResult.Value, cancellationToken);
                    if (sendResult.IsFailure) Console.WriteLine(sendResult.Error);

                    try
                    {
                        _consumer.StoreOffset(consumeResult);
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"Store Offset error: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
