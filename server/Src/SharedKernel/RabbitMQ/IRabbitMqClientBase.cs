using SharedKernel.Domain;

namespace SharedKernel.RabbitMQ
{
    public interface IRabbitMqClientBase
    {
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : DomainEvent;
    }
}
