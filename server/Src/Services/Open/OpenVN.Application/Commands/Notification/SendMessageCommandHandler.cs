namespace OpenVN.Application
{
    public class SendMessageCommandHandler : BaseCommandHandler, IRequestHandler<SendMessageCommand, Unit>
    {
        private readonly IOpenMessageHub _hub;

        public SendMessageCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IOpenMessageHub hub
        ) : base(eventDispatcher, authService)
        {
            _hub = hub;
        }

        public async Task<Unit> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            await _hub.SendMessages(request.Notification, cancellationToken);
            return Unit.Value;
        }
    }
}
