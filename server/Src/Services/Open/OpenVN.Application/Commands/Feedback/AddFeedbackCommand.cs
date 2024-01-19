using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.AllowAnonymous })]
    public class AddFeedbackCommand : BaseInsertCommand
    {
        public FeedbackDto Dto { get; }

        public AddFeedbackCommand(FeedbackDto dto)
        {
            Dto = dto;
        }
    }
}
