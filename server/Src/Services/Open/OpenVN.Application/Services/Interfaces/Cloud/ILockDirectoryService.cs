using Action = System.Action;

namespace OpenVN.Application
{
    public interface ILockDirectoryService
    {
        Task MakeSureLockedDirectoryIsSafeAsync(long directoryId, string code = "", Action callback = default, CancellationToken cancellationToken = default);
    }
}
