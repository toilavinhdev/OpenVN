using AutoMapper;

namespace OpenVN.Application
{
    public class GetAllFeedbackQueryHandler : BaseQueryHandler, IRequestHandler<GetAllFeedbackQuery, List<FeedbackDto>>
    {
        private readonly IUserService _userService;
        private readonly IFeedbackReadOnlyRepository _feedbackReadOnlyRepository;

        public GetAllFeedbackQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IUserService userService,
            IFeedbackReadOnlyRepository feedbackReadOnlyRepository
        ) : base(authService, mapper)
        {
            _userService = userService;
            _feedbackReadOnlyRepository = feedbackReadOnlyRepository;
        }

        public async Task<List<FeedbackDto>> Handle(GetAllFeedbackQuery request, CancellationToken cancellationToken)
        {
            var feedbacks = await _feedbackReadOnlyRepository.GetAllAsync<FeedbackDto>(cancellationToken);
            var avatars = feedbacks.DistinctBy(x => x.OwnerId).Where(x => !string.IsNullOrEmpty(x.AvatarUrl)).ToList();
            var result = await Task.WhenAll(avatars.Select(x => _userService.GetAvatarUrlByFileNameAsync(x.AvatarUrl, x.TenantId, x.OwnerId, cancellationToken)));
            var dict = new Dictionary<string, string>();

            for (int i = 0; i <avatars.Count; i++)
            {
                dict.Add(avatars[i].OwnerId, result[i]);
            }
            foreach (var feedback in feedbacks)
            {
                if (!string.IsNullOrEmpty(feedback.AvatarUrl))
                {
                    feedback.AvatarUrl = dict[feedback.OwnerId];
                }
            }

            return feedbacks.ToList();
        }
    }
}
