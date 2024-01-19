using AutoMapper;

namespace OpenVN.Application
{
    public class GetGenerateByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetGenerateByIdQuery, ChatGeneratorDto>
    {
        private readonly IChatGeneratorReadOnlyRepository _chatGeneratorReadOnlyRepository;

        public GetGenerateByIdQueryHandler(
            IChatGeneratorReadOnlyRepository chatGeneratorReadOnlyRepository,
            IAuthService authService,
            IMapper mapper
        ) : base(authService, mapper)
        {
            _chatGeneratorReadOnlyRepository = chatGeneratorReadOnlyRepository;
        }

        public async Task<ChatGeneratorDto> Handle(GetGenerateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _chatGeneratorReadOnlyRepository.GetByIdAsync<ChatGeneratorDto>(request.Id, cancellationToken);
        }
    }
}
