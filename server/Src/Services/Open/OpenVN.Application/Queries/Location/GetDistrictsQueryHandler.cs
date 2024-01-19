using AutoMapper;

namespace OpenVN.Application
{
    public class GetDistrictsQueryHandler : BaseQueryHandler, IRequestHandler<GetDistrictsQuery, List<DistrictDto>>
    {
        private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;

        public GetDistrictsQueryHandler(
            IAuthService authService, 
            IMapper mapper,
            ILocationReadOnlyRepository locationReadOnlyRepository
        ) : base(authService, mapper)
        {
            _locationReadOnlyRepository = locationReadOnlyRepository;
        }

        public async Task<List<DistrictDto>> Handle(GetDistrictsQuery request, CancellationToken cancellationToken)
        {
            return (await _locationReadOnlyRepository.GetDistrictsAsync(request.ProvinceId, cancellationToken)).ToList();
        }
    }
}