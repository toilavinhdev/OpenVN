using AutoMapper;

namespace OpenVN.Application
{
    public class GetRankQueryHandler : BaseQueryHandler, IRequestHandler<GetRankQuery, PagingResult<RankDto>>
    {
        private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;

        public GetRankQueryHandler(ILocationReadOnlyRepository locationReadOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _locationReadOnlyRepository = locationReadOnlyRepository;
        }

        public async Task<PagingResult<RankDto>> Handle(GetRankQuery request, CancellationToken cancellationToken)
        {
            return await _locationReadOnlyRepository.GetRankPagingAsync(request.PagingRequest, cancellationToken);
        }
    }
}
