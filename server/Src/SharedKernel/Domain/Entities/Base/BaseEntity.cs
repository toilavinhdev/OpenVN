using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SharedKernel.Domain
{
    //[ModelBinder(BinderType = typeof(BaseEntityBinder))]
    public abstract class BaseEntity<TKey> : CoreEntity, IBaseEntity<TKey>
    {
        [System.ComponentModel.DataAnnotations.Key]
        public TKey Id { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public long TenantId { get; set; }

        public DateTime CreatedDate { get; set; } = SharedKernel.Libraries.DateHelper.Now;

        public long CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public long? LastModifiedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public long? DeletedBy { get; set; }

        #region Domain events
        private List<DomainEvent> _domainEvents;

        [NotMapped, Libraries.Ignore]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(DomainEvent @event)
        {
            _domainEvents = _domainEvents ?? new List<DomainEvent>();
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(DomainEvent @event)
        {
            _domainEvents?.Remove(@event);
        }

        public void ClearDomainEvents()
        {
            _domainEvents = null;
        }
        #endregion

        #region Cloneable
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }

    /// <summary>
    /// By default, TKey is long
    /// </summary>
    public class BaseEntity : BaseEntity<long>, IBaseEntity
    {
    }
}
