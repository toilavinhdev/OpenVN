namespace OpenVN.Master.Application.Repositories
{
    public interface IAppWriteOnlyRepository : IBaseWriteOnlyRepository<App>
    {
        Task UpdateFavouriteAsync(long appId, bool isFavourite, CancellationToken cancellationToken);
    }
}
