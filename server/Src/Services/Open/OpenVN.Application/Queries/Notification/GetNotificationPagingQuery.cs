namespace OpenVN.Application
{
    public class GetNotificationPagingQuery : BaseQuery<PagingResult<NotificationDto>>
    {
        public PagingRequest Request { get; }

        public GetNotificationPagingQuery(PagingRequest request)
        {
            Request = request;
        }
    }
}
