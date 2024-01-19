using SharedKernel.Auth;

namespace SharedKernel.Domain
{
    public class UpdateAuditEvent<T> : AuditEvent where T : IBaseEntity
    {
        public List<UpdateAuditModel<T>> UpdateModels { get; }

        public UpdateAuditEvent(List<UpdateAuditModel<T>> updateModels, IToken token) : base(typeof(T).Name, AuditAction.Update, token)
        {
            UpdateModels = updateModels;
        }
    }
}
