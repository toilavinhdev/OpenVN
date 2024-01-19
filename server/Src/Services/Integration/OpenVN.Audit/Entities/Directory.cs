using SharedKernel.Domain;
using SharedKernel.Libraries;
using System.ComponentModel;

namespace OpenVN.Audit.Entities
{
    public class Directory : PersonalizedEntity
    {
        [Description("Tên thư mục"), Auditable(true, true)]
        public string Name { get; set; }

        public long ParentId { get; set; }

        public string Path { get; set; }

        public int DuplicateNo { get; set; }
    }
}
