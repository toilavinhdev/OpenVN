using Microsoft.Extensions.DependencyInjection;
using OpenVN.Master.Application.Commands;
using System.Reflection;

namespace OpenVN.Master.Application
{
    public static class QueryHandlersExtension
    {
        public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddScoped<IRequestHandler<GetAppQuery, List<AppDto>>, GetAppQueryHandler>();
            services.AddScoped<IRequestHandler<UpdateFavouriteCommand, Unit>, UpdateFavouriteCommandHandler>();

            return services;
        }
    }
}
