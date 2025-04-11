namespace FlashcardApi.Domain.Entities;

public class Desk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public string? FolderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
