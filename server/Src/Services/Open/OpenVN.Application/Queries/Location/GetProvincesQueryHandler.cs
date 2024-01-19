using AutoMapper;

namespace OpenVN.Application
{
    public class GetProvincesQueryHandler : BaseQueryHandler, IRequestHandler<GetProvincesQuery, List<ProvinceDto>>
    {
        private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;

        public GetProvincesQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ILocationReadOnlyRepository locationReadOnlyRepository
        ) : base(authService, mapper)
        {
            _locationReadOnlyRepository = locationReadOnlyRepository;
        }

        public async Task<List<ProvinceDto>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
        {
            return (await _locationReadOnlyRepository.GetProvincesAsync(cancellationToken)).ToList();
        }
    }
}
