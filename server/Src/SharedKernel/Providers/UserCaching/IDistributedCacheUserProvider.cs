using static SharedKernel.Application.Enum;

namespace SharedKernel.Providers
{
    public interface IDistributedCacheUserProvider
    {
        Task<AccountState?> GetAccountStateAsync(string userId, CancellationToken cancellationToken = default);

        Task SetAccountStateAsync(string userId, AccountState state, CancellationToken cancellationToken = default);
    }
}
