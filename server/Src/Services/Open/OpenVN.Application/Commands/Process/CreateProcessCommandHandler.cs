using AutoMapper;

namespace OpenVN.Application
{
    public class CreateProcessCommandHandler : BaseCommandHandler, IRequestHandler<CreateProcessCommand, string>
    {
        private readonly IMapper _mapper;
        private readonly IProcessWriteOnlyRepository _processRepository;

        public CreateProcessCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            IProcessWriteOnlyRepository processRepository
        ) : base(eventDispatcher, authService)
        {
            _mapper = mapper;
            _processRepository = processRepository;
        }

        public async Task<string> Handle(CreateProcessCommand request, CancellationToken cancellationToken)
        {
            //var process = _mapper.Map<Process>(request.Process);
            //process.AddDomainEvent(new CreateProcessEvent(Guid.NewGuid(), _mapper.Map<Process>(request.Process)));

            //await _processRepository.SaveAsync(process, cancellationToken);
            //await _processRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            //return process.Id.ToString();
            return "";
        }
    }
}
