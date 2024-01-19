using SharedKernel.Domain;
using SharedKernel.Libraries;
using System.ComponentModel;

namespace OpenVN.Audit.Entities
{
    public class CloudFile : PersonalizedEntity
    {
        [Description("Tên tệp"), Auditable(true, true)]
        public string FileName { get; set; }

        public string OriginalFileName { get; set; }

        public string FileExtension { get; set; }

        public long Size { get; set; }

        public long DirectoryId { get; set; }
    }
}
