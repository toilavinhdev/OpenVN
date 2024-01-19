namespace OpenVN.Application
{
    public interface INoteReadOnlyRepository : IBaseReadOnlyRepository<Note>
    {
        Task<List<NoteWithoutContentDto>> SearchAsync(string query, CancellationToken cancellationToken);
    }
}
