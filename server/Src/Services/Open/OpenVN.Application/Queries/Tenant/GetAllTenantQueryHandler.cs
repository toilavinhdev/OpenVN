using AutoMapper;

namespace OpenVN.Application
{
    public class GetAllTenantQueryHandler : BaseQueryHandler, IRequestHandler<GetAllTenantQuery, List<TenantDto>>
    {
        private readonly ITenantReadOnlyRepository _tenantReadOnlyRepository;

        public GetAllTenantQueryHandler(
            IAuthService authService, 
            IMapper mapper,
            ITenantReadOnlyRepository tenantReadOnlyRepository
        ) : base(authService, mapper)
        {
            _tenantReadOnlyRepository = tenantReadOnlyRepository;
        }

        public async Task<List<TenantDto>> Handle(GetAllTenantQuery request, CancellationToken cancellationToken)
        {
            return (await _tenantReadOnlyRepository.GetAllAsync<TenantDto>(cancellationToken)).ToList();
        }
    }
}
