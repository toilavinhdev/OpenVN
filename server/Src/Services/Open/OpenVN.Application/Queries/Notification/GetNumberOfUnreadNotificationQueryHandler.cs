using AutoMapper;

namespace OpenVN.Application
{
    public class GetNumberOfUnreadNotificationQueryHandler : BaseQueryHandler, IRequestHandler<GetNumberOfUnreadNotificationQuery, int>
    {
        private readonly INotificationReadOnlyRepository _notificationReadOnlyRepository;

        public GetNumberOfUnreadNotificationQueryHandler(
            IAuthService authService,
            IMapper mapper,
            INotificationReadOnlyRepository notificationReadOnlyRepository
        ) : base(authService, mapper)
        {
            _notificationReadOnlyRepository = notificationReadOnlyRepository;
        }

        public async Task<int> Handle(GetNumberOfUnreadNotificationQuery request, CancellationToken cancellationToken)
        {
            return await _notificationReadOnlyRepository.GetNumberOfUnreadNotificationAsync(cancellationToken);
        }
    }
}
