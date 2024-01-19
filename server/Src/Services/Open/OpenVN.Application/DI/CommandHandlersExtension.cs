using SharedKernel.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace OpenVN.Application
{
    public static class CommandHandlersExtension
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            // Auth
            services.AddScoped<IRequestHandler<SignInCommand, BaseResponse>, SignInCommandHandler>();
            services.AddScoped<IRequestHandler<SignOutCommand, Unit>, SignOutCommandHandler>();
            services.AddScoped<IRequestHandler<RefreshTokenCommand, BaseResponse>, RefreshTokenCommandHandler>();

            // Config
            services.AddScoped<IRequestHandler<UpdateConfigCommand, UserConfigDto>, UpdateConfigCommandHandler>();

            // Cpanel
            services.AddScoped<IRequestHandler<CreateUserCommand, string>, CreateUserCommandHandler>();

            // Process
            services.AddScoped<IRequestHandler<CreateProcessCommand, string>, CreateProcessCommandHandler>();

            // Asset management
            services.AddScoped<IRequestHandler<CreateSpendingCommand, string>, CreateSpendingCommandHandler>();

            // Notebook
            services.AddScoped<IRequestHandler<CreateNoteCommand, string>, CreateNoteCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateNoteCommand, Unit>, UpdateNoteCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteNoteCommand, Unit>, DeleteNoteCommandHandler>();
            services.AddScoped<IRequestHandler<CreateNoteCategoryCommand, string>, CreateNoteCategoryCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateNoteCategoryCommand, Unit>, UpdateNoteCategoryCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteNoteCategoryCommand, Unit>, DeleteNoteCategoryCommandHandler>();

            // Cloud
            services.AddScoped<IRequestHandler<CreateCodeCommand, string>, CreateCodeCommandHandler>();
            services.AddScoped<IRequestHandler<UploadCommand, List<CloudFileDto>>, UploadCommandHandler>();

            return services;
        }
    }
}
