namespace OpenVN.Application
{
    public interface ISpendingWriteOnlyRepository : IBaseWriteOnlyRepository<Spending>
    {
        Task<long> AddSpendingAsync(List<Spending> spendings, CancellationToken cancellationToken);
    }
}
