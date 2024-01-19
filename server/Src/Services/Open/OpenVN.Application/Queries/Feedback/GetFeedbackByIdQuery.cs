namespace OpenVN.Application
{
    public class GetFeedbackByIdQuery : BaseQuery<FeedbackDto>
    {
        public GetFeedbackByIdQuery(object id)
        {
            Id = id;
        }

        public object Id { get; }
    }
}
