using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class FeedbackWriteOnlyRepository : BaseWriteOnlyRepository<Feedback>, IFeedbackWriteOnlyRepository
    {
        public FeedbackWriteOnlyRepository(
            IDbConnection dbConnection, 
            IToken token, 
            ISequenceCaching sequenceCaching, 
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }
    }
}
