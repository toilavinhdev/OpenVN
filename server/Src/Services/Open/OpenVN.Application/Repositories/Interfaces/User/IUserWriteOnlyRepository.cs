using SharedKernel.Domain;

namespace OpenVN.Application
{
    public interface IUserWriteOnlyRepository : IBaseWriteOnlyRepository<User>
    {
        Task<long> CreateUserAsync(CreateUserDto data, CancellationToken cancellationToken = default);

        Task SetAvatarAsync(string fileName, CancellationToken cancellationToken);

        Task RemoveAvatarAsync(CancellationToken cancellationToken);
    }
}
