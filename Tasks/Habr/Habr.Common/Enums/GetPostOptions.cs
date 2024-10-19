namespace Habr.Common.Enums;

[Flags]
public enum GetPostOptions
{
    None = 0,
    AsTracking = 1,
    PublishedOnly = 2,
    NotPublishedOnly = 4,
    IncludeCreator = 8,
    IncludeComments = 16,
    OrderByUpdatedDate = 32,
    OrderByPublicationDate = 64,
}
