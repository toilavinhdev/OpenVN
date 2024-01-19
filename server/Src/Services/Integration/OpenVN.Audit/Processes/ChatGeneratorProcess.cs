using OpenVN.Audit.Entities;

namespace OpenVN.Audit.Processes
{
    public class ChatGeneratorProcess : BaseProcess<ChatGenerator>
    {
        public ChatGeneratorProcess() : base(new Models.AuditConfigModel("CG"))
        {
        }
    }
}
