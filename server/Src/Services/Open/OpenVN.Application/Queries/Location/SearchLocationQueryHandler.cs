using AutoMapper;

namespace OpenVN.Application
{
    public class SearchLocationQueryHandler : BaseQueryHandler, IRequestHandler<SearchLocationQuery, List<ProvinceDto>>
    {
        private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;

        public SearchLocationQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ILocationReadOnlyRepository locationReadOnlyRepository
        ) : base(authService, mapper)
        {
            _locationReadOnlyRepository = locationReadOnlyRepository;
        }

        public async Task<List<ProvinceDto>> Handle(SearchLocationQuery request, CancellationToken cancellationToken)
        {
            return (await _locationReadOnlyRepository.SearchLocationsAsync(request.Query, cancellationToken)).ToList();
        }
    }
}
