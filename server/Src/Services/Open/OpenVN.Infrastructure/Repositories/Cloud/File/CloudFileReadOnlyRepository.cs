using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class CloudFileReadOnlyRepository : BaseReadOnlyRepository<CloudFile>, ICloudFileReadOnlyRepository
    {
        public CloudFileReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<IEnumerable<CloudFile>> GetListFileByIdsAsync(List<long> ids, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT * FROM {new CloudFile().GetTableName()} 
                         WHERE Id IN ({string.Join(",", ids)}) AND 
                         TenantId = {_token.Context.TenantId} AND 
                         OwnerId = {_token.Context.OwnerId} AND 
                         IsDeleted = 0";
            return await _dbConnection.QueryAsync<CloudFile>(cmd);
        }

        public async Task<List<CloudFileDto>> GetFilesInDirectoryAsync(long directoryId, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetFilesInDirectory"];
            var param = new Dictionary<string, object>
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_directory_id", directoryId },
            };
            return (await _dbConnection.QueryAsync<CloudFileDto>(sp, param, commandType: System.Data.CommandType.StoredProcedure)).ToList();
        }

        public async Task<List<CloudFile>> GetFilesInDirectoriesAsync(List<long> directoryIds, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetFilesInDirectories"];
            var param = new Dictionary<string, object>
                {
                    { "v_tenant_id", _token.Context.TenantId },
                    { "v_owner_id", _token.Context.OwnerId },
                    { "v_ids", string.Join(",", directoryIds) },
                };
            return (await _dbConnection.QueryAsync<CloudFile>(sp, param, commandType: System.Data.CommandType.StoredProcedure)).ToList();
        }

        public async Task<long> GetTotalFileSizeAsync(CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetTotalFileSize"];
            var param = new Dictionary<string, object>
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_directory_id", -1 },
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<long>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<long> GetTotalFileSizeInSpecialDirectoryAsync(long directoryId, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetTotalFileSize"];
            var param = new Dictionary<string, object>
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_directory_id", directoryId },
            };
            return await _dbConnection.QueryFirstOrDefaultAsync<long>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<FilePropertyDto> GetPropertiesAsync(long id, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT * FROM {new CloudFile().GetTableName()} 
                         WHERE Id = {id} AND 
                               TenantId = {_token.Context.TenantId} AND
                               OwnerId = {_token.Context.OwnerId} AND
                               IsDeleted = 0;";

            var file = await _dbConnection.QuerySingleOrDefaultAsync<CloudFile>(cmd);
            var type = FileHelper.IsImage(file.FileName) ? "Image" : FileHelper.IsVideo(file.FileName) ? "Video" : "File";

            var result = new FilePropertyDto
            {
                Type = type,
                Size = file.Size,
                Name = file.OriginalFileName,
                CreatedDate = file.CreatedDate,
                LastModifiedDate = file.LastModifiedDate,
            };

            return result;
        }
    }
}
