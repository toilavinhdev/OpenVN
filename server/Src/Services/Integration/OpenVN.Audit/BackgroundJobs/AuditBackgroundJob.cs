using OpenVN.Audit.Consumers;
using RabbitMQ.Client;
using SharedKernel.Log;
using SharedKernel.Runtime;

namespace OpenVN.Audit.BackgroundJobs
{
    public class AuditBackgroundJob : BackgroundService
    {
        private string exchange = "openvn-event-bus";
        private string queue = "audit-event";
        private string routingKey = "audit-event-key";

        private readonly ConnectionFactory _connectionFactory;
        private readonly IExceptionHandler _exceptionHandler;

        public AuditBackgroundJob(
            ConnectionFactory connectionFactory,
            IExceptionHandler exceptionHandler
        )
        {
            _connectionFactory = connectionFactory;
            _exceptionHandler = exceptionHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue, true, false, false);
                        channel.QueueBind(queue, exchange, routingKey);

                        var consumer = new AuditConsumer(channel);
                        channel.BasicConsume(queue, false, consumer);
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            await Task.Delay(5 * 60000, stoppingToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                await _exceptionHandler.PutToDatabaseAsync(ex);
            }
        }
    }
}
