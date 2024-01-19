using AutoMapper;

namespace OpenVN.Application
{
    public class TestDeadlockQueryHandler : BaseQueryHandler, IRequestHandler<TestDeadlockQuery, PagingResult<TestDeadlock>>
    {
        private readonly IBaseReadOnlyRepository<TestDeadlock> _repository;

        public TestDeadlockQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IBaseReadOnlyRepository<TestDeadlock> repository
        ) : base(authService, mapper)
        {
            _repository = repository;
        }

        public async Task<PagingResult<TestDeadlock>> Handle(TestDeadlockQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetPagingAsync<TestDeadlock>(new PagingRequest(1, 5), cancellationToken);
        }
    }
}
