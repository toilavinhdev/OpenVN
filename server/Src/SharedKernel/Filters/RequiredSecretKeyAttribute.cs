using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Runtime.Exceptions;

namespace SharedKernel.Filters
{
    public class RequiredSecretKeyAttribute : ActionFilterAttribute
    {
        private readonly string _keyName;

        public RequiredSecretKeyAttribute(string keyName)
        {
            _keyName = keyName;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var code = context.HttpContext.Request.Headers[HeaderNamesExtension.SecretKey].ToString();
            if (string.IsNullOrEmpty(code))
            {
                throw new ForbiddenException();
            }

            var token = context.HttpContext.RequestServices.GetRequiredService<IToken>();
            var sequenceCaching = context.HttpContext.RequestServices.GetRequiredService<ISequenceCaching>();
            var key = BaseCacheKeys.GetSecretKey(_keyName, token.Context.TenantId, token.Context.OwnerId);
            var value = await sequenceCaching.GetStringAsync(key);

            if (!code.Equals(value))
            {
                throw new ForbiddenException();
            }
            await next();
        }
    }
}
