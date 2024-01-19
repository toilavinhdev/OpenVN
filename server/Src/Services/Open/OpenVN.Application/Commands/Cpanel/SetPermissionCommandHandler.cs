namespace OpenVN.Application
{

    public class SetPermissionCommandHandler : BaseCommandHandler, IRequestHandler<SetPermissionCommand, Unit>
    {
        private readonly ICpanelReadOnlyRepository _cpanelReadOnlyRepository;

        public SetPermissionCommandHandler(
            IEventDispatcher eventDispatcher, 
            IAuthService authService,
            ICpanelReadOnlyRepository cpanelReadOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _cpanelReadOnlyRepository = cpanelReadOnlyRepository;
        }

        public Task<Unit> Handle(SetPermissionCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
