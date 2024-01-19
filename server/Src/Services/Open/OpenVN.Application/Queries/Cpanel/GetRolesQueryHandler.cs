using AutoMapper;
using OpenVN.Application.Dto.Cpanel;

namespace OpenVN.Application
{
    public class GetRolesQueryHandler : BaseQueryHandler, IRequestHandler<GetRolesQuery, List<RoleDto>>
    {
        private readonly ICpanelReadOnlyRepository _cpanelReadOnlyRepository;

        public GetRolesQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ICpanelReadOnlyRepository cpanelReadOnlyRepository
        ) : base(authService, mapper)
        {
            _cpanelReadOnlyRepository = cpanelReadOnlyRepository;
        }

        public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await _cpanelReadOnlyRepository.GetRolesAsync(cancellationToken);
        }
    }
}
