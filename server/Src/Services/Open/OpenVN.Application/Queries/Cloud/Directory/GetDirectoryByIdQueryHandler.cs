using AutoMapper;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class GetDirectoryByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetDirectoryByIdQuery, DirectoryDto>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;

        public GetDirectoryByIdQueryHandler(
            IDirectoryReadOnlyRepository directoryReadOnlyRepository, 
            IAuthService authService, 
            IMapper mapper
        ) : base(authService, mapper)
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
        }

        public async Task<DirectoryDto> Handle(GetDirectoryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _directoryReadOnlyRepository.GetByIdAsync<DirectoryDto>(request.Id, cancellationToken);
        }
    }
}
