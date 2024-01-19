using static SharedKernel.Application.Enum;

namespace SharedKernel.Application
{
    public class ChangResult
    {
        public ChangeType ChangeType { get; set; }

        public List<Change> Changes { get; set; }
    }
}
