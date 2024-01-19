using SharedKernel.Auth;
using SharedKernel.Core;

namespace OpenVN.Application
{
    public class SignOutCommandHandler : BaseCommandHandler, IRequestHandler<SignOutCommand>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IToken _token;
        private readonly IOpenMessageHub _hub;

        public SignOutCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IAuthRepository authRepository,
            IToken token,
            IOpenMessageHub hub
        ) : base(eventDispatcher, authService)
        {
            _authRepository = authRepository;
            _token = token;
            _hub = hub;
        }

        public async Task<Unit> Handle(SignOutCommand request, CancellationToken cancellationToken)
        {
            // Revoke access token
            await _authService.RevokeAccessTokenAsync(_token.Context.AccessToken, cancellationToken);

            // Xóa refresh token
            await _authRepository.SignOutAsync(cancellationToken);

            _ = _eventDispatcher.FireEvent(new SignOutAuditEvent(_token), cancellationToken);

            if (CoreSettings.IsSingleDevice)
            {
                _ = _hub.SendSignOutMessage(cancellationToken);
            }
            return Unit.Value;
        }


    }
}
