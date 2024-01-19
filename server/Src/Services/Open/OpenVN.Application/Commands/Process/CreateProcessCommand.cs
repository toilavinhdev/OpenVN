using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Process })]
    public class CreateProcessCommand : BaseInsertCommand<string>
    {
        public ProcessDto Process { get; }

        public CreateProcessCommand(ProcessDto process)
        {
            Process = process;
        }
    }
}
