namespace OpenVN.Application
{
    public class PagingFeedbackQuery : BaseAllowAnonymousQuery<PagingResult<FeedbackDto>>
    {
        public PagingFeedbackQuery(PagingRequest request)
        {
            Request = request;
        }

        public PagingRequest Request { get; }
    }
}
