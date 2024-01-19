using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using System.Collections.Generic;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Infrastructure
{
    public class DirectoryReadOnlyRepository : BaseReadOnlyRepository<Directory>, IDirectoryReadOnlyRepository
    {
        public DirectoryReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public Task<PagingResult<Directory>> GetPagingResultAsync(long directoryId, PagingRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Directory>> GetListDirectoryByIdsAsync(List<long> ids, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT *, CONVERT(FROM_BASE64(Name), char(512)) as Name  FROM {new Directory().GetTableName()} 
                         WHERE Id IN ({string.Join(",", ids)}) AND 
                         TenantId = {_token.Context.TenantId} AND 
                         OwnerId = {_token.Context.OwnerId} AND 
                         IsDeleted = 0";
            return await _dbConnection.QueryAsync<Directory>(cmd);
        }

        public async Task<int> GetMaxDirectoryDuplicateNoAsync(Directory directory, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetMaxDirectoryDuplicateNo"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_directory_name", directory.Name },
                { "v_parent_id", directory.ParentId },
                { "v_id", directory.Id },
            };

            return await _dbConnection.QuerySingleOrDefaultAsync<int>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public override async Task<TResult> GetByIdAsync<TResult>(object id, CancellationToken cancellationToken)
        {
            var cmd = @$"SELECT
                            d.Id, d.ParentId, d.Path, d.DuplicateNo, d.CreatedDate, d.LastModifiedDate, d.Path,
                            (CASE WHEN d.DuplicateNo > 0 THEN CONCAT(CONVERT(FROM_BASE64(d.Name), char(512)), '(', d.DuplicateNo, ')') ELSE CONVERT(FROM_BASE64(d.Name), char(512)) END) AS Name,
                            (select COUNT(*) + (SELECT COUNT(*) FROM cloud_file cf WHERE cf.DirectoryId = d.Id AND cf.TenantId = {_token.Context.TenantId} AND cf.OwnerId = {_token.Context.OwnerId} AND cf.IsDeleted = 0)
                            FROM {_tableName} d1 WHERE d1.ParentId = d.Id AND d1.TenantId = {_token.Context.TenantId} AND d1.OwnerId = {_token.Context.OwnerId} AND d1.IsDeleted = 0) AS 'ChildrenCount'
                        FROM {_tableName} AS d
                        WHERE     d.Id = @Id 
                              AND d.OwnerId = {_token.Context.OwnerId} 
                              AND d.TenantId = {_token.Context.TenantId} 
                              AND d.IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<TResult>(cmd, new { Id = id });
        }

        public override async Task<IEnumerable<TResult>> GetAllAsync<TResult>(CancellationToken cancellationToken)
        {
            var cacheResult = await GetAllCacheAsync<TResult>(cancellationToken);
            if (cacheResult.Value != null && cacheResult.Value.Any())
            {
                return cacheResult.Value;
            }

            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetAllDirectory"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
            };
            var result = await _dbConnection.QueryAsync<TResult>(sp, param, commandType: System.Data.CommandType.StoredProcedure);

            if (result.Any())
            {
                await _sequenceCaching.SetAsync(cacheResult.Key, result, TimeSpan.FromDays(7), cancellationToken: cancellationToken);
            }
            return result;
        }

        public async Task<List<DirectoryDto>> GetChildrenDirectoryAsync(long parentDirectoryId, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetChildrenDirectory"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_parent_id", parentDirectoryId },
            };
            var result = await _dbConnection.QueryAsync<DirectoryDto>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
            return result.ToList();
        }

        public Task<Directory> GetParentDirectoryAsync(long parentDirectoryId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<PathDto> GetPathAsync(long directoryId, CancellationToken cancellationToken)
        {
            var dir = await GetByIdAsync<Directory>(directoryId.ToString(), cancellationToken);
            var pathIds = dir.Path.Split("-");
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetDirectoryPath"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_dir_id", directoryId },
            };
            var pathMap = await _dbConnection.QueryAsync<PathMapping>(sp, param, commandTimeout: 5, commandType: System.Data.CommandType.StoredProcedure);

            return new PathDto()
            {
                Path = string.Join("/", pathMap.Select(x => x.Name)),
                Mapping = pathMap.ToList()
            };
        }

        public async Task<IEnumerable<string>> GetChildrenNodeIdAsync(long rootId, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetChildrenNodeId"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_root_id", rootId },
            };
            return await _dbConnection.QueryAsync<string>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Directory> FindFirstNodeLockedDirectoryAsync(long sourceId, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["FindFirstNodeLockedDirectory"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_source_id", sourceId },
            };
            return await _dbConnection.QuerySingleOrDefaultAsync<Directory>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<DirectoryRelationshipType> GetRelationshipBetwenTwoDirectories(long leftId, long rightId, CancellationToken cancellationToken)
        {
            var directories = await GetListDirectoryByIdsAsync(new List<long> { leftId, rightId }, cancellationToken);
            if (!directories.Any())
            {
                return DirectoryRelationshipType.AllNotFound;
            }
            var left = directories.FirstOrDefault(x => x.Id == leftId);
            var right = directories.FirstOrDefault(x => x.Id == rightId);

            if (left == null)
            {
                return DirectoryRelationshipType.LeftDirectoryNotFound;
            }

            if (right == null)
            {
                return DirectoryRelationshipType.RightDirectoryNotFound;
            }

            if (right.Path.Contains(left.Id.ToString()))
            {
                return DirectoryRelationshipType.LeftIsRoot;
            }

            if (left.Path.Contains(right.Id.ToString()))
            {
                return DirectoryRelationshipType.RightIsRoot;
            }

            if (left.ParentId == right.ParentId)
            {
                return DirectoryRelationshipType.SameRank;
            }

            return DirectoryRelationshipType.NoRelationship;
        }

        public async Task<bool> IsLockedDirectoryAsync(long directoryId, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT Id FROM {new LockedDirectory().GetTableName()} 
                         WHERE TenantId = {_token.Context.TenantId} 
                               AND OwnerId = {_token.Context.OwnerId} 
                               AND DirectoryId = {directoryId}
                               AND IsDeleted = 0 
                               AND EnabledLock = 1";
            return (await _dbConnection.QuerySingleOrDefaultAsync<LockedDirectory>(cmd)) != null;
        }

        public async Task<bool> HasPasswordAsync(long directoryId, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT CASE WHEN Password IS NOT NULL AND PASSWORD <> '' THEN TRUE ELSE FALSE END 
                         FROM {new LockedDirectory().GetTableName()} 
                         WHERE TenantId = {_token.Context.TenantId} 
                               AND OwnerId = {_token.Context.OwnerId} 
                               AND DirectoryId = {directoryId}
                               AND IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<bool>(cmd);
        }

        public async Task<DirectoryPropertyDto> GetPropertiesAsync(long id, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetDirectoryProperties"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_dir_id", id },
            };
            return await _dbConnection.QuerySingleOrDefaultAsync<DirectoryPropertyDto>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
