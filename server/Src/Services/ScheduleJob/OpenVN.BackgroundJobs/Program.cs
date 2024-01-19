using Microsoft.AspNetCore.Http;
using OpenVN.BackgroundJobs;
using Polly;
using Polly.Extensions.Http;
using SharedKernel.Application;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Configure;
using SharedKernel.Core;
using SharedKernel.Infrastructures;
using SharedKernel.Log;
using SharedKernel.MySQL;
using SharedKernel.Providers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        // DI
        CoreSettings.SetConnectionStrings(configuration);
        CoreSettings.SetEmailConfig(configuration);

        services.AddSingleton<IDbConnection, DbConnection>();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetRequiredSection("Redis").Value;
            options.InstanceName = "";
        });
        services.AddCoreRabbitMq();
        services.AddExceptionHandler();
        services.AddCoreProviders();
        services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>();
        services.AddSingleton<IDistributedRedisCache, DistributedRedisCache>();
        services.AddSingleton<IMemoryCaching, MemoryCaching>();
        services.AddSingleton<ISequenceCaching, SequenceCaching>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IToken, Token>();
        services.AddSingleton<ITenantReadOnlyRepository, TenantReadOnlyRepository>();
        services.AddSingleton<ITenantReadOnlyRepository, TenantReadOnlyRepository>();
        services.AddHttpClient("")
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(
                        HttpPolicyExtensions
                                .HandleTransientHttpError()
                                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                                .WaitAndRetryAsync(1, retryAttempt =>
                                {
                                    Logging.Warning("http retry...");
                                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                                })
             );

        // Background Job
        //services.AddHostedService<ProcessBackgroundJob>();
        services.AddHostedService<KeepApiRunningBackgroundJob>();
    })
    .UseCoreSerilog()
    .Build();

await host.RunAsync();


