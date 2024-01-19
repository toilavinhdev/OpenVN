namespace OpenVN.Application
{
    public interface IAuditReadOnlyRepository
    {
        Task<PagingResult<AuditDto>> GetPagingAsync(PagingRequest request, CancellationToken cancellationToken = default);
    }
}
