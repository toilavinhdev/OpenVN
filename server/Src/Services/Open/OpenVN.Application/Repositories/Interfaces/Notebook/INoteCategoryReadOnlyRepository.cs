namespace OpenVN.Application
{
    public interface INoteCategoryReadOnlyRepository : IBaseReadOnlyRepository<NoteCategory>
    {
        Task<int> GetNextOrderAsync(long categoryId, CancellationToken cancellationToken);
    }
}
