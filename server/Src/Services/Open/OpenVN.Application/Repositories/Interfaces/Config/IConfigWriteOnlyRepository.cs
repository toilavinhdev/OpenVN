using SharedKernel.Domain;

namespace OpenVN.Application
{
    public interface IConfigWriteOnlyRepository : IBaseWriteOnlyRepository<UserConfig>
    {
        Task<UserConfig> CreateOrUpdateAsync(UserConfig userConfig, CancellationToken cancellationToken);
    }
}
