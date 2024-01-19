using SharedKernel.Domain;
using SharedKernel.Libraries;
using System.ComponentModel;

namespace OpenVN.Audit.Entities
{
    public class ChatGenerator : PersonalizedEntity
    {
        [Description("tệp"), Auditable(true)]
        public string FileName { get; set; }

        public string Contents { get; set; }
    }
}
