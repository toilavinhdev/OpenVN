namespace OpenVN.Application
{
    public interface IUserService
    {
        Task<string> GetAvatarUrlByFileNameAsync(string fileName, object tenantId, object ownerId, CancellationToken cancellationToken);
    }
}
