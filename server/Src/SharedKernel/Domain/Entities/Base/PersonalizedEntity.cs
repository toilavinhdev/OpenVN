namespace SharedKernel.Domain
{
    public abstract class PersonalizedEntity : BaseEntity, IPersonalizeEntity
    {
        public long OwnerId { get; set; }
    }
}
