using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using KSharedKernel.RabbitMQ;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using RabbitMQ.Client;
using Serilog;
using SharedKernel.ApiGateway;
using SharedKernel.Application;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Core;
using SharedKernel.Domain;
using SharedKernel.DomainEvents;
using SharedKernel.Infrastructures;
using SharedKernel.Libraries;
using SharedKernel.Log;
using SharedKernel.Middlewares;
using SharedKernel.MongoDB;
using SharedKernel.MySQL;
using SharedKernel.Properties;
using SharedKernel.Providers;
using SharedKernel.RabbitMQ;
using SharedKernel.Runtime;
using SharedKernel.Runtime.Exceptions;
using SharedKernel.SignalR;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using IExceptionHandler = SharedKernel.Runtime.IExceptionHandler;

namespace SharedKernel.Configure
{
    public static class ConfigureExtension
    {
        #region DependencyInjection
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration Configuration)
        {
            CoreSettings.SetConnectionStrings(Configuration);
            CoreSettings.SetBlack3pKeywords(Configuration);

            services.AddSingleton(_ => Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            //services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            //services.AddResponseCompression(options =>
            //{
            //    options.EnableForHttps = true;
            //    options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
            //    options.Providers.Add<GzipCompressionProvider>();
            //});

            services.AddCors();

            services.AddCoreLocalization();

            services.AddCoreRateLimit();

            services.AddCoreBehaviors();

            #region AddController + CamelCase + FluentValidation
            services.AddControllersWithViews()
                    .AddJsonOptions(options =>
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    )
                    .AddFluentValidation(delegate (FluentValidationMvcConfiguration f)
                    {
                        f.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic));
                        //f.RegisterValidatorsFromAssembly(Assembly.GetEntryAssembly());
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

            return services;
        }

        public static IServiceCollection AddCoreLocalization(this IServiceCollection services)
        {
            var supportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("vi-VN") };
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(culture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new[] { new RouteDataRequestCultureProvider() };
            });

