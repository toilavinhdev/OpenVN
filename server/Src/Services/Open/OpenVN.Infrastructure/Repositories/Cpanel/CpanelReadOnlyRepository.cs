using Microsoft.Extensions.DependencyInjection;
using OpenVN.Application.Dto.Cpanel;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using Action = OpenVN.Domain.Action;

namespace OpenVN.Infrastructure
{
    public class CpanelReadOnlyRepository : BaseReadOnlyRepository<BaseEntity>, ICpanelReadOnlyRepository
    {
        public CpanelReadOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IServiceProvider provider
        ) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken)
        {
            var result = new List<RoleDto>();
            var multiQuery = @$"SELECT * FROM {new Role().GetTableName()} WHERE TenantId = {_token.Context.TenantId} AND IsDeleted = 0;
                               SELECT * FROM {new Action().GetTableName()} WHERE TenantId = {_token.Context.TenantId} AND IsDeleted = 0";
            var grid = await _dbConnection.QueryMultipleAsync(multiQuery);
            var roles = (await grid.ReadAsync<Role>()).ToList();
            var actions = (await grid.ReadAsync<Action>()).ToList();

            if (roles.Any())
            {
                var sp = JsonHelper.GetConfiguration("auth-stored-procedure.json")["GetRoleActions"];
                var roleActions = await _dbConnection.QueryAsync<RoleAction>(sp, new { v_tenant_id = _token.Context.TenantId }, commandType: System.Data.CommandType.StoredProcedure);
                foreach (var role in roles)
                {
                    var actionDto = actions.Select(action =>
                    {
                        var dto = new ActionDto
                        {
                            Id = action.Id.ToString(),
                            Code = action.Code,
                            Name = action.Name,
                            Description = action.Description,
                            Exponent = action.Exponent
                        };
                        if (roleActions.FirstOrDefault(x => x.RoleId == role.Id && x.ActionId == action.Id) != null)
                        {
                            dto.IsContain = true;
                        }
                        return dto;
                    });

                    result.Add(new RoleDto
                    {
                        Id = role.Id.ToString(),
                        RoleCode = role.Code,
                        RoleName = role.Name,
                        Actions = actionDto.ToList()
                    });
                }
            }

            return result;
        }

        public async Task<List<Action>> GetActionsByExponentsAsync(List<SharedKernel.Application.Enum.ActionExponent> exponents, CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM {new Action().GetTableName()} WHERE TenantId = {_token.Context.TenantId} AND Exponent IN ( {string.Join(",", exponents.Select(x => (int) x))} AND IsDeleted = 0)";
            return (await _dbConnection.QueryAsync<Action>(query)).ToList();
        }


        public async Task<List<RecordDashboardDto>> GetRecordDashboardAsync(CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("auth-stored-procedure.json")["GetRecordDashboard"];
            var result = await _dbConnection.QueryAsync<RecordDashboardDto>(sp, new { v_tenant_id = _token.Context.TenantId }, commandType: System.Data.CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<PagingResult<UserDto>> GetUsersPagingAsync(PagingRequest request, CancellationToken cancellationToken)
        {
            var brr = _provider.GetRequiredService<IBaseReadOnlyRepository<User>>();
            return await brr.GetPagingAsync<UserDto>(request, cancellationToken);
        }

        public async Task<List<User>> GetUsersByRoleId(object roleId, CancellationToken cancellationToken)
        {
            var query = @$"SELECT u.* FROM {new User().GetTableName()} u 
                           INNER JOIN {new UserRole().GetTableName()} ur ON u.Id = ur.UserId 
                           WHERE u.TenantId = {_token.Context.TenantId} AND 
                                 ur.TenantId = {_token.Context.TenantId} AND
                                 ur.RoleId = @RoleId AND
                                 u.IsDeleted = 0 AND
                                 ur.IsDeleted = 0";

            return (await _dbConnection.QueryAsync<User>(query, new { RoleId = roleId })).ToList();
        }
    }
}
