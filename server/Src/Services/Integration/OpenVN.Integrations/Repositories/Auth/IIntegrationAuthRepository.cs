namespace OpenVN.BackgroundJob
{
    public interface IIntegrationAuthRepository
    {
        Task WriteSignInAsync(SignInHistory history, CancellationToken cancellationToken);
    }
}
