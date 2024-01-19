using SharedKernel.Libraries;

namespace OpenVN.Application
{
    public class CapacityConfigurationDto
    {
        public long MaxFileSize { get; set; }

        public long MaxCapacity { get; set; }

        public long AvailableCapacity { get; set; }

        public long UsedCapacity => MaxCapacity - AvailableCapacity;

        public string MaxFileSizeText => MaxFileSize >= 1024 * 1024 * 1024 ?
                                FileHelper.ConvertBytesToGigabytes(MaxFileSize).ToString("0.00") + " GB" :
                                FileHelper.ConvertBytesToMegabytes(MaxFileSize).ToString("0.00") + " MB";

        public string MaxCapacityText => MaxCapacity >= 1024 * 1024 * 1024 ?
                                        FileHelper.ConvertBytesToGigabytes(MaxCapacity).ToString("0.00") + " GB" :
                                        FileHelper.ConvertBytesToMegabytes(MaxCapacity).ToString("0.00") + " MB";

        public string AvailableCapacityText => AvailableCapacity >= 1024 * 1024 * 1024 ?
                                        FileHelper.ConvertBytesToGigabytes(AvailableCapacity).ToString("0.00") + " GB" :
                                        FileHelper.ConvertBytesToMegabytes(AvailableCapacity).ToString("0.00") + " MB";

        public string UsedCapacityText => UsedCapacity >= 1024 * 1024 * 1024 ?
                                FileHelper.ConvertBytesToGigabytes(UsedCapacity).ToString("0.00") + " GB" :
                                FileHelper.ConvertBytesToMegabytes(UsedCapacity).ToString("0.00") + " MB";


        public string AvailablePercentage => ((1.0 * AvailableCapacity / MaxCapacity) * 100).ToString("0.00");

        public string UsedPercentage => (100 - (1.0 * AvailableCapacity / MaxCapacity) * 100).ToString("0.00");
    }
}
