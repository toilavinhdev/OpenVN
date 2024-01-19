using AutoMapper;

namespace OpenVN.Application
{
    public class GetUserPagingQueryHandler : BaseQueryHandler, IRequestHandler<GetUserPagingQuery, PagingResult<UserDto>>
    {
        private readonly ICpanelReadOnlyRepository _cpanelReadOnlyRepository;

        public GetUserPagingQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ICpanelReadOnlyRepository cpanelReadOnlyRepository
        ) : base(authService, mapper)
        {
            _cpanelReadOnlyRepository = cpanelReadOnlyRepository;
        }

        public Task<PagingResult<UserDto>> Handle(GetUserPagingQuery request, CancellationToken cancellationToken)
        {
            return _cpanelReadOnlyRepository.GetUsersPagingAsync(request.Request, cancellationToken);
        }
    }
}
