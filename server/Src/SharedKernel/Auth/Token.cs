using SharedKernel.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace SharedKernel.Auth
{
    public class Token : IToken
    {
        #region Properties
        private readonly IHttpContextAccessor _accessor;
        private ExecutionContext _context { get; set; }
        public string TokenId => Guid.NewGuid().ToString();
        public ExecutionContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = GetContext(GetAccessToken());
                }
                return _context;
            }
            set { _context = value; }
        }
        #endregion

        #region Constructors
        public Token(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        #endregion

        #region Private
        private string GetAccessToken()
        {
            var bearerToken = _accessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString();
            if (string.IsNullOrEmpty(bearerToken) || bearerToken.Equals("Bearer"))
            {
                return "";
            }
            return bearerToken.Substring(7);
        }

        private ExecutionContext GetContext(string accessToken)
        {
            var httpContext = _accessor.HttpContext;
            if (string.IsNullOrEmpty(accessToken))
            {
                return new ExecutionContext { HttpContext = httpContext };
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(accessToken);
            var claims = jwtSecurityToken.Claims;
            return new ExecutionContext
            {
                AccessToken = accessToken,
                OwnerId = Convert.ToInt64(claims.First(c => c.Type == ClaimConstant.USER_ID).Value),
                Username = claims.First(c => c.Type == ClaimConstant.USERNAME).Value,
                Permission = claims.First(c => c.Type == ClaimConstant.PERMISSION).Value,
                TenantId =  Convert.ToInt64(claims.First(c => c.Type == ClaimConstant.TENANT_ID).Value),
                HttpContext = httpContext
            };
        }
        #endregion
    }
}