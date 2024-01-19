using AutoMapper;

namespace OpenVN.Application
{
    public class GetNotificationPagingQueryHandler : BaseQueryHandler, IRequestHandler<GetNotificationPagingQuery, PagingResult<NotificationDto>>
    {
        private readonly INotificationReadOnlyRepository _notificationReadOnlyRepository;

        public GetNotificationPagingQueryHandler(
            IAuthService authService,
            IMapper mapper,
            INotificationReadOnlyRepository notificationReadOnlyRepository
        ) : base(authService, mapper)
        {
            _notificationReadOnlyRepository = notificationReadOnlyRepository;
        }

        public async Task<PagingResult<NotificationDto>> Handle(GetNotificationPagingQuery request, CancellationToken cancellationToken)
        {
            return await _notificationReadOnlyRepository.GetPagingAsync<NotificationDto>(request.Request, cancellationToken);
        }
    }
}
