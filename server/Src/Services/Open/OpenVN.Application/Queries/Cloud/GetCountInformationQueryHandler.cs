using AutoMapper;

namespace OpenVN.Application
{
    public class GetCountInformationQueryHandler : BaseQueryHandler, IRequestHandler<GetCountInformationQuery, CountInformation>
    {
        private readonly ICloudFileReadOnlyRepository _cloudFileReadOnlyRepository;
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;

        public GetCountInformationQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ICloudFileReadOnlyRepository cloudFileReadOnlyRepository,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository
        ) : base(authService, mapper)
        {
            _cloudFileReadOnlyRepository = cloudFileReadOnlyRepository;
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
        }

        public async Task<CountInformation> Handle(GetCountInformationQuery request, CancellationToken cancellationToken)
        {
            return new CountInformation
            {
                Directory = await _directoryReadOnlyRepository.GetCountAsync(cancellationToken),
                File = await _cloudFileReadOnlyRepository.GetCountAsync(cancellationToken),
            };
        }
    }
}
