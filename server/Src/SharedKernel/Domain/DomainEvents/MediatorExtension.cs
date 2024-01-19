using SharedKernel.Domain;
using MediatR;

namespace SharedKernel.DomainEvents
{
    public static class MediatorExtension
    {
        public static Task DispatchDomainEventsAsync(this IMediator mediator, IList<DomainEvent> events, CancellationToken cancellationToken = default)
        {
            if (events != null && events.Any())
            {
                foreach (var @event in events)
                {
                    mediator.Publish(@event, cancellationToken);
                }
            }
            return Task.CompletedTask;  
        }
    }
}
