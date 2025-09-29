using Confluent.Kafka;
using DeliveryApp.Core.Domain.Model.OrderAggrerate.DomainEvents;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderStatusChanged;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged
{
    public class Producer : IMessageBusProducer
    {
        private readonly ProducerConfig _config;
        private readonly string _topicName;

        public Producer(IOptions<Settings> options)
        {
            if (string.IsNullOrWhiteSpace(options.Value.MessageBrokerHost))
                throw new ArgumentException(nameof(options.Value.MessageBrokerHost));
            if (string.IsNullOrWhiteSpace(options.Value.OrderStatusChangedTopic))
                throw new ArgumentException(nameof(options.Value.OrderStatusChangedTopic));

            _config = new ProducerConfig
            {
                BootstrapServers = options.Value.MessageBrokerHost
            };
            _topicName = options.Value.OrderStatusChangedTopic;
        }

        public async Task PublishOrderStatusCompletedDomainEvent(OrderCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            var orderCompletedIntegrationEvent = new OrderCreatedIntegrationEvent()
            {
                EventId = notification.EventId.ToString(),
                OccurredAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(notification.OccurredAt),
                OrderId = notification.Order.Id.ToString()
            };

            // Создаем сообщение для Kafka
            var message = new Message<string, string>
            {
                Key = notification.EventId.ToString(),
                Value = JsonConvert.SerializeObject(orderCompletedIntegrationEvent),
                Headers = new Headers() { new Header("Completed", new byte[0])}
            };

            try
            {
                // Отправляем сообщение в Kafka
                using var producer = new ProducerBuilder<string, string>(_config).Build();
                var dr = await producer.ProduceAsync(_topicName, message, cancellationToken);
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }

        public async Task PublishOrderStatusCreatedDomainEvent(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent
            {
                EventId = notification.EventId.ToString(),
                OccurredAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(notification.OccurredAt),
                OrderId = notification.Order.Id.ToString()
            };

            // Создаем сообщение для Kafka
            var message = new Message<string, string>
            {
                Key = notification.EventId.ToString(),
                Value = JsonConvert.SerializeObject(orderCreatedIntegrationEvent),
                Headers = new Headers() { new Header("Created", new byte[0]) }
            };

            try
            {
                // Отправляем сообщение в Kafka
                using var producer = new ProducerBuilder<string, string>(_config).Build();
                var dr = await producer.ProduceAsync(_topicName, message, cancellationToken);
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}
