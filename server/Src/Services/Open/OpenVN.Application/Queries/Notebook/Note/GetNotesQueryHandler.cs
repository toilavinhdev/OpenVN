using AutoMapper;

namespace OpenVN.Application
{
    public class GetNotesQueryHandler : BaseQueryHandler, IRequestHandler<GetNotesQuery, List<NoteWithoutContentDto>>
    {
        private readonly INoteReadOnlyRepository _noteReadOnlyRepository;

        public GetNotesQueryHandler(INoteReadOnlyRepository noteReadOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _noteReadOnlyRepository = noteReadOnlyRepository;
        }

        public async Task<List<NoteWithoutContentDto>> Handle(GetNotesQuery request, CancellationToken cancellationToken)
        {
            return await _noteReadOnlyRepository.SearchAsync(request.Query, cancellationToken);
        }
    }
}
