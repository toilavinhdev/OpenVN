using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class SpendingWriteOnlyRepository : BaseWriteOnlyRepository<Spending>, ISpendingWriteOnlyRepository
    {
        public SpendingWriteOnlyRepository(
            IDbConnection dbConnection, 
            IToken token, 
            ISequenceCaching sequenceCaching, 
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }

        public Task<long> AddSpendingAsync(List<Spending> spendings, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
