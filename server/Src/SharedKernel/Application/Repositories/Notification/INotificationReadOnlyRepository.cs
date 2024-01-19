using SharedKernel.Domain;

namespace SharedKernel.Application
{
    public interface INotificationReadOnlyRepository : IBaseReadOnlyRepository<Notification>
    {
        Task<int> GetNumberOfUnreadNotificationAsync(CancellationToken cancellationToken);
    }
}
