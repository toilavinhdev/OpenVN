using AutoMapper;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class PagingSpendingQueryHandler : BaseQueryHandler, IRequestHandler<PagingSpendingQuery, PagingResult<SpendingDto>>
    {
        private readonly ISpendingReadOnlyRepository _readOnlyRepository;

        public PagingSpendingQueryHandler(ISpendingReadOnlyRepository readOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _readOnlyRepository = readOnlyRepository;
        }

        public async Task<PagingResult<SpendingDto>> Handle(PagingSpendingQuery request, CancellationToken cancellationToken)
        {
            var result = await _readOnlyRepository.GetPagingAsync<SpendingDto>(request.PagingRequest, cancellationToken);
            return result;
        }
    }
}
