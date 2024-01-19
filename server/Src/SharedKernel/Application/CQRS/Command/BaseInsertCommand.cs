using MediatR;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace SharedKernel.Application
{
    public class BaseInsertCommand<TResponse> : BaseCommand<TResponse>
    {
    }

    public class BaseInsertCommand : BaseInsertCommand<Unit>
    {
    }
}
