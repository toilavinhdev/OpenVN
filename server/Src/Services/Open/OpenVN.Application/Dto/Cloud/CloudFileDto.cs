using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class CloudFileDto
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string OriginalFileName { get; set; }

        public string FileExtension { get; set; }

        public long Size { get; set; }

        public string DirectoryId { get; set; }

        public string Url { get; set; }

        public FileType FileType => FileHelper.IsImage(FileName) ? FileType.Image : FileHelper.IsVideo(FileName) ? FileType.Video : FileType.Other;
    }
}
