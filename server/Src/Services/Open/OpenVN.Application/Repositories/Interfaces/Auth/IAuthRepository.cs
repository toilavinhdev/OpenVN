using SharedKernel.UnitOfWork;

namespace OpenVN.Application
{
    public interface IAuthRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task<TokenUser> GetTokenUserByIdentityAsync(string username, string password, CancellationToken cancellationToken);

        Task<TokenUser> GetTokenUserByOwnerIdAsync(long ownerId, CancellationToken cancellationToken);

        Task SignOutAsync(CancellationToken cancellationToken);

        Task<bool> CheckRefreshTokenAsync(string value, long ownerId, CancellationToken cancellationToken);

        Task CreateOrUpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

        Task RemoveRefreshTokenAsync(CancellationToken cancellationToken);

        Task SetRoleForUserAsync(long userId, List<long> roleIds, CancellationToken cancellationToken);

        Task<bool> VerifySecretKeyAsync(string secretKey, CancellationToken cancellationToken);

        Task<PagingResult<SignInHistoryDto>> GetSignInHistoryPaging(PagingRequest request, CancellationToken cancellationToken);
    }
}
