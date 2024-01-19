using AutoMapper;

namespace OpenVN.Application
{
    public class UpdateNoteCategoryCommandHandler : BaseCommandHandler, IRequestHandler<UpdateNoteCategoryCommand, Unit>
    {
        private readonly INoteCategoryWriteOnlyRepository _noteCategoryWriteOnlyRepository;
        private readonly IMapper _mapper;

        public UpdateNoteCategoryCommandHandler(IEventDispatcher eventDispatcher, IAuthService authService, INoteCategoryWriteOnlyRepository noteCategoryWriteOnlyRepository, IMapper mapper) : base(eventDispatcher, authService)
        {
            _noteCategoryWriteOnlyRepository = noteCategoryWriteOnlyRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateNoteCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<NoteCategory>(request.NoteCategoryDto);
            await _noteCategoryWriteOnlyRepository.UpdateAsync(entity, new List<string> { nameof(entity.Name) }, cancellationToken);
            await _noteCategoryWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
