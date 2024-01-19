using AutoMapper;

namespace OpenVN.Application
{
    public class GetGeneratePagingQueryHandler : BaseQueryHandler, IRequestHandler<GetGeneratePagingQuery, PagingResult<ChatGeneratorDto>>
    {
        private readonly IChatGeneratorReadOnlyRepository _chatGeneratorReadOnlyRepository;

        public GetGeneratePagingQueryHandler(
            IChatGeneratorReadOnlyRepository chatGeneratorReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService, mapper)
        {
            _chatGeneratorReadOnlyRepository = chatGeneratorReadOnlyRepository;
        }

        public async Task<PagingResult<ChatGeneratorDto>> Handle(GetGeneratePagingQuery request, CancellationToken cancellationToken)
        {
            return await _chatGeneratorReadOnlyRepository.GetPagingAsync<ChatGeneratorDto>(request.Request, cancellationToken);
        }
    }
}
