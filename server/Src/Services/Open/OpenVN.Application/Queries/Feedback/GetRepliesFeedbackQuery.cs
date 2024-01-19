namespace OpenVN.Application
{
    public class GetRepliesFeedbackQuery : BaseAllowAnonymousQuery<List<FeedbackDto>>
    {
        public string Id { get; }

        public GetRepliesFeedbackQuery(string id)
        {
            Id = id;
        }
    }
}
