using SharedKernel.Domain;

namespace SharedKernel.Application
{
    public interface IBaseReadOnlyRepository<TEntity> where TEntity : BaseEntity
    {

        #region Cache

        #region Get
        Task<CacheResult<List<TResult>>> GetAllCacheAsync<TResult>(CancellationToken cancellationToken);

        Task<CacheResult<TResult>> GetByIdCacheAsync<TResult>(object id, CancellationToken cancellationToken);
        #endregion

        #region Set

        #endregion

        #endregion

        Task<IEnumerable<TResult>> GetAllAsync<TResult>(CancellationToken cancellationToken);

        Task<TResult> GetByIdAsync<TResult>(object id, CancellationToken cancellationToken);

        Task<PagingResult<TResult>> GetPagingAsync<TResult>(PagingRequest request, CancellationToken cancellationToken);

        Task<long> GetCountAsync(CancellationToken cancellationToken);
    }
}