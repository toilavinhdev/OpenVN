using AutoMapper;
using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Properties;
using Enum = SharedKernel.Application.Enum;

namespace OpenVN.Infrastructure
{
    public class UserWriteOnlyRepository : BaseWriteOnlyRepository<User>, IUserWriteOnlyRepository
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public UserWriteOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IStringLocalizer<Resources> localizer,
            IAuthRepository authRepository,
            IMapper mapper
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
            _authRepository = authRepository;
            _mapper = mapper;
        }

        protected override void BeforeSave(IEnumerable<User> entities)
        {
            var batches = entities.ChunkList(1000);
            batches.ToList().ForEach(async entities =>
            {
                entities.ForEach(entity =>
                {
                    entity.Id = AuthUtility.GenerateSnowflakeId();
                    entity.CreatedBy = _token.Context.OwnerId;
                    entity.CreatedDate = DateHelper.Now;
                    entity.LastModifiedDate = null;
                    entity.LastModifiedBy = null;
                    entity.DeletedDate = null;
                    entity.DeletedBy = null;

                    var clone = (User)entity.Clone();
                    clone.ClearDomainEvents();

                    entity.AddDomainEvent(new InsertAuditEvent<User>(new List<User> { clone }, _token));
                });

                if (batches.Count() > 1)
                {
                    await Task.Delay(69);
                }
            });
        }

        public async Task<long> CreateUserAsync(CreateUserDto data, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<User>(data);
            entity.Id = AuthUtility.GenerateSnowflakeId();
            entity.PasswordHash = data.Password.ToMD5();
            entity.Salt = Utility.RandomString(6);

            await SaveAsync(entity, cancellationToken);
            await _authRepository.SetRoleForUserAsync(entity.Id, new List<long> { (long)Enum.RoleId.EMPLOYEE }, cancellationToken);
            await UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return entity.Id;
        }

        public async Task SetAvatarAsync(string fileName, CancellationToken cancellationToken)
        {
            var query = $"SELECT Id FROM {new Avatar().GetTableName()} WHERE TenantId = {_token.Context.TenantId} AND OwnerId = {_token.Context.OwnerId} AND IsDeleted = 0";
            var entity = await _dbConnection.QuerySingleOrDefaultAsync<Avatar>(query);
            var cmd = string.Empty;
            if (entity == null)
            {
                cmd = $"INSERT INTO {new Avatar().GetTableName()}(Id, FileName, TenantId, OwnerId, CreatedBy) VALUES(@Id, @FileName, @TenantId, @OwnerId, @CreatedBy)";
            }
            else
            {
                cmd = $@"UPDATE {new Avatar().GetTableName()} 
                            SET FileName = @FileName, 
                                LastModifiedDate = CURRENT_TIMESTAMP(), 
                                LastModifiedBy = {_token.Context.OwnerId}
                         WHERE TenantId = @TenantId AND OwnerId = @OwnerId AND IsDeleted = 0";
            }

            var avatar = new Avatar
            {
                Id = AuthUtility.GenerateSnowflakeId(),
                FileName = fileName,
                TenantId = _token.Context.TenantId,
                OwnerId = _token.Context.OwnerId,
                CreatedBy = _token.Context.OwnerId
            };

            await _dbConnection.ExecuteAsync(cmd, avatar);
        }

        public async Task RemoveAvatarAsync(CancellationToken cancellationToken)
        {
            var cmd = $"UPDATE {new Avatar().GetTableName()} SET IsDeleted = 1 WHERE OwnerId = {_token.Context.OwnerId} AND TenantId = {_token.Context.TenantId}";
            await _dbConnection.ExecuteAsync(cmd, null);
        }
    }
}
