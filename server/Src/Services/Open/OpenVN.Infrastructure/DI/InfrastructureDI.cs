using OpenVN.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenVN.Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            var connectionString = configuration.GetConnectionString("MasterDb");
            services.AddDbContext<EfDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                   .LogTo(s => System.Diagnostics.Debug.WriteLine(s))
                   .EnableDetailedErrors(true)
                   .EnableSensitiveDataLogging(true)
            );

            // Base
            services.AddScoped(typeof(IBaseReadOnlyRepository<>), typeof(BaseReadOnlyRepository<>));
            services.AddScoped(typeof(IBaseWriteOnlyRepository<>), typeof(BaseWriteOnlyRepository<>));

            // Auth
            services.AddScoped<IAuthRepository, AuthRepository>();

            // Tenant
            services.AddScoped<ITenantReadOnlyRepository, TenantReadOnlyRepository>();

            // Audit
            services.AddScoped<IAuditReadOnlyRepository, AuditReadOnlyRepository>();

            // Notification
            services.AddScoped<INotificationReadOnlyRepository, NotificationReadOnlyRepository>();
            services.AddScoped<INotificationWriteOnlyRepository, NotificationWriteOnlyRepository>();

            // Config
            services.AddScoped<IConfigReadOnlyRepository, ConfigReadOnlyRepository>();
            services.AddScoped<IConfigWriteOnlyRepository, ConfigWriteOnlyRepository>();

            // Cpanel
            services.AddScoped<ICpanelReadOnlyRepository, CpanelReadOnlyRepository>();
            services.AddScoped<ICpanelWriteOnlyRepository, CpanelWriteOnlyRepository>();

            // Location
            services.AddScoped<ILocationReadOnlyRepository, LocationReadOnlyRepository>();

            // User
            services.AddScoped<IUserWriteOnlyRepository, UserWriteOnlyRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();

            // Process
            services.AddScoped<IProcessWriteOnlyRepository, ProcessWriteOnlyRepository>();

            // Asset
            services.AddScoped<ISpendingWriteOnlyRepository, SpendingWriteOnlyRepository>();
            services.AddScoped<ISpendingReadOnlyRepository, SpendingReadOnlyRepository>();

            // Notebook
            services.AddScoped<INoteWriteOnlyRepository, NoteWriteOnlyRepository>();
            services.AddScoped<INoteReadOnlyRepository, NoteReadOnlyRepository>();
            services.AddScoped<INoteCategoryWriteOnlyRepository, NoteCategoryWriteOnlyRepository>();
            services.AddScoped<INoteCategoryReadOnlyRepository, NoteCategoryReadOnlyRepository>();

            // Cloud
            services.AddScoped<IDirectoryReadOnlyRepository, DirectoryReadOnlyRepository>();
            services.AddScoped<IDirectoryWriteOnlyRepository, DirectoryWriteOnlyRepository>();
            services.AddScoped<ILockedDirectoryReadOnlyRepository, LockedDirectoryReadOnlyRepository>();
            services.AddScoped<ILockedDirectoryWriteOnlyRepository, LockedDirectoryWriteOnlyRepository>();
            services.AddScoped<ICloudConfigReadOnlyRepository, CloudConfigReadOnlyRepository>();
            services.AddScoped<ICloudFileReadOnlyRepository, CloudFileReadOnlyRepository>();
            services.AddScoped<ICloudFileWriteOnlyRepository, CloudFileWriteOnlyRepository>();
            services.AddScoped<IMoveCloudObjectRepository, MoveCloudObjectRepository>();

            // Chat generator
            services.AddScoped<IChatGeneratorWriteOnlyRepository, ChatGeneratorWriteOnlyRepository>();
            services.AddScoped<IChatGeneratorReadOnlyRepository, ChatGeneratorReadOnlyRepository>();

            // Feedback
            services.AddScoped<IFeedbackReadOnlyRepository, FeedbackReadOnlyRepository>();
            services.AddScoped<IFeedbackWriteOnlyRepository, FeedbackWriteOnlyRepository>();

            return services;
        }
    }
}
