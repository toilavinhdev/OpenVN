using AutoMapper;

namespace OpenVN.Application
{
    public class PagingWithSubSpendingQueryHandler : BaseQueryHandler, IRequestHandler<PagingWithSubSpendingQuery, PagingResult<SpendingDto>>
    {
        private readonly ISpendingReadOnlyRepository _readOnlyRepository;

        public PagingWithSubSpendingQueryHandler(ISpendingReadOnlyRepository readOnlyRepository, IAuthService authService, IMapper mapper) : base(authService, mapper)
        {
            _readOnlyRepository = readOnlyRepository;
        }

        public async Task<PagingResult<SpendingDto>> Handle(PagingWithSubSpendingQuery request, CancellationToken cancellationToken)
        {
            var result = await _readOnlyRepository.GetPagingWithSubAsync(request.PagingRequest, cancellationToken);

            return new PagingResult<SpendingDto>
            {
                Count = result.Count,
                Data = _mapper.Map<List<SpendingDto>>(result.Data)
            };
        }
    }
}
