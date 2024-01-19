using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class DeleteNoteCategoryCommandHandler : BaseCommandHandler, IRequestHandler<DeleteNoteCategoryCommand, Unit>
    {
        private readonly INoteCategoryWriteOnlyRepository _noteCategoryWriteOnlyRepository;

        public DeleteNoteCategoryCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            INoteCategoryWriteOnlyRepository noteCategoryWriteOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _noteCategoryWriteOnlyRepository = noteCategoryWriteOnlyRepository;
        }

        public async Task<Unit> Handle(DeleteNoteCategoryCommand request, CancellationToken cancellationToken)
        {
            await _noteCategoryWriteOnlyRepository.DeleteAsync(request.Ids, cancellationToken);
            await _noteCategoryWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
