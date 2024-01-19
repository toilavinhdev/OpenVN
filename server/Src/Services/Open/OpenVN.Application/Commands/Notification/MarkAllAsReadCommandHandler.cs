namespace OpenVN.Application
{
    public class MarkAllAsReadCommandHandler : BaseCommandHandler, IRequestHandler<MarkAllAsReadCommand, Unit>
    {
        private readonly INotificationWriteOnlyRepository _notificationWriteOnlyRepository;

        public MarkAllAsReadCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            INotificationWriteOnlyRepository notificationWriteOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _notificationWriteOnlyRepository = notificationWriteOnlyRepository;
        }

        public async Task<Unit> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationWriteOnlyRepository.MarkAllAsReadAsync(cancellationToken);
            await _notificationWriteOnlyRepository.UnitOfWork.CommitAsync(true, cancellationToken);

            return Unit.Value;
        }
    }
}
