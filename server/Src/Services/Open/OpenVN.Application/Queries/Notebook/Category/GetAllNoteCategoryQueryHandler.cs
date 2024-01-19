using AutoMapper;

namespace OpenVN.Application
{
    public class GetAllNoteCategoryQueryHandler : BaseQueryHandler, IRequestHandler<GetAllNoteCategoryQuery, List<NoteCategoryDto>>
    {
        private readonly INoteCategoryReadOnlyRepository _noteCategoryReadOnlyRepository;

        public GetAllNoteCategoryQueryHandler(INoteCategoryReadOnlyRepository noteCategoryReadOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _noteCategoryReadOnlyRepository = noteCategoryReadOnlyRepository;
        }

        public async Task<List<NoteCategoryDto>> Handle(GetAllNoteCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _noteCategoryReadOnlyRepository.GetAllAsync<NoteCategoryDto>(cancellationToken);
            return result.ToList();
        }
    }
}
