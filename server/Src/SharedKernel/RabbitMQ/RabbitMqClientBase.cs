using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using SharedKernel.Domain;
using SharedKernel.Libraries;
using SharedKernel.Log;
using System.Text;

namespace SharedKernel.RabbitMQ
{
    public class RabbitMqClientBase : IRabbitMqClientBase
    {
        protected const string EXCHANGE = "openvn-event-bus";
        protected readonly ConnectionFactory _connectionFactory;
        protected readonly ILogger _logger;
        protected Dictionary<string, int> retries = new Dictionary<string, int>();
        protected const int MAX_RETRY = 5;

        public RabbitMqClientBase(ConnectionFactory connectionFactory, ILogger logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : DomainEvent
        {
            var retryKey = @event.EventId.ToString();
            try
            {
                if (retries.ContainsKey(retryKey))
                {
                    var delay = Convert.ToInt32(Math.Pow(2, retries[retryKey]) * 1000);
                    Logging.Information($"   RabbitMQ is retrying publish event {@event.EventId} for {retries[retryKey]}(th) time(s) with delay time = {delay}ms");

                    await Task.Delay(delay, cancellationToken);
                }

                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var properties = channel.CreateBasicProperties();
                        var queue = !string.IsNullOrEmpty(@event.EventQueue) ? @event.EventQueue : @event.EventType.ToKebabCaseLower();
                        var routingKey = (!string.IsNullOrEmpty(@event.EventQueue) ? @event.EventQueue : @event.EventType.ToKebabCaseLower()) + "-key";

                        channel.ExchangeDeclare(EXCHANGE, ExchangeType.Direct, true);
                        channel.QueueDeclare(queue, true, false, false);
                        channel.QueueBind(queue, EXCHANGE, routingKey);
                        channel.BasicPublish(EXCHANGE, routingKey, true, properties, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)));
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex.Message);
                if (retries.ContainsKey(retryKey))
                {
                    if (retries[retryKey] >= MAX_RETRY)
                    {
                        Logging.Warning($"{@event.EventId} could not send because over {MAX_RETRY} times retry");
                        throw;
                    }
                    retries[retryKey] = retries[retryKey] + 1;
                }
                else
                {
                    retries[retryKey] = 1;
                }
                await PublishAsync(@event, cancellationToken);
            }
        }
    }
}
