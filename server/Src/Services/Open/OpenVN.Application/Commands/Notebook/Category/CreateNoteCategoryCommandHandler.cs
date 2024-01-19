using AutoMapper;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class CreateNoteCategoryCommandHandler : BaseCommandHandler, IRequestHandler<CreateNoteCategoryCommand, string>
    {
        private readonly IMapper _mapper;
        private readonly INoteCategoryWriteOnlyRepository _noteCategoryWriteOnlyRepository;

        public CreateNoteCategoryCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            INoteCategoryWriteOnlyRepository noteCategoryWriteOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _mapper = mapper;
            _noteCategoryWriteOnlyRepository = noteCategoryWriteOnlyRepository;
        }

        public async Task<string> Handle(CreateNoteCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<NoteCategory>(request.NoteCategoryDto);
            await _noteCategoryWriteOnlyRepository.SaveAsync(entity, cancellationToken);
            await _noteCategoryWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return entity.Id.ToString();
        }
    }
}
