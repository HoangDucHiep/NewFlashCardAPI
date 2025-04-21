namespace FlashcardApi.Domain.Entities;

public class Desk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public string FolderId { get; set; } // Bắt buộc thuộc folder
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ApplicationUser Owner { get; set; }
    public Folder Folder { get; set; }
    public ICollection<Card> Cards { get; set; } = new List<Card>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}