using AutoMapper;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class CreateNoteCommandHandler : BaseCommandHandler, IRequestHandler<CreateNoteCommand, string>
    {
        private readonly IMapper _mapper;
        private readonly INoteWriteOnlyRepository _noteWriteOnlyRepository;
        private readonly INoteCategoryReadOnlyRepository _noteCategoryReadOnlyRepository;

        public CreateNoteCommandHandler(IEventDispatcher eventDispatcher, IAuthService authService, IMapper mapper, INoteWriteOnlyRepository noteWriteOnlyRepository, INoteCategoryReadOnlyRepository noteCategoryReadOnlyRepository) : base(eventDispatcher, authService)
        {
            _mapper = mapper;
            _noteWriteOnlyRepository = noteWriteOnlyRepository;
            _noteCategoryReadOnlyRepository = noteCategoryReadOnlyRepository;
        }

        public async Task<string> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            var order = await _noteCategoryReadOnlyRepository.GetNextOrderAsync(long.Parse(request.NoteDto.CategoryId), cancellationToken);
            var entity = _mapper.Map<Note>(request.NoteDto);
            entity.Order = order;

            await _noteWriteOnlyRepository.SaveAsync(entity, cancellationToken);
            await _noteWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return entity.Id.ToString();
        }
    }
}
