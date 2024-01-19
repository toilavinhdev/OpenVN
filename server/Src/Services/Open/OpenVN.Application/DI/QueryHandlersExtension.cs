using Microsoft.Extensions.DependencyInjection;

namespace OpenVN.Application
{
    public static class QueryHandlersExtension
    {
        public static IServiceCollection AddQueryHandlers(this IServiceCollection services)
        {
            // Config
            services.AddScoped<IRequestHandler<GetConfigQuery, UserConfigDto>, GetConfigQueryHandler>();

            // Location
            services.AddScoped<IRequestHandler<GetProvincesQuery, List<ProvinceDto>>, GetProvincesQueryHandler>();
            services.AddScoped<IRequestHandler<GetDistrictsQuery, List<DistrictDto>>, GetDistrictsQueryHandler>();
            services.AddScoped<IRequestHandler<GetWardsQuery, List<WardDto>>, GetWardsQueryHandler>();
            services.AddScoped<IRequestHandler<GetRankQuery, PagingResult<RankDto>>, GetRankQueryHandler>();

            // Asset
            services.AddScoped<IRequestHandler<PagingSpendingQuery, PagingResult<SpendingDto>>, PagingSpendingQueryHandler>();
            services.AddScoped<IRequestHandler<PagingWithSubSpendingQuery, PagingResult<SpendingDto>>, PagingWithSubSpendingQueryHandler>();

            // Notebook
            services.AddScoped<IRequestHandler<PagingNoteQuery, PagingResult<NoteWithoutContentDto>>, PagingNoteQueryHandler>();
            services.AddScoped<IRequestHandler<PagingNoteCategoryQuery, PagingResult<NoteCategoryDto>>, PagingNoteCategoryQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllNoteCategoryQuery, List<NoteCategoryDto>>, GetAllNoteCategoryQueryHandler>();
            services.AddScoped<IRequestHandler<GetNoteByIdQuery, NoteDto>, GetNoteByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetNotesQuery, List<NoteWithoutContentDto>>, GetNotesQueryHandler>();

            // Cloud
            //services.AddScoped<IRequestHandler<GetDirectoryByIdQuery, DirectoryDto>, GetDirectoryByIdQueryHandler>();
            //services.AddScoped<IRequestHandler<GetAllDirectoryQuery, List<DirectoryDto>>, GetAllDirectoryQueryHandler>();

            return services;
        }
    }
}
