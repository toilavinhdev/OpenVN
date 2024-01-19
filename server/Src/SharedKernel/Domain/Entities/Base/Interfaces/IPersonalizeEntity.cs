namespace SharedKernel.Domain
{
    public interface IPersonalizeEntity : IBaseEntity
    {
        long OwnerId { get; set; }
    }
}
