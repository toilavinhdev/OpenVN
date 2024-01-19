using AutoMapper;

namespace OpenVN.Application
{
    public class GetAuditPagingQueryHandler : BaseQueryHandler, IRequestHandler<GetAuditPagingQuery, PagingResult<AuditDto>>
    {
        private readonly IAuditReadOnlyRepository _auditReadOnlyRepository;

        public GetAuditPagingQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IAuditReadOnlyRepository auditReadOnlyRepository
        ) : base(authService, mapper)
        {
            _auditReadOnlyRepository = auditReadOnlyRepository;
        }

        public async Task<PagingResult<AuditDto>> Handle(GetAuditPagingQuery request, CancellationToken cancellationToken)
        {
            return await _auditReadOnlyRepository.GetPagingAsync(request.Request, cancellationToken);
        }
    }
}
