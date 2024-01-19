namespace SharedKernel.Domain
{
    public interface IAuditable
    {
        DateTime CreatedDate { get; set; }

        long CreatedBy { get; set; }

        DateTime? LastModifiedDate { get; set; }

        long? LastModifiedBy { get; set; }

        DateTime? DeletedDate { get; set; }

        long? DeletedBy { get; set; }
    }
}
