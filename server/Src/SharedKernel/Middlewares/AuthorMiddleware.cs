using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SharedKernel.Middlewares
{
    public class AuthorMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("openvn-author", "Cuong Nguyen");
            context.Response.Headers.Add("openvn-facebook", "https://facebook.com/cuongnguyen.ftdev");
            context.Response.Headers.Add("openvn-email", "cuongnguyen.ftdev@gmail.com");
            context.Response.Headers.Add("openvn-contact", "0847-88-4444");

            await _next(context);
        }

    }

    public static class AuthorMiddlewareMiddlewareExtension
    {
        public static IApplicationBuilder UseCoreAuthor(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthorMiddleware>();
        }
    }
}
