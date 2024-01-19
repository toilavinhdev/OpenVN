namespace OpenVN.Application
{
    public interface ICloudConfigService
    {
        Task<CapacityConfigurationDto> GetCapacityConfigurationAsync(CancellationToken cancellationToken);
    }
}
