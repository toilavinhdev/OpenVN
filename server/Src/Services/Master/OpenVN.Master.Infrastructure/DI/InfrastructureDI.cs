using Microsoft.Extensions.DependencyInjection;
using OpenVN.Master.Application;
using OpenVN.Master.Application.Repositories;
using OpenVN.Master.Infrastructure.Repositories;

namespace OpenVN.Master.Infrastructure
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IAppReadOnlyRepository, AppReadOnlyRepository>();
            services.AddScoped<IAppWriteOnlyRepository, AppWriteOnlyRepository>();
            return services;
        }
    }
}
