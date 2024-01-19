namespace OpenVN.Application
{
    public class CloudConfigService : ICloudConfigService
    {
        private readonly ICloudConfigReadOnlyRepository _cloudConfigReadOnlyRepository;
        private readonly ICloudFileReadOnlyRepository _cloudFileReadOnlyRepository;

        public CloudConfigService(ICloudConfigReadOnlyRepository cloudConfigReadOnlyRepository, ICloudFileReadOnlyRepository cloudFileReadOnlyRepository)
        {
            _cloudConfigReadOnlyRepository = cloudConfigReadOnlyRepository;
            _cloudFileReadOnlyRepository = cloudFileReadOnlyRepository;
        }
        public async Task<CapacityConfigurationDto> GetCapacityConfigurationAsync(CancellationToken cancellationToken)
        {
            var config = await _cloudConfigReadOnlyRepository.GetCloudConfigAsync(cancellationToken);
            var capacityUsed = await _cloudFileReadOnlyRepository.GetTotalFileSizeAsync(cancellationToken);

            return new CapacityConfigurationDto
            {
                MaxCapacity = config.MaxCapacity,
                AvailableCapacity = config.MaxCapacity - capacityUsed,
                MaxFileSize = config.MaxFileSize,
            };
        }
    }
}
