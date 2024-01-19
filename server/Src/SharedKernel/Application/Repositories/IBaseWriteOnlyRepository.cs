using MySqlConnector;
using SharedKernel.Domain;
using SharedKernel.UnitOfWork;

namespace SharedKernel.Application
{
    public interface IBaseWriteOnlyRepository<TEntity> where TEntity : IBaseEntity
    {
        IUnitOfWork UnitOfWork { get; }

        Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken);

        Task<List<TEntity>> SaveAsync(List<TEntity> entities, CancellationToken cancellationToken);

        Task<TEntity> UpdateAsync(TEntity entity, IList<string> updateFields = default, CancellationToken cancellationToken = default);

        Task<List<TEntity>> DeleteAsync(List<string> ids, CancellationToken cancellationToken);

        ValueTask<MySqlBulkCopyResult> BulkInsertAsync(List<TEntity> entities, CancellationToken cancellationToken);
    }
}
