namespace OpenVN.Application
{
    public class RefreshTokenCommand : BaseAllowAnonymousCommand<BaseResponse>
    {
        public long UserId { get; set; }

        public string RefreshToken { get; set; }

        public RefreshTokenCommand(long userId, string refreshToken)
        {
            UserId = userId;
            RefreshToken = refreshToken;  
        }
    }
}
