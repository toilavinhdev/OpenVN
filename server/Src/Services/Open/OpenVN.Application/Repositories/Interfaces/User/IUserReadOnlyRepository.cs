namespace OpenVN.Application
{
    public interface IUserReadOnlyRepository : IBaseReadOnlyRepository<User>
    {
        Task<string> CheckDuplicateAsync(string username, string email, string phone, long tenantId, CancellationToken cancellationToken = default);

        Task<Avatar> GetAvatarAsync(CancellationToken cancellationToken);

        Task<User> GetUserInformationAsync(CancellationToken cancellationToken);
    }
}
