using AutoMapper;

namespace OpenVN.Application
{
    public class PagingNoteCategoryQueryHandler : BaseQueryHandler, IRequestHandler<PagingNoteCategoryQuery, PagingResult<NoteCategoryDto>>
    {
        private readonly INoteCategoryReadOnlyRepository _noteCategoryReadOnlyRepository;

        public PagingNoteCategoryQueryHandler(INoteCategoryReadOnlyRepository noteCategoryReadOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _noteCategoryReadOnlyRepository = noteCategoryReadOnlyRepository;
        }

        public async Task<PagingResult<NoteCategoryDto>> Handle(PagingNoteCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _noteCategoryReadOnlyRepository.GetPagingAsync<NoteCategoryDto>(request.PagingRequest, cancellationToken);
            return result;
        }
    }
}
