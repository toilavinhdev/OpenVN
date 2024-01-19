namespace OpenVN.Domain
{
    public class NoteShared : BaseEntity
    {
        public long NoteId { get; set; }

        public long PersonId { get; set; }
    }
}
