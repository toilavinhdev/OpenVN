using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Libraries;
using System.Net;

namespace OpenVN.Application
{
    public class SignInCommandHandler : BaseCommandHandler, IRequestHandler<SignInCommand, BaseResponse>
    {
        private readonly IAuthRepository _repository;
        private readonly IToken _token;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IOpenMessageHub _hub;

        public SignInCommandHandler(
            IAuthService authService,
            IAuthRepository repository,
            IToken token,
            IEventDispatcher eventDispatcher,
            IStringLocalizer<Resources> localizer,
            IOpenMessageHub hub
        ) : base(eventDispatcher, authService)
        {
            _repository = repository;
            _token = token;
            _localizer = localizer;
            _hub = hub;
        }

        public async Task<BaseResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var tokenUser = await _repository.GetTokenUserByIdentityAsync(request.UserName, request.Password, cancellationToken);
            if (tokenUser == null)
            {
                return new BaseResponse
                {
                    Error = new Error(HttpStatusCode.BadRequest, _localizer["auth_sign_in_info_incorrect"].Value)
                };
            }

            var result = new AuthResponse
            {
                AccessToken = await _authService.GenerateAccessTokenAsync(tokenUser, cancellationToken),
                RefreshToken = _authService.GenerateRefreshToken(),
            };

            // Save refresh token
            var refreshToken = new RefreshToken
            {
                RefreshTokenValue = result.RefreshToken,
                CurrentAccessToken = result.AccessToken,
                OwnerId = tokenUser.Id,
                ExpriedDate = DateHelper.Now.AddSeconds(AuthConstant.REFRESH_TOKEN_TIME),
            };
            await _repository.CreateOrUpdateRefreshTokenAsync(refreshToken, cancellationToken);

            // Publish event
            var @event = new SignInEvent(_token, Guid.NewGuid(), new
            {
                TokenUser = tokenUser,
                RequestId = AuthUtility.GetCurrentRequestId(_token.Context.HttpContext)
            });

            _token.Context.AccessToken = result.AccessToken;
            _token.Context.TenantId = tokenUser.TenantId;
            _token.Context.OwnerId = tokenUser.Id;
            _ = _eventDispatcher.FireEvent(@event, cancellationToken);
            _ = _eventDispatcher.FireEvent(new SignInAuditEvent(_token), cancellationToken);

            //_ = _hub.SendSignInMessage(cancellationToken);

            return result;
        }
    }
}
