using Microsoft.AspNetCore.Http;
using OpenVN.BackgroundJob;
using Polly;
using Polly.Extensions.Http;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Configure;
using SharedKernel.Core;
using SharedKernel.Log;
using SharedKernel.Providers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        Console.WriteLine($"Now is {DateTime.Now}");
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        // DI
        CoreSettings.SetConnectionStrings(configuration);
        CoreSettings.SetEmailConfig(configuration);

        services.AddHostedService<SignInBackgroundJob>();

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
        services.AddSingleton<IIntegrationAuthNoticeService, IntegrationAuthNoticeService>();
        services.AddSingleton<IIntegrationAuthRepository, IntegrationAuthRepository>();
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
    })
    .UseCoreSerilog()
    .Build();

await host.RunAsync();
