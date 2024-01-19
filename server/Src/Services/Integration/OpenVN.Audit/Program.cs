using Microsoft.AspNetCore.Http;
using OpenVN.Audit.BackgroundJobs;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Configure;
using SharedKernel.Core;
using SharedKernel.Providers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        Console.WriteLine($"Now is {DateTime.Now}");
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        // DI
        CoreSettings.SetConnectionStrings(configuration);
        CoreSettings.SetEmailConfig(configuration);

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

        // Background Job
        services.AddHostedService<AuditBackgroundJob>();
    })
    .UseCoreSerilog()
    .Build();

await host.RunAsync();
