using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SharedKernel.Domain;
using SharedKernel.Libraries;
using SharedKernel.Log;
using SharedKernel.MySQL;
using SharedKernel.RabbitMQ;

namespace SharedKernel.DomainEvents
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IMediator _mediator;
        private readonly IRabbitMqClientBase _rabbitMqClient;
        private readonly IServiceProvider _provider;

        public EventDispatcher(IServiceProvider provider)
        {
            _provider = provider.CreateScope().ServiceProvider;
            _mediator = _provider.GetRequiredService<IMediator>();
            _rabbitMqClient = _provider.GetRequiredService<IRabbitMqClientBase>();
        }

        public async Task FireEvent<T>(T @event, CancellationToken cancellationToken = default) where T : DomainEvent
        {
            await Task.Yield();
            Logging.Information($"[{@event.EventType}] Fire event [{@event.EventId}] at {@event.Timestamp}");

            @event.Token.Context.HttpContext = null;
            _ = Publish(@event, cancellationToken);
        }

        public async Task FireEvent<T>(List<T> events, CancellationToken cancellationToken = default) where T : DomainEvent
        {
            await Task.Yield();
            foreach (var @event in events)
            {
                _ = FireEvent(@event, cancellationToken);
            }
        }

        private async Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : DomainEvent
        {
            var mediatorTask = _mediator.Publish(@event, cancellationToken);
            var saveTask = SaveEventAsync(@event, cancellationToken);
            var mqTask = _rabbitMqClient.PublishAsync(@event, cancellationToken);

            await Task.WhenAll(mediatorTask, saveTask, mqTask);
        }

        private async Task SaveEventAsync<T>(T @event, CancellationToken cancellationToken) where T : DomainEvent
        {
            try
            {
                using (var dbConnection = new DbConnection("CentralizedEventsDb"))
                {
                    var properties = typeof(Event).GetProperties().Where(p => p.GetIndexParameters().Length == 0);
                    var cmd = $"INSERT INTO events ( `EventId`, `Timestamp`, `EventType`, `Body`, `CreatedDate` ) VALUES ( @EventId, @Timestamp, @EventType, @Body, @CreatedDate );";
                    var param = new Event
                    {
                        EventId = @event.EventId.ToString(),
                        EventType = @event.EventType,
                        Timestamp = @event.Timestamp,
                        Body = JsonConvert.SerializeObject(@event.Body),
                        CreatedDate = DateHelper.Now
                    };

                    await dbConnection.ExecuteAsync(cmd, param);
                    await dbConnection.CommitAsync(cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw;
            }
        }
    }
}
