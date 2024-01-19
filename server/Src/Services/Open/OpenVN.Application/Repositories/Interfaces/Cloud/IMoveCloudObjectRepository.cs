using SharedKernel.UnitOfWork;

namespace OpenVN.Application
{
    public interface IMoveCloudObjectRepository
    {
        IUnitOfWork UnitOfWork { get; }

        Task MoveObjectAsync(object destinationId, List<MoveObject> moveObjects, CancellationToken cancellationToken);
    }
}
