using AutoMapper;

namespace OpenVN.Application
{
    public class GetWardsQueryHandler : BaseQueryHandler, IRequestHandler<GetWardsQuery, List<WardDto>>
    {
        private readonly ILocationReadOnlyRepository _locationReadOnlyRepository;

        public GetWardsQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ILocationReadOnlyRepository locationReadOnlyRepository
        ) : base(authService, mapper)
        {
            _locationReadOnlyRepository = locationReadOnlyRepository;
        }

        public async Task<List<WardDto>> Handle(GetWardsQuery request, CancellationToken cancellationToken)
        {
            return (await _locationReadOnlyRepository.GetWardsAsync(request.DistrictId, cancellationToken)).ToList();
        }
    }
}
