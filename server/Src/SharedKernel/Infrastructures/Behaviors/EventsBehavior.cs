using MediatR;
using SharedKernel.Domain;
using SharedKernel.MySQL;
using System.Diagnostics;

namespace SharedKernel.Infrastructures
{
    public class EventsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IDbConnection _dbConnection;
        private readonly IEventDispatcher _eventDispatcher;

        public EventsBehavior(IDbConnection dbConnection, IEventDispatcher eventDispatcher)
        {
            _dbConnection = dbConnection;
            _eventDispatcher = eventDispatcher;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var result = await next();
            await _dbConnection.PublishEvents(_eventDispatcher, cancellationToken);

            return result;
        }
    }
}
