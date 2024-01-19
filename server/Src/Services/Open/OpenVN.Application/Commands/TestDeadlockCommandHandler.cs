
using SharedKernel.Libraries;

namespace OpenVN.Application
{
    public class TestDeadlockCommandHandler : BaseCommandHandler, IRequestHandler<TestDeadlockCommand, Unit>
    {
        private readonly IBaseWriteOnlyRepository<TestDeadlock> _repository;

        public TestDeadlockCommandHandler(
            IEventDispatcher eventDispatcher, IAuthService authService,
            IBaseWriteOnlyRepository<TestDeadlock> repository
        ) : base(eventDispatcher, authService)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(TestDeadlockCommand request, CancellationToken cancellationToken)
        {
            var entities = new List<TestDeadlock>();
            for (int i = 1; i <= 1000000; i++)
            {
                entities.Add(new TestDeadlock
                {
                    Value = Utility.RandomString(64),
                    IsDeleted = int.Parse(Utility.RandomNumber(1)) % 2 == 0
                });
            }
            await _repository.SaveAsync(entities, cancellationToken);
            await _repository.UnitOfWork.CommitAsync(false, cancellationToken);

            return Unit.Value;
        }
    }
}
