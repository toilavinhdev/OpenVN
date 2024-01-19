using AutoMapper;
using SharedKernel.Auth;

namespace OpenVN.Application
{
    public class GetAvatarQueryHandler : BaseQueryHandler, IRequestHandler<GetAvatarQuery, string>
    {
        private readonly IUserService _userService;
        private readonly IToken _token;

        public GetAvatarQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IUserService userService,
            IToken token
        ) : base(authService, mapper)
        {
            _userService = userService;
            _token = token;
        }

        public async Task<string> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
        {
            return await _userService.GetAvatarUrlByFileNameAsync(string.Empty, _token.Context.TenantId, _token.Context.OwnerId, cancellationToken);
        }
    }
}
