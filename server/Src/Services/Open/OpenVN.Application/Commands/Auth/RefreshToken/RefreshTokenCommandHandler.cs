using SharedKernel.Libraries;

namespace OpenVN.Application
{
    public class RefreshTokenCommandHandler : BaseCommandHandler, IRequestHandler<RefreshTokenCommand, BaseResponse>
    {
        private readonly IAuthRepository _authRepository;

        public RefreshTokenCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IAuthRepository authRepository
        ) : base(eventDispatcher, authService)
        {
            _authRepository = authRepository;
        }

        public async Task<BaseResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var isValid = await _authService.CheckRefreshTokenAsync(request.RefreshToken, request.UserId, cancellationToken);
            if (!isValid)
            {
                return new BaseResponse
                {
                    Error = new Error(400, "The refresh-token is invalid or expried!")
                };
            }

            var tokenUser = await _authRepository.GetTokenUserByOwnerIdAsync(request.UserId, cancellationToken);
            if (tokenUser == null)
            {
                return new BaseResponse
                {
                    Error = new Error(400, "Currently, this account does not exist")
                };
            }

            var currentAccessToken = await _authService.GenerateAccessTokenAsync(tokenUser, cancellationToken);
            var refreshToken = new RefreshToken
            {
                RefreshTokenValue = request.RefreshToken,
                CurrentAccessToken= currentAccessToken,
                OwnerId= request.UserId,
                ExpriedDate = DateHelper.Now.AddSeconds(AuthConstant.REFRESH_TOKEN_TIME)
            };
            await _authRepository.CreateOrUpdateRefreshTokenAsync(refreshToken, cancellationToken);

            return new AuthResponse { AccessToken = currentAccessToken, RefreshToken = request.RefreshToken };
        }
    }
}
