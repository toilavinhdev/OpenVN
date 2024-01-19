namespace OpenVN.Application
{
    public interface INoteWriteOnlyRepository : IBaseWriteOnlyRepository<Note>
    {
        Task UpdateFromIndexOrderToLastAsync(int fromOrder, int toOrder, long categoryId, bool isIncrease, CancellationToken cancellationToken);
    }
}
