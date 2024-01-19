using AutoMapper;

namespace OpenVN.Application
{
    public class GetCapacityConfigurationQueryHandler : BaseQueryHandler, IRequestHandler<GetCapacityConfigurationQuery, CapacityConfigurationDto>
    {
        private readonly ICloudConfigService _cloudConfigService;

        public GetCapacityConfigurationQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ICloudConfigService cloudConfigService
        ) : base(authService, mapper)
        {
            _cloudConfigService = cloudConfigService;
        }

        public async Task<CapacityConfigurationDto> Handle(GetCapacityConfigurationQuery request, CancellationToken cancellationToken)
        {
            return await _cloudConfigService.GetCapacityConfigurationAsync(cancellationToken);
        }
    }
}
