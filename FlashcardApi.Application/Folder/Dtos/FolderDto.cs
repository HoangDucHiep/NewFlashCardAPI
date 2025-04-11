namespace FlashcardApi.Application.Folder.Dtos;

public class FolderDto
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public string? ParentFolderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
}
