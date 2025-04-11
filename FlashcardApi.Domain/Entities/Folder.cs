namespace FlashcardApi.Domain.Entities;

public class Folder
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public string? ParentFolderId { get; set; } // null for root
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
