using SharedKernel.Auth;
using SharedKernel.Caching;

namespace OpenVN.Application
{
    public class RemoveAvatarCommandHandler : BaseCommandHandler, IRequestHandler<RemoveAvatarCommand, Unit>
    {
        private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
        private readonly IToken _token;
        private readonly ISequenceCaching _caching;

        public RemoveAvatarCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IUserWriteOnlyRepository userWriteOnlyRepository,
            IToken token,
            ISequenceCaching caching
        ) : base(eventDispatcher, authService)
        {
            _userWriteOnlyRepository = userWriteOnlyRepository;
            _token = token;
            _caching = caching;
        }

        public async Task<Unit> Handle(RemoveAvatarCommand request, CancellationToken cancellationToken)
        {
            await _userWriteOnlyRepository.RemoveAvatarAsync(cancellationToken);
            await _userWriteOnlyRepository.UnitOfWork.CommitAsync(false, cancellationToken);

            _ = _eventDispatcher.FireEvent(new UpdateAvatarAuditEvent(_token, true));

            var cacheKey = OpenCacheKeys.GetAvatarUrlKey(_token.Context.TenantId, _token.Context.OwnerId);
            await _caching.RemoveAsync(cacheKey, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
