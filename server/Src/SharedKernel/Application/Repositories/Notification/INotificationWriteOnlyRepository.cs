using SharedKernel.Domain;

namespace SharedKernel.Application
{
    public interface INotificationWriteOnlyRepository : IBaseWriteOnlyRepository<Notification>
    {
        Task MarkAsReadOrUnreadAsync(object id, bool markAsRead, CancellationToken cancellationToken);

        Task MarkAllAsReadAsync(CancellationToken cancellationToken);
    }
}
