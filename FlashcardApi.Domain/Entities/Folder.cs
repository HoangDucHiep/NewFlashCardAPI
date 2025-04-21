namespace FlashcardApi.Domain.Entities;

public class Folder
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public string? ParentFolderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ApplicationUser Owner { get; set; }
    public Folder? ParentFolder { get; set; }
    public ICollection<Folder> SubFolders { get; set; } = new List<Folder>();
    public ICollection<Desk> Desks { get; set; } = new List<Desk>();
}