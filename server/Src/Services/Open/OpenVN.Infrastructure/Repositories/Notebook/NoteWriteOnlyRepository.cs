using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class NoteWriteOnlyRepository : BaseWriteOnlyRepository<Note>, INoteWriteOnlyRepository
    {
        public NoteWriteOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }

        public async Task UpdateFromIndexOrderToLastAsync(int fromOrder, int toOrder, long categoryId, bool isIncrease, CancellationToken cancellationToken)
        {
            var cmd = $@"UPDATE {new Note().GetTableName()} n 
                         INNER JOIN {new NoteCategory().GetTableName()} nc 
                            ON n.CategoryId = nc.Id 
                         SET n.`Order` = n.`Order` {(isIncrease ? "+" : "-")} 1
                         WHERE nc.TenantId = {_token.Context.TenantId} 
                            AND nc.Id = {categoryId}
                            AND nc.OwnerId = {_token.Context.OwnerId} 
                            AND n.IsDeleted = 0 
                            AND nc.IsDeleted = 0";
            if (toOrder > 0)
            {
                cmd += $" AND n.Order BETWEEN {fromOrder} AND {toOrder}";
            }
            else
            {
                cmd += $" AND n.Order >= {fromOrder}";
            }

            await _dbConnection.ExecuteAsync(cmd, null);
        }
    }
}
