namespace OpenVN.Master.Application
{
    public interface IAppReadOnlyRepository : IBaseReadOnlyRepository<App>
    {
        Task<IEnumerable<AppDto>> GetAppsAsync(CancellationToken cancellationToken);
    }
}
