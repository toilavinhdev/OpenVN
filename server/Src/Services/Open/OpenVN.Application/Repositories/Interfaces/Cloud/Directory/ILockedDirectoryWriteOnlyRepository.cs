namespace OpenVN.Application
{
    public interface ILockedDirectoryWriteOnlyRepository : IBaseWriteOnlyRepository<LockedDirectory>
    {
        Task SetPasswordAsync(long directoryId, string password, CancellationToken cancellationToken);

        Task ChangeLockStatusAsync(long directoryId, string type, CancellationToken cancellationToken);
    }
}
