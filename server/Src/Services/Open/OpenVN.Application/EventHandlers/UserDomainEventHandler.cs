//using Serilog;

//namespace OpenVN.Application
//{
//    public class UserDomainEventHandler : IEventHandler<CreateUserEvent>
//    {
//        private readonly ILogger _logger;

//        public UserDomainEventHandler(ILogger logger)
//        {
//            _logger = logger;
//        }

//        public async Task Handle(CreateUserEvent notification, CancellationToken cancellationToken)
//        {
//            await Task.Yield();
//            _logger.Information($"[{GetType().Name}] received an event: {notification.EventType} [{notification.EventId}]");
//        }
//    }
//}
