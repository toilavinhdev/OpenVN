using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class ExtendCodeCommandHandler : BaseCommandHandler, IRequestHandler<ExtendCodeCommand, Unit>
    {
        private readonly IToken _token;
        private readonly ISequenceCaching _caching;

        public ExtendCodeCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IToken token,
            ISequenceCaching caching
        ) : base(eventDispatcher, authService)
        {
            _token = token;
            _caching = caching;
        }

        public async Task<Unit> Handle(ExtendCodeCommand request, CancellationToken cancellationToken)
        {
            var key = OpenCacheKeys.GetCloudCodeKey(_token.Context.TenantId, _token.Context.OwnerId, request.DirectoryId);
            var code = await _caching.GetStringAsync(key, cancellationToken: cancellationToken);

            await _caching.SetAsync(key, code, TimeSpan.FromMinutes(15), cancellationToken: cancellationToken);
            return Unit.Value;
        }
    }
}
