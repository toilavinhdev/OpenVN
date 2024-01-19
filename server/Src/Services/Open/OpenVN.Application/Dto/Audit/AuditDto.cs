using SharedKernel.Libraries;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class AuditDto
    {
        public long Id { get; set; }

        public string TableName { get; set; }

        public string DisplayModule
        {
            get
            {
                switch (TableName)
                {
                    case nameof(Directory): return "Thư mục";
                    case nameof(CloudFile): return "Tệp";
                    case nameof(Note): return "Ghi chú";
                    case nameof(Avatar): return "Hồ sơ";
                    case nameof(ChatGenerator): return "Chat Generator";
                    case "AppMaster": return "Ứng dụng";
                    case "SignIn":
                    case "SignOut": return "Bảo mật";
                    default: return "";
                }
            }
        }

        public int Action { get; set; }

        public string DisplayAction
        {
            get
            {
                return ((AuditAction)Action).GetDescription();
            }
        }

        public string Description { get; set; }

        public DateTime Timestamp { get; set; }

        public string IpAddress { get; set; }
    }
}
