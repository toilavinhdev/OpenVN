using SharedKernel.Domain;

namespace OpenVN.BackgroundJob
{
    public interface IIntegrationAuthNoticeService
    {
        Task SignInWarningAsync(User user, RequestValue request, DateTime timestamp, CancellationToken cancellationToken = default);
    }
}
