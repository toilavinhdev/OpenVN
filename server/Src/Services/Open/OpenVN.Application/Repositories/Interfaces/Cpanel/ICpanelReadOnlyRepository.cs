using OpenVN.Application.Dto.Cpanel;
using static SharedKernel.Application.Enum;
using Action = OpenVN.Domain.Action;

namespace OpenVN.Application
{
    public interface ICpanelReadOnlyRepository : IBaseReadOnlyRepository<BaseEntity>
    {
        Task<List<User>> GetUsersByRoleId(object roleId, CancellationToken cancellationToken);

        Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken);

        Task<List<Action>> GetActionsByExponentsAsync(List<ActionExponent> exponents, CancellationToken cancellationToken);

        Task<PagingResult<UserDto>> GetUsersPagingAsync(PagingRequest request, CancellationToken cancellationToken);

        Task<List<RecordDashboardDto>> GetRecordDashboardAsync(CancellationToken cancellationToken);
    }
}
