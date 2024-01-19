using MassTransit.RabbitMqTransport.Topology;
using Polly;
using SharedKernel.Log;
using SharedKernel.Runtime;
using static MassTransit.Logging.OperationName;
using System.Threading.Channels;

namespace OpenVN.BackgroundJobs
{
    public class KeepApiRunningBackgroundJob : BackgroundService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IExceptionHandler _exceptionHandler;
        private int retryIndex = 0;
        private int retryCount = 10;

        public KeepApiRunningBackgroundJob(IHttpClientFactory clientFactory, IExceptionHandler exceptionHandler)
        {
            _clientFactory = clientFactory;
            _exceptionHandler = exceptionHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var url = "https://open-vn.pro/api/keeper/ping";
                var url2 = "https://apps.backend.edu.vn/api/keeper/ping";
                var client = _clientFactory.CreateClient();

                while (!stoppingToken.IsCancellationRequested)
                {
                    var response = await client.GetAsync(url);
                    var response2 = await client.GetAsync(url2);
                    await Task.Delay(5 * 60000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                await _exceptionHandler.PutToDatabaseAsync(ex);
            }
            finally
            {
                if (retryIndex++ < retryCount)
                {
                    Logging.Information($"Background is restarting with delay time = {Math.Pow(2, retryIndex) * 1000}ms");
                    await Task.Delay(Convert.ToInt32(Math.Pow(2, retryIndex) * 1000));
                    await ExecuteAsync(stoppingToken);
                }
                else
                {
                    Logging.Information("Background could not restarted, it will be stop!!!");
                }
            }
        }
    }
}
