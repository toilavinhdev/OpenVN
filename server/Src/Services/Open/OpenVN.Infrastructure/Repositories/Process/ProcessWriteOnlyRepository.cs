using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class ProcessWriteOnlyRepository : BaseWriteOnlyRepository<Process>, IProcessWriteOnlyRepository
    {
        public ProcessWriteOnlyRepository(
            IDbConnection dbConnection, 
            IToken token, 
            ISequenceCaching sequenceCaching, 
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }
    }
}
