using MediatR;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace SharedKernel.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.AllowAnonymous })]
    public class BaseAllowAnonymousCommand<TResponse> : BaseCommand<TResponse>
    {
    }

    public class BaseAllowAnonymousCommand : BaseAllowAnonymousCommand<Unit>
    {
    }
}
