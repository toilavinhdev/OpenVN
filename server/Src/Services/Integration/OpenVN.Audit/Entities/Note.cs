using SharedKernel.Domain;
using SharedKernel.Libraries;
using System.ComponentModel;

namespace OpenVN.Audit.Entities
{
    public class Note : PersonalizedEntity
    {
        [Description("Tiêu đề"), Auditable(true, true)]
        public string Title { get; set; }

        [Description("Nội dung"), Auditable]
        public string Content { get; set; }

        [Description("Thứ tự"), Auditable]
        public int Order { get; set; }

        [Description("Loại ghi chú"), Auditable]
        public long CategoryId { get; set; }

        public string CategoryName { get; set; }

        [Description("Công khai"), Auditable]
        public bool IsPublic { get; set; }

        [Description("Ghim"), Auditable]
        public bool IsPinned { get; set; }

        [Description("Màu nền"), Auditable]
        public string BackgroundColor { get; set; }
    }
}
