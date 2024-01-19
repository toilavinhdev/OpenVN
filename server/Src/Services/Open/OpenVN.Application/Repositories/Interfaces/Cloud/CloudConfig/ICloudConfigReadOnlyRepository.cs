namespace OpenVN.Application
{
    public interface ICloudConfigReadOnlyRepository : IBaseReadOnlyRepository<CloudConfig>
    {
        Task<CloudConfig> GetCloudConfigAsync(CancellationToken cancellationToken);
    }
}
