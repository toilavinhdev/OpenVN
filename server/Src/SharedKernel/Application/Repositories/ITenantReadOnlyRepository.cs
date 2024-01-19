namespace SharedKernel.Application
{
    public interface ITenantReadOnlyRepository
    {
        Task<IEnumerable<TResult>> GetAllAsync<TResult>(CancellationToken cancellationToken);
    }
}
