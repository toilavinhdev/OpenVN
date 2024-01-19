﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Ocelot.Infrastructure.Extensions;
using SharedKernel.Application;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using System.Net;

namespace SharedKernel.Filters
{
    public class AccessTokenValidatorAsyncFilter : IAsyncResourceFilter
    {
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (AuthUtility.EndpointRequiresAuthorize(context))
            {
                var bearerToken = context.HttpContext.Request.Headers[HeaderNames.Authorization];
                if (!string.IsNullOrEmpty(bearerToken.GetValue()))
                {
                    var token = context.HttpContext.RequestServices.GetRequiredService<IToken>();
                    var sequenceCaching = context.HttpContext.RequestServices.GetRequiredService<ISequenceCaching>();
                    var accessToken = bearerToken.GetValue()[7..];
                    var key = BaseCacheKeys.GetRevokeAccessTokenKey(accessToken);
                    var isRevoked = !string.IsNullOrEmpty(await sequenceCaching.GetStringAsync(key));

                    // If the user is logged out
                    if (isRevoked || token.Context.TenantId <= 0)
                    {
                        context.Result = new ContentResult()
                        {
                            StatusCode = (int)HttpStatusCode.Unauthorized,
                            ContentType = "application/json"
                        };
                        return;
                    }
                }
            }
            //else
            //{
            //    context.HttpContext.Request.Headers[HeaderNames.Authorization] = string.Empty;
            //}
            await next();
        }
    }
}
