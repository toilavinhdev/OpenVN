using MediatR;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace SharedKernel.Application
{
    public class BaseDeleteCommand<TResponse> : BaseCommand<TResponse>
    {
    }

    public class BaseDeleteCommand : BaseDeleteCommand<Unit>
    {
    }
}
