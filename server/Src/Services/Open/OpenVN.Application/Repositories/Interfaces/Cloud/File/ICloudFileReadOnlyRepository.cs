namespace OpenVN.Application
{
    public interface ICloudFileReadOnlyRepository : IBaseReadOnlyRepository<CloudFile>
    {
        Task<IEnumerable<CloudFile>> GetListFileByIdsAsync(List<long> ids, CancellationToken cancellationToken);

        Task<long> GetTotalFileSizeAsync(CancellationToken cancellationToken);

        Task<long> GetTotalFileSizeInSpecialDirectoryAsync(long directoryId, CancellationToken cancellationToken);

        Task<List<CloudFileDto>> GetFilesInDirectoryAsync(long directoryId, CancellationToken cancellationToken);

        Task<List<CloudFile>> GetFilesInDirectoriesAsync(List<long> directoryIds, CancellationToken cancellationToken);

        Task<FilePropertyDto> GetPropertiesAsync(long id, CancellationToken cancellationToken);
    }
}
