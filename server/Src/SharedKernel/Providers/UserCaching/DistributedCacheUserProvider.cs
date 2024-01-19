using SharedKernel.Caching;
using static SharedKernel.Application.Enum;

namespace SharedKernel.Providers
{
    public class DistributedCacheUserProvider : IDistributedCacheUserProvider
    {
        private readonly ISequenceCaching _caching;

        public DistributedCacheUserProvider(ISequenceCaching caching)
        {
            _caching = caching;
        }

        public async Task<AccountState?> GetAccountStateAsync(string userId, CancellationToken cancellationToken = default)
        {
            var accountState = await _caching.GetAsync(userId, cancellationToken: cancellationToken);
            if (accountState == null)
                return null;

            return (AccountState)Convert.ToInt32(accountState);
        }

        public Task SetAccountStateAsync(string userId, AccountState state, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
