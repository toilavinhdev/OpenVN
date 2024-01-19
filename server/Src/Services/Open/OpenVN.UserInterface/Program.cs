using Microsoft.AspNetCore.HttpOverrides;
using OpenVN.Infrastructure;
using SharedKernel.Configure;
using SharedKernel.Core;
using SharedKernel.Filters;

var builder = WebApplication.CreateBuilder(args);
#pragma warning disable CS0612 // Type or member is obsolete
builder.WebHost.UseCoreSerilog();
#pragma warning restore CS0612 // Type or member is obsolete

// Add services to the container.
var services = builder.Services;

CoreSettings.SetEmailConfig(builder.Configuration);
CoreSettings.SetS3Config(builder.Configuration);

services.AddCoreServices(builder.Configuration);

services.AddCoreAuthentication(builder.Configuration);

services.AddCoreORM();

services.AddCoreCaching(builder.Configuration);

services.AddHealthChecks();

services.Configure<ForwardedHeadersOptions>(o => o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

services.AddCoreProviders();

services.AddSignalR();

services.AddCoreRabbitMq();

services.AddToken();

services.AddDispatchers();

services.AddExceptionHandler();

services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AccessTokenValidatorAsyncFilter());
});

services.AddApplicationServices(builder.Configuration);

services.AddInfrastructureServices(builder.Configuration);

// Configure the HTTP request pipeline.
var app = builder.Build();
app.UseCoreCors(builder.Configuration);
app.UseCoreConfigure(app.Environment);
app.Run();