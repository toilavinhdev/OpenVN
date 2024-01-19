using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using SharedKernel.Properties;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Infrastructure
{
    public class CpanelWriteOnlyRepository : BaseWriteOnlyRepository<BaseEntity>, ICpanelWriteOnlyRepository
    {
        public CpanelWriteOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }

        public async Task UpdateRoleAsync(object roleId, object actionId, bool value, CancellationToken cancellationToken)
        {
            var query = @$"SELECT * FROM {new Role().GetTableName()} WHERE Id = @RoleId AND TenantId = {_token.Context.TenantId} AND IsDeleted = 0;
                         SELECT * FROM {new RoleAction().GetTableName()} WHERE RoleId = @RoleId AND ActionId = @ActionId AND TenantId = {_token.Context.TenantId}";
            var grid = await _dbConnection.QueryMultipleAsync(query, new { RoleId = roleId, ActionId = actionId });
            var role = await grid.ReadFirstOrDefaultAsync<Role>();
            if (role == null)
            {
                throw new BadRequestException(_localizer["repository_data_does_not_exist_or_was_deleted"]);
            }
            var roleAction = await grid.ReadFirstOrDefaultAsync<RoleAction>();
            if (roleAction == null)
            {
                var cmd = $"INSERT INTO {new RoleAction().GetTableName()}(RoleId, ActionId, TenantId, CreatedBy) VALUES ( @RoleId, @ActionId, {_token.Context.TenantId}, {_token.Context.OwnerId})";
                await _dbConnection.ExecuteAsync(cmd, new { RoleId = roleId, ActionId = actionId });
            }
            else
            {
                var cmd = @$"UPDATE {new RoleAction().GetTableName()} SET IsDeleted = {!value}, LastModifiedBy = {_token.Context.OwnerId}, LastModifiedDate = CURRENT_TIMESTAMP()
                            WHERE RoleId = @RoleId AND ActionId = @ActionId AND TenantId = {_token.Context.TenantId}";
                await _dbConnection.ExecuteAsync(cmd, new { RoleId = roleId, ActionId = actionId });
            }
        }

        //public async Task CreateAccountAsync(CreateAccountDataDto account, CancellationToken cancellationToken)
        //{
        //    var user = _mapper.Map<User>(account);

        //    user.UserId = Guid.NewGuid().ToString();
        //    user.PasswordHash = account.Password.ToMD5();
        //    user.Salt = Utility.RandomString(6);

        //    _dbContext.Users.Add(user);
        //    await _dbContext.SaveChangesAsync(cancellationToken);
        //}
    }
}
