using SharedKernel.Auth;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.UnitOfWork;

namespace OpenVN.Infrastructure
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IToken _token;
        private readonly IServiceProvider _provider;

        public AuthRepository(IDbConnection dbConnection, IToken token, IServiceProvider provider)
        {
            _dbConnection = dbConnection;
            _token = token;
            _provider = provider;
        }
        public IUnitOfWork UnitOfWork => _dbConnection;

        public async Task<TokenUser> GetTokenUserByIdentityAsync(string username, string password, CancellationToken cancellationToken)
        {
            var configurationRoot = JsonHelper.GetConfiguration("auth-stored-procedure.json");
            var tokenUser = await _dbConnection.QueryFirstOrDefaultAsync<TokenUser>(configurationRoot["SignIn"], new { v_username = username, v_password_hash = password.ToMD5() }, commandType: System.Data.CommandType.StoredProcedure);
            if (tokenUser == null)
                return null;

            var roleActions = await _dbConnection.QueryAsync<RoleAction>(configurationRoot["GetRoleAction"], new { v_user_id = tokenUser.Id }, commandType: System.Data.CommandType.StoredProcedure);
            var sa = roleActions.FirstOrDefault(x => x.RoleCode.Equals("SUPER_ADMIN"));
            var admin = roleActions.FirstOrDefault(x => x.RoleCode.Equals("ADMIN"));

            if (sa != null)
            {
                tokenUser.Permission = AuthUtility.CalculateToTalPermision(Enumerable.Range(0, sa.Exponent + 1));
            }
            else if (admin != null)
            {
                tokenUser.Permission = AuthUtility.CalculateToTalPermision(Enumerable.Range(0, admin.Exponent + 1));
            }
            else
            {
                tokenUser.Permission = AuthUtility.CalculateToTalPermision(roleActions.Select(x => x.Exponent));
            }

            tokenUser.RoleNames = roleActions.DistinctBy(x => x.RoleName).Select(x => x.RoleName).ToList();
            return tokenUser;
        }

        public async Task<TokenUser> GetTokenUserByOwnerIdAsync(long ownerId, CancellationToken cancellationToken)
        {
            var cmd = $"SELECT * FROM {new User().GetTableName()} WHERE Id = @Id AND IsDeleted = 0";
            var tokenUser = await _dbConnection.QuerySingleOrDefaultAsync<TokenUser>(cmd, new { Id = ownerId });
            if (tokenUser == null)
                return null;

            var configurationRoot = JsonHelper.GetConfiguration("auth-stored-procedure.json");
            var roleActions = await _dbConnection.QueryAsync<RoleAction>(configurationRoot["GetRoleAction"], new { v_user_id = tokenUser.Id }, commandType: System.Data.CommandType.StoredProcedure);
            var sa = roleActions.FirstOrDefault(x => x.RoleCode.Equals("SUPER_ADMIN"));
            var admin = roleActions.FirstOrDefault(x => x.RoleCode.Equals("ADMIN"));

            if (sa != null)
            {
                tokenUser.Permission = AuthUtility.CalculateToTalPermision(Enumerable.Range(0, sa.Exponent + 1));
            }
            else if (admin != null)
            {
                tokenUser.Permission = AuthUtility.CalculateToTalPermision(Enumerable.Range(0, admin.Exponent + 1));
            }
            else
            {
                tokenUser.Permission = AuthUtility.CalculateToTalPermision(roleActions.Select(x => x.Exponent));
            }

            tokenUser.RoleNames = roleActions.DistinctBy(x => x.RoleName).Select(x => x.RoleName).ToList();
            return tokenUser;
        }

        public async Task<bool> CheckRefreshTokenAsync(string value, long ownerId, CancellationToken cancellationToken)
        {
            var cmd = $"SELECT Id FROM {new RefreshToken().GetTableName()} WHERE RefreshTokenValue = @RefreshTokenValue AND ExpriedDate >= @ExpriedDate AND OwnerId = @OwnerId AND IsDeleted = 0";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<RefreshToken>(cmd, new
            {
                RefreshTokenValue = value,
                ExpriedDate = DateHelper.Now,
                OwnerId = ownerId,
            });

            return result != null;
        }

        public async Task CreateOrUpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            var store = JsonHelper.GetConfiguration("auth-stored-procedure.json")["CreateOrUpdateRefreshToken"];
            var param = new
            {
                v_owner_id = refreshToken.OwnerId,
                v_refresh_token_value = refreshToken.RefreshTokenValue,
                v_current_access_token = refreshToken.CurrentAccessToken,
                v_expried_date = refreshToken.ExpriedDate,
            };

            await _dbConnection.ExecuteAsync(store, param, autoCommit: true, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task RemoveRefreshTokenAsync(CancellationToken cancellationToken)
        {
            var sqlCommand = $"UPDATE {new RefreshToken().GetTableName()} SET IsDeleted = 1 WHERE CurrentAccessToken = @CurrentAccessToken";
            await _dbConnection.ExecuteAsync(sqlCommand, new { CurrentAccessToken = _token.Context.AccessToken }, autoCommit: true);
        }

        public async Task SignOutAsync(CancellationToken cancellationToken)
        {
            await RemoveRefreshTokenAsync(cancellationToken);
        }

        public async Task SetRoleForUserAsync(long userId, List<long> roleIds, CancellationToken cancellationToken)
        {
            var store = JsonHelper.GetConfiguration("auth-stored-procedure.json")["SetRoleForUser"];
            var param = new
            {
                v_user_id = userId,
                v_role_ids = string.Join(",", roleIds)
            };

            await _dbConnection.ExecuteAsync(store, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public Task<bool> VerifySecretKeyAsync(string secretKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<PagingResult<SignInHistoryDto>> GetSignInHistoryPaging(PagingRequest request, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT Id, TenantId, Username, Origin, Ip, Browser, OS, Device, UA, CreatedDate, City, Country, Lat, `Long`, Timezone, Org, Postal, DATE_ADD(SignInTime, INTERVAL 7 hour) AS SignInTime 
                             FROM {new SignInHistory().GetTableName()} 
                             WHERE Username = @Username AND TenantId = {_token.Context.TenantId} 
                             ORDER BY SignInTime DESC LIMIT {request.Offset}, {request.Size};";
            var countCmd = $"SELECT COUNT(*) FROM {new SignInHistory().GetTableName()} WHERE Username = @Username AND TenantId = {_token.Context.TenantId};";
            var dataTask = _dbConnection.QueryAsync<SignInHistoryDto>(cmd, new { _token.Context.Username });
            var countTask = _dbConnection.QueryFirstOrDefaultAsync<long>(countCmd, new { _token.Context.Username });

            await Task.WhenAll(dataTask, countTask);
            return new PagingResult<SignInHistoryDto>
            {
                Data = await dataTask,
                Count = await countTask,
            };
        }
    }
}
