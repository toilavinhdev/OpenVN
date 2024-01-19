namespace OpenVN.Application
{
    public class SignInHistoryPagingQuery : BaseQuery<PagingResult<SignInHistoryDto>>
    {
        public PagingRequest PagingRequest { get; }

        public SignInHistoryPagingQuery(PagingRequest pagingRequest)
        {
            PagingRequest = pagingRequest;
        }
    }
}
