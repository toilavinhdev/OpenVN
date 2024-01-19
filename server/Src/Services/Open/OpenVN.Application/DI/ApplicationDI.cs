using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OpenVN.Application
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOpenMessageHub, OpenMessageHub>();

            // AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddQueryHandlers();
            services.AddCommandHandlers();

            // Cloud
            services.AddScoped<ICloudConfigService, CloudConfigService>();
            services.AddScoped<ILockDirectoryService, LockDirectoryService>();

            // User
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
