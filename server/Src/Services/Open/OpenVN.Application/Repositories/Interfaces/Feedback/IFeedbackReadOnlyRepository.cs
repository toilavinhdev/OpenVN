namespace OpenVN.Application
{
    public interface IFeedbackReadOnlyRepository : IBaseReadOnlyRepository<Feedback>
    {
        Task<IEnumerable<TResult>> GetRepliesAsync<TResult>(object sourceFeedbackId, CancellationToken cancellationToken);

        Task<int> GetRepliesCountAsync(object sourceFeedbackId, CancellationToken cancellationToken);
    }
}
