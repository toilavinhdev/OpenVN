using AutoMapper;

namespace OpenVN.Application
{
    public class GetNoteByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetNoteByIdQuery, NoteDto>
    {
        private readonly INoteReadOnlyRepository _noteReadOnlyRepository;

        public GetNoteByIdQueryHandler(INoteReadOnlyRepository noteReadOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _noteReadOnlyRepository = noteReadOnlyRepository;
        }

        public async Task<NoteDto> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _noteReadOnlyRepository.GetByIdAsync<NoteDto>(request.Id, cancellationToken);
            return result;
        }
    }
}
