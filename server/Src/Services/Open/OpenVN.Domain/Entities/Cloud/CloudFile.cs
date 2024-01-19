namespace OpenVN.Domain
{
    [Table("cloud_file")]
    public class CloudFile : PersonalizedEntity
    {
        public string FileName { get; set; }

        public string OriginalFileName { get; set; }

        public string FileExtension { get; set; }

        public long Size { get; set; }

        public long DirectoryId { get; set; }

        public CloudFile()
        {
        }

        public CloudFile(string fileName, string originalFileName, string fileExtension, long size, long directoryId)
        {
            FileName = fileName;
            OriginalFileName = originalFileName;
            FileExtension = fileExtension;
            Size = size;
            DirectoryId = directoryId;
        }

        public CloudFile(string fileName, string originalFileName, string fileExtension, long size, long directoryId, DomainEvent @event)
        {
            FileName = fileName;
            OriginalFileName = originalFileName;
            FileExtension = fileExtension;
            Size = size;
            DirectoryId = directoryId;
            AddDomainEvent(@event);
        }
    }
}
