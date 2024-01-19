using MediatR;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace SharedKernel.Application
{
    public class BaseUpdateCommand<TResponse> : BaseCommand<TResponse>
    {
    }

    public class BaseUpdateCommand : BaseUpdateCommand<Unit>
    {
    }
}
