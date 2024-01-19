namespace OpenVN.Application
{
    public interface ISpendingReadOnlyRepository : IBaseReadOnlyRepository<Spending>
    {
        Task<PagingResult<SpendingDto>> GetPagingWithSubAsync(PagingRequest request, CancellationToken cancellationToken);
    }
}
