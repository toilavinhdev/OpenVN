namespace OpenVN.Application
{
    public interface ICpanelWriteOnlyRepository : IBaseWriteOnlyRepository<BaseEntity>
    {
        //Task CreateAccountAsync(CreateAccountDataDto account, CancellationToken cancellationToken);

        Task UpdateRoleAsync(object roleId, object actionId, bool value, CancellationToken cancellationToken);
    }
}
