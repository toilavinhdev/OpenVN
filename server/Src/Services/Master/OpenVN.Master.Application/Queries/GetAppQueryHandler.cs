using AutoMapper;

namespace OpenVN.Master.Application
{
    public class GetAppQueryHandler : IRequestHandler<GetAppQuery, List<AppDto>>
    {
        private readonly IAppReadOnlyRepository _appReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetAppQueryHandler(IAppReadOnlyRepository appReadOnlyRepository, IMapper mapper)
        {
            _appReadOnlyRepository = appReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<AppDto>> Handle(GetAppQuery request, CancellationToken cancellationToken)
        {
            return (await _appReadOnlyRepository.GetAppsAsync(cancellationToken)).ToList();
        }
    }
}
