namespace OpenVN.Application
{
    public interface IConfigReadOnlyRepository : IBaseReadOnlyRepository<UserConfig>
    {
        Task<UserConfig> GetConfigAsync(CancellationToken cancellationToken);
    }
}
