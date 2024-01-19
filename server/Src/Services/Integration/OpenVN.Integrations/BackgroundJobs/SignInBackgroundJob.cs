using RabbitMQ.Client;
using SharedKernel.Log;
using SharedKernel.Runtime;

namespace OpenVN.BackgroundJob
{
    public class SignInBackgroundJob : BackgroundService
    {
        private string exchange = "openvn-event-bus";
        private string queue = "sign-in-event";
        private string routingKey = "sign-in-event-key";

        private readonly ConnectionFactory _connectionFactory;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IServiceProvider _provider;

        public SignInBackgroundJob(
            ConnectionFactory connectionFactory,
            IExceptionHandler exceptionHandler,
            IServiceProvider provider
        )
        {
            _connectionFactory = connectionFactory;
            _exceptionHandler = exceptionHandler;
            _provider = provider;
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

                        var consumer = new SignInConsumer(_provider, channel);
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
            finally
            {
                Logging.Information($"Stopped {GetType().Name}");
            }
        }
    }
}