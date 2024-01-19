using AutoMapper;

namespace OpenVN.Application
{
    public class GetConfigQueryHandler : BaseQueryHandler, IRequestHandler<GetConfigQuery, UserConfigDto>
    {
        private readonly IConfigReadOnlyRepository _readRepository;
        private readonly IConfigWriteOnlyRepository _writeRepository;

        public GetConfigQueryHandler(
            IAuthService authService,
            IMapper mapper, 
            IConfigReadOnlyRepository readRepository, 
            IConfigWriteOnlyRepository writeRepository
        ) : base(authService, mapper)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
        }

        public async Task<UserConfigDto> Handle(GetConfigQuery request, CancellationToken cancellationToken)
        {
            var result = await _readRepository.GetConfigAsync(cancellationToken) ?? await _writeRepository.CreateOrUpdateAsync(null, cancellationToken);
            return _mapper.Map<UserConfigDto>(result);
        }
    }
}
