namespace OpenVN.Application
{
    public class PagingSpendingQuery : BaseQuery<PagingResult<SpendingDto>>
    {
        public PagingRequest PagingRequest { get; }

        public PagingSpendingQuery(PagingRequest pagingRequest)
        {
            PagingRequest = pagingRequest;
        }
    }
}
