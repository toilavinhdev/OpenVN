using Microsoft.Extensions.Localization;
using SharedKernel.Properties;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Application
{
    public class MarkAsReadOrUnreadCommandHandler : BaseCommandHandler, IRequestHandler<MarkAsReadOrUnreadCommand, Unit>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly INotificationWriteOnlyRepository _notificationWriteOnlyRepository;

        public MarkAsReadOrUnreadCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IStringLocalizer<Resources> localizer,
            INotificationWriteOnlyRepository notificationWriteOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _localizer = localizer;
            _notificationWriteOnlyRepository = notificationWriteOnlyRepository;
        }

        public async Task<Unit> Handle(MarkAsReadOrUnreadCommand request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.Id, out var id))
            {
                throw new BadRequestException(_localizer["bad_data"]);
            }
            await _notificationWriteOnlyRepository.MarkAsReadOrUnreadAsync(request.Id, request.MarkAsRead, cancellationToken);
            await _notificationWriteOnlyRepository.UnitOfWork.CommitAsync(true, cancellationToken);

            return Unit.Value;
        }
    }
}
