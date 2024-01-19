using Microsoft.Extensions.Configuration;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Core;
using SharedKernel.Libraries;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ISequenceCaching _sequenceCaching;
        private readonly IToken _token;

        public AuthService(
            IAuthRepository repository,
            IConfiguration configuration,
            ISequenceCaching sequenceCaching,
            IToken token
        )
        {
            _repository = repository;
            _configuration = configuration;
            _sequenceCaching = sequenceCaching;
            _token = token;
        }


        public bool CheckPermission(ActionExponent[] exponents)
        {
            for (int i = 0; i < exponents.Length; i++)
            {
                var action = AuthUtility.FromExponentToPermission((int)exponents[i]);
                if (!AuthUtility.ComparePermissionAsString(_token.Context.Permission, action))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckPermission(ActionExponent exponent)
        {
            return CheckPermission(new ActionExponent[] { exponent });
        }

        public async Task<string> GenerateAccessTokenAsync(TokenUser token, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>();
            var secretKey = Encoding.UTF8.GetBytes(_configuration["Auth:JwtSettings:Key"]);
            var symmetricSecurityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(secretKey);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(symmetricSecurityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);

            // add claims
            claims.Add(new Claim(ClaimConstant.TENANT_ID, token.TenantId.ToString()));
            claims.Add(new Claim(ClaimConstant.USER_ID, token.Id.ToString()));
            claims.Add(new Claim(ClaimConstant.USERNAME, token.Username));
            claims.Add(new Claim(ClaimConstant.ROLES, string.Join(",", token.RoleNames)));
            claims.Add(new Claim(ClaimConstant.PERMISSION, token.Permission.ToString()));
            claims.Add(new Claim(ClaimConstant.CREATE_AT, DateHelper.Now.ToString()));
            //claims.Add(new Claim(ClaimConstant.AUTHOR, "Cương Nguyễn"));
            //claims.Add(new Claim(ClaimConstant.ORGANIZATION, "Open VN"));
            //claims.Add(new Claim(ClaimConstant.AUTHORS_MESSAGE, "Contact for work: 0847-88-4444; Facebook: https://facebook.com/cuongnguyen.ftdev"));

            var securityToken = new JwtSecurityToken(
                    issuer: _configuration["Auth:JwtSettings:Issuer"],
                    audience: _configuration["Auth:JwtSettings:Issuer"],
                    claims: claims,
                    expires: DateHelper.Now.AddSeconds(AuthConstant.TOKEN_TIME),
                    signingCredentials: credentials
                );

            var accessToken = tokenHandler.WriteToken(securityToken);

            /**
             * Save token vào redis
             * Nếu chỉ cho phép online trên 1 thiết bị: revoke token cũ, save token mới
             * Nếu cho phép online trên nhiều thiết bị: update token
             */
            var key = BaseCacheKeys.GetAccessTokenKey(token.Id);
            var oldTokens = await _sequenceCaching.GetStringAsync(key);

            if (CoreSettings.IsSingleDevice)
            {
                if (!string.IsNullOrEmpty(oldTokens))
                {
                    var tokens = oldTokens.Split(";");
                    await _repository.RemoveRefreshTokenAsync(cancellationToken);
                    await Task.WhenAll(tokens.Select(token => RevokeAccessTokenAsync(token, cancellationToken)).Concat(new Task[] { _sequenceCaching.RemoveAsync(key, cancellationToken: cancellationToken) }));
                }
                await _sequenceCaching.SetAsync(key, accessToken, TimeSpan.FromSeconds(AuthConstant.TOKEN_TIME));
            }
            else
            {
                var tokenValues = string.IsNullOrEmpty(oldTokens) ? accessToken : $"{oldTokens};{accessToken}";
                await _sequenceCaching.SetAsync(key, tokenValues, TimeSpan.FromSeconds(AuthConstant.TOKEN_TIME));
            }

            return accessToken;
        }

        public string GenerateRefreshToken()
        {
            return Utility.RandomString(128);
        }

        public async Task<bool> CheckRefreshTokenAsync(string value, long ownerId, CancellationToken cancellationToken)
        {
            return await _repository.CheckRefreshTokenAsync(value, ownerId, cancellationToken);
        }

        public async Task RevokeAccessTokenAsync(string accessToken, CancellationToken cancellationToken)
        {
            await _sequenceCaching.SetAsync(BaseCacheKeys.GetRevokeAccessTokenKey(accessToken), DateHelper.Now, TimeSpan.FromSeconds(AuthConstant.TOKEN_TIME));
        }
    }
}
