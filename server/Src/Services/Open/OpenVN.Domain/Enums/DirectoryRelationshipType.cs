namespace OpenVN.Domain
{
    public enum DirectoryRelationshipType
    {
        LeftDirectoryNotFound = 1,
        RightDirectoryNotFound,
        AllNotFound,
        SameRank,
        LeftIsRoot,
        RightIsRoot,
        NoRelationship,
    }
}
