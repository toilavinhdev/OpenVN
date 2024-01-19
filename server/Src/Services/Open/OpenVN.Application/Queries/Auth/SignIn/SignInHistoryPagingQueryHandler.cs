using AutoMapper;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class SignInHistoryPagingQueryHandler : BaseQueryHandler, IRequestHandler<SignInHistoryPagingQuery, PagingResult<SignInHistoryDto>>
    {
        private readonly IAuthRepository _authRepository;

        public SignInHistoryPagingQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IAuthRepository authRepository
        ) : base(authService, mapper)
        {
            _authRepository = authRepository;
        }

        public async Task<PagingResult<SignInHistoryDto>> Handle(SignInHistoryPagingQuery request, CancellationToken cancellationToken)
        {
            return await _authRepository.GetSignInHistoryPaging(request.PagingRequest, cancellationToken);
        }
    }
}
