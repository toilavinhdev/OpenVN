using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using SharedKernel.Application;
using System.Net;
using SharedKernel.Log;
using SharedKernel.Runtime.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using SharedKernel.Properties;
using SharedKernel.Core;
using SharedKernel.Libraries;

namespace SharedKernel.Middlewares
{
    public class Reject3pMiddleware
    {
        private readonly RequestDelegate _next;

        public Reject3pMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var header = context.Request.Headers;
            var pass3p = !string.IsNullOrEmpty(header[HeaderNamesExtension.Pass3p]);
            if (!pass3p)
            {
                var ua = header[HeaderNames.UserAgent].ToString().ToLower();
                foreach (var bkw in CoreSettings.Black3pKeywords)
                {
                    var _3p = "";
                    if (!string.IsNullOrEmpty(header["Postman-Token"])) _3p = "Postman";
                    else if (ua.Contains(bkw)) _3p = bkw;

                    if (!string.IsNullOrEmpty(_3p))
                    {
                        Logging.Warning($"Attack from third party [{_3p}] with ip {AuthUtility.TryGetIP(context.Request)}");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.ContentType = "application/json";

                        var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<Resources>>();
                        var error = new Error(400, localizer["not_support_3p_tool"], "NOT_SUPPORTED");
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BaseResponse(error), new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }));
                        return;
                    }
                }
            }
            await _next(context);
        }
    }

    public static class Reject3pMiddlewareExtension
    {
        public static IApplicationBuilder UseReject3P(this IApplicationBuilder app)
        {
            return app.UseMiddleware<Reject3pMiddleware>();
        }
    }
}
