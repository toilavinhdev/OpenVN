namespace OpenVN.Application
{
    public class GetAuditPagingQuery : BaseQuery<PagingResult<AuditDto>>
    {
        public PagingRequest Request { get; }

        public GetAuditPagingQuery(PagingRequest request)
        {
            Request = request;
        }
    }
}
