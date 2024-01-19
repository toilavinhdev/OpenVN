namespace OpenVN.Application
{
    public class GetRankQuery : BaseAllowAnonymousQuery<PagingResult<RankDto>>
    {
        public PagingRequest PagingRequest { get; }

        public GetRankQuery(PagingRequest pagingRequest)
        {
            PagingRequest = pagingRequest;
        }
    }
}
