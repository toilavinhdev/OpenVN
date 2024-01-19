namespace OpenVN.Application
{
    public class PagingWithSubSpendingQuery : BaseQuery<PagingResult<SpendingDto>>
    {
        public PagingRequest PagingRequest { get; }

        public PagingWithSubSpendingQuery(PagingRequest pagingRequest)
        {
            PagingRequest = pagingRequest;
        }
    }
}
