using AutoMapper;
using Newtonsoft.Json;
using SharedKernel.Auth;

namespace OpenVN.Application
{
    public class UpdateConfigCommandHandler : BaseCommandHandler, IRequestHandler<UpdateConfigCommand, UserConfigDto>
    {
        private readonly IConfigWriteOnlyRepository _configWriteOnlyRepository;
        private readonly IToken _token;
        private readonly IMapper _mapper;

        public UpdateConfigCommandHandler(IEventDispatcher eventDispatcher, IAuthService authService, IConfigWriteOnlyRepository configWriteOnlyRepository, IToken token, IMapper mapper) : base(eventDispatcher, authService)
        {
            _configWriteOnlyRepository = configWriteOnlyRepository;
            _token = token;
            _mapper = mapper;
        }

        public async Task<UserConfigDto> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = new UserConfig()
            {
                Json = JsonConvert.SerializeObject(request.ConfigValue),
                OwnerId = _token.Context.OwnerId,
                TenantId= _token.Context.TenantId,
            };
            var config = await _configWriteOnlyRepository.CreateOrUpdateAsync(entity, cancellationToken);
            await _configWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return _mapper.Map<UserConfigDto>(config);
        }
    }
}
