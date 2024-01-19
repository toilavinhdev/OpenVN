using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenVN.Master.Application;
using OpenVN.Master.Infrastructure;
using SharedKernel.Application;
using SharedKernel.Configure;
using SharedKernel.Core;
using SharedKernel.Filters;
using SharedKernel.Middlewares;
using SharedKernel.Providers;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
#pragma warning disable CS0612 // Type or member is obsolete
builder.WebHost.UseCoreSerilog();
#pragma warning restore CS0612 // Type or member is obsolete

// Add services to the container.
var services = builder.Services;

CoreSettings.SetConnectionStrings(builder.Configuration);
CoreSettings.SetBlack3pKeywords(builder.Configuration);

services.AddSingleton(_ => builder.Configuration);

services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
    x.MultipartHeadersLengthLimit = int.MaxValue;
});


services.AddCors();

services.AddCoreLocalization();

services.AddCoreRateLimit();

#region AddController + CamelCase + FluentValidation
services.AddControllersWithViews()
        .AddJsonOptions(options =>
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        )
        .AddFluentValidation(delegate (FluentValidationMvcConfiguration f)
        {
            f.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic));
        })
        .ConfigureApiBehaviorOptions(delegate (ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = delegate (ActionContext c)
            {
                string errors = string.Join(", ", from v in c.ModelState.Values.Where((ModelStateEntry v) => v.Errors.Any()).SelectMany((ModelStateEntry v) => v.Errors) select v.ErrorMessage);
                return new OkObjectResult(new BaseResponse
                {
                    Error = new Error(HttpStatusCode.BadRequest, errors)
                });
            };
        });
#endregion

services.AddCoreORM();

services.AddCoreCaching(builder.Configuration);

services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AccessTokenValidatorAsyncFilter());
});
services.Configure<ForwardedHeadersOptions>(o => o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

services.AddToken();

services.AddCoreAuthentication(builder.Configuration);

services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>();

services.AddCoreRabbitMq();

services.AddDispatchers();

services.AddQueryHandlers();

services.AddInfrastructureServices();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseCoreLocalization();

app.UseCoreExceptionHandler();

app.UseCoreCors(builder.Configuration);

app.UseIpRateLimiting();

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseCoreUnauthorized();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();