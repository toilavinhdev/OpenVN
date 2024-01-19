using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public interface IDirectoryReadOnlyRepository : IBaseReadOnlyRepository<Directory>
    {
        Task<PagingResult<Directory>> GetPagingResultAsync(long directoryId, PagingRequest request, CancellationToken cancellationToken);

        Task<IEnumerable<Directory>> GetListDirectoryByIdsAsync(List<long> ids, CancellationToken cancellationToken);

        Task<int> GetMaxDirectoryDuplicateNoAsync(Directory directory, CancellationToken cancellationToken);

        Task<Directory> GetParentDirectoryAsync(long parentDirectoryId, CancellationToken cancellationToken);

        Task<List<DirectoryDto>> GetChildrenDirectoryAsync(long parentDirectoryId, CancellationToken cancellationToken);

        Task<PathDto> GetPathAsync(long directoryId, CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetChildrenNodeIdAsync(long rootId, CancellationToken cancellationToken);

        Task<Directory> FindFirstNodeLockedDirectoryAsync(long sourceId, CancellationToken cancellationToken);

        Task<DirectoryRelationshipType> GetRelationshipBetwenTwoDirectories(long leftId, long rightId, CancellationToken cancellationToken);

        Task<bool> IsLockedDirectoryAsync(long directoryId, CancellationToken cancellationToken);

        Task<bool> HasPasswordAsync(long directoryId, CancellationToken cancellationToken);

        Task<DirectoryPropertyDto> GetPropertiesAsync(long id, CancellationToken cancellationToken);
    }
}
