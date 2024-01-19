using AutoMapper;

namespace OpenVN.Application
{
    public class GetFeedbackByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetFeedbackByIdQuery, FeedbackDto>
    {
        private readonly IFeedbackReadOnlyRepository _feedbackReadOnlyRepository;
        private readonly IUserService _userService;

        public GetFeedbackByIdQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IFeedbackReadOnlyRepository feedbackReadOnlyRepository,
            IUserService userService
        ) : base(authService, mapper)
        {
            _feedbackReadOnlyRepository = feedbackReadOnlyRepository;
            _userService = userService;
        }

        public async Task<FeedbackDto> Handle(GetFeedbackByIdQuery request, CancellationToken cancellationToken)
        {
            var feedback = await _feedbackReadOnlyRepository.GetByIdAsync<FeedbackDto>(request.Id, cancellationToken);
            if (feedback != null)
            {
                feedback.AvatarUrl = await _userService.GetAvatarUrlByFileNameAsync(feedback.AvatarUrl, feedback.TenantId, feedback.OwnerId, cancellationToken);
            }
            return feedback;
        }
    }
}
