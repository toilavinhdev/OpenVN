using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace SharedKernel.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.AllowAnonymous })]
    public class BaseAllowAnonymousQuery<TResponse> : BaseQuery<TResponse>
    {
    }
}
