using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public interface ILockedDirectoryReadOnlyRepository : IBaseReadOnlyRepository<Directory>
    {
        Task<bool> CheckPasswordAsync(long directoryId, string password, CancellationToken cancellationToken);
    }
}
