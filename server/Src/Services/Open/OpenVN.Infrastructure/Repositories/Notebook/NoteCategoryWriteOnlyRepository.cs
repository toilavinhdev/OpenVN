using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class NoteCategoryWriteOnlyRepository : BaseWriteOnlyRepository<NoteCategory>, INoteCategoryWriteOnlyRepository
    {
        public NoteCategoryWriteOnlyRepository(
            IDbConnection dbConnection, 
            IToken token, 
            ISequenceCaching sequenceCaching, 
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }
    }
}