            return services;
        }

        public static IServiceCollection AddCoreRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var appSettingConfiguration = sp.GetRequiredService<IApplicationConfiguration>();
                var rabbitConfig = appSettingConfiguration.GetConfiguration<Rabbit>();
                return new ConnectionFactory
                {
                    HostName = rabbitConfig.Host,
                    UserName = rabbitConfig.Username,
                    Password = rabbitConfig.Password,
                    VirtualHost = rabbitConfig.VirtualHost,
                    Port = AmqpTcpEndpoint.UseDefaultPort,
                };
            });
            services.AddScoped<IRabbitMqClientBase, RabbitMqClientBase>();

            return services;
        }

        public static IServiceCollection AddCoreAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = Configuration["Auth:JwtSettings:Issuer"],
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Auth:JwtSettings:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:JwtSettings:Key"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };

                jwtOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/socket-message"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }

        public static IServiceCollection AddCoreCaching(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetRequiredSection("Redis").Value;
                options.InstanceName = "";
            });

            services.AddTransient<IDistributedRedisCache, DistributedRedisCache>();
            services.AddSingleton<IMemoryCaching, MemoryCaching>();
            services.AddTransient<ISequenceCaching, SequenceCaching>();
            return services;
        }

        public static IServiceCollection AddCoreORM(this IServiceCollection services)
        {
            services.AddScoped(typeof(IMongoService<>), typeof(MongoService<>));
            services.AddScoped<IDbConnection, DbConnection>();
            return services;
        }

        public static IServiceCollection AddCoreProviders(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>();
            services.AddSingleton<IDistributedCacheUserProvider, DistributedCacheUserProvider>();
            services.AddScoped<IS3StorageProvider, S3StorageProvider>();
            return services;
        }

        public static IServiceCollection AddToken(this IServiceCollection services)
        {
            services.AddScoped<IToken, Token>();
            return services;
        }

        public static IServiceCollection AddDispatchers(this IServiceCollection services)
        {
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            return services;
        }

        public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionHandler, ExceptionHandler>();
            return services;
        }

        public static IServiceCollection AddCoreRateLimit(this IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;
                options.RealIpHeader = HeaderNamesExtension.RealIpHeader;
                options.ClientIdHeader = HeaderNamesExtension.ClientIdHeader;
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Period = "1s",
                        Limit = 8,
                    }
                };
            });

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();

            return services;
        }

        public static IServiceCollection AddCoreBehaviors(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EventsBehavior<,>));
            return services;
        }
        #endregion

        #region Middlewares
        public static void UseCoreConfigure(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            //app.UseCoreAuthor();
            app.UseCoreLocalization();
            if (!environment.IsDevelopment())
            {
                app.UseReject3P();
            }
            app.UseCoreExceptionHandler();
            app.UseIpRateLimiting();
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseCoreUnauthorized();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MessageHub>("/socket-message");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseCoreHealthChecks();
        }

        public static void UseCoreCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            var origins = configuration.GetRequiredSection("Allowedhosts").Value;
            if (origins.Equals("*"))
            {
                app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            }
            else
            {
                app.UseCors(x => x.WithOrigins(origins.Split(";")).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            }
        }

        public static void UseCoreLocalization(this IApplicationBuilder app)
        {
            //app.Use(async (context, next) =>
            //{
            //    var culture = context.Request.Headers[HeaderNames.AcceptLanguage].ToString();
            //    switch (culture)
            //    {
            //        case "vi":
            //            Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN");
            //            Thread.CurrentThread.CurrentUICulture = new CultureInfo("vi-VN");
            //            break;
            //        default:
            //            break;
            //    }

            //    await next();
            //});

            var supportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("vi-VN") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
        }

        public static void UseCoreHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = (HealthCheckRegistration _) => true,
                ResponseWriter = new Func<HttpContext, HealthReport, Task>(UIResponseWriter.WriteHealthCheckUIResponse)
            });
        }

        public static void UseCoreSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "");
                c.RoutePrefix = string.Empty;
            });
        }

        public static void UseCoreExceptionHandler(this IApplicationBuilder app)
        {
            // Handle exception
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;
                var responseContent = new BaseResponse();
                var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<Resources>>();

                // Catchable
                if (exception is CatchableException)
                {
                    responseContent.Error = new Error(500, exception.Message);
                    Logging.Error(exception);
                }
                // For bi đần
                else if (exception is ForbiddenException)
                {
                    responseContent.Error = new Error(403, localizer["not_permission"].Value);
                }
                // Sql Injection
                else if (exception is SqlInjectionException)
                {
                    responseContent.Error = new Error(400, Secure.MsgDetectedSqlInjection);
                    Logging.Error(exception);
                }
                // Bad request
                else if (exception is BadRequestException)
                {
                    if ((exception as BadRequestException).Body != null)
                    {
                        responseContent = new SimpleDataResult
                        {
                            Data = (exception as BadRequestException).Body,
                            Error = new Error(400, exception.Message, (exception as BadRequestException).Type)
                        };
                    }
                    else
                    {
                        responseContent.Error = new Error(400, exception.Message, (exception as BadRequestException).Type);
                    }
                }
                // Unknown exception
                else
                {
                    responseContent.Error = new Error(500, localizer["system_error_occurred"].Value);
                    Logging.Error(exception);
                }

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(responseContent, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }));
        }
        #endregion

        #region ELK
        public static IHostBuilder UseCoreSerilog(this IHostBuilder builder) => builder.UseSerilog((context, loggerConfiguration) =>
        {
            CoreSettings.SetElasticSearchConfig(context.Configuration);
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", DefaultElasticSearchConfig.ApplicationName)
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level}] {Message}{NewLine}{Exception}")
                .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(DefaultElasticSearchConfig.Uri))
                {
                    IndexFormat = $"{DefaultElasticSearchConfig.ApplicationName}-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfReplicas = 1,
                    NumberOfShards = 2,
                    ModifyConnectionSettings = x => x.BasicAuthentication(DefaultElasticSearchConfig.Username, DefaultElasticSearchConfig.Password)
                })
                .ReadFrom.Configuration(context.Configuration);
        })
        .ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var configuration = sp.GetRequiredService<IConfiguration>();

            CoreSettings.SetLoggingConfig(configuration, logger);
        });


        [Obsolete]
        public static IWebHostBuilder UseCoreSerilog(this IWebHostBuilder builder) =>
        builder.UseSerilog((context, loggerConfiguration) =>
        {
            CoreSettings.SetElasticSearchConfig(context.Configuration);
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", DefaultElasticSearchConfig.ApplicationName)
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level}] {Message}{NewLine}{Exception}")
                .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(DefaultElasticSearchConfig.Uri))
                {
                    IndexFormat = $"{DefaultElasticSearchConfig.ApplicationName}-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfReplicas = 1,
                    NumberOfShards = 2,
                    ModifyConnectionSettings = x => x.BasicAuthentication(DefaultElasticSearchConfig.Username, DefaultElasticSearchConfig.Password)
                })
                .ReadFrom.Configuration(context.Configuration);
        })
        .ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            var logger = sp.GetRequiredService<ILogger>();
            var configuration = sp.GetRequiredService<IConfiguration>();

            CoreSettings.SetLoggingConfig(configuration, logger);
        });
        #endregion

        #region ApiGateway
        [Obsolete]
        public static IWebHostBuilder ConfigCoreApiGateway(this IWebHostBuilder builder) =>
        builder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Limits.MaxRequestBodySize = int.MaxValue;
                })
                .ConfigureAppConfiguration(config =>
                     config.AddJsonFile("appsettings.json", false, true)
                           .AddJsonFile("ocelot.json"))
                           .ConfigureServices(services =>
                           {
                               services.AddSingleton(builder);
                               services.AddOcelot().AddPolly();
                               services.AddCors();
                           })
                           .UseCoreSerilog()
                           .Configure(app =>
                           {
                               app.UseCoreExceptionHandler();
                               app.UseCoreCors(app.ApplicationServices.GetRequiredService<IConfiguration>());
                               app.UseMiddleware<RequestResponseLoggingMiddleware>();
                               app.UseOcelot().Wait();
                           });
        #endregion
    }
}
