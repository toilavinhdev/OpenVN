using AutoMapper;

namespace OpenVN.Application
{
    public class PagingNoteQueryHandler : BaseQueryHandler, IRequestHandler<PagingNoteQuery, PagingResult<NoteWithoutContentDto>>
    {
        private readonly INoteReadOnlyRepository _noteReadOnlyRepository;

        public PagingNoteQueryHandler(INoteReadOnlyRepository noteReadOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _noteReadOnlyRepository = noteReadOnlyRepository;
        }

        public async Task<PagingResult<NoteWithoutContentDto>> Handle(PagingNoteQuery request, CancellationToken cancellationToken)
        {
            return await _noteReadOnlyRepository.GetPagingAsync<NoteWithoutContentDto>(request.PagingRequest, cancellationToken);
        }
    }
}
