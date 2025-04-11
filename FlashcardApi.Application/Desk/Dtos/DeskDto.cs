namespace FlashcardApi.Application.Desk.Dtos;

public class DeskDto
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public bool IsPublic { get; set; }
    public string? FolderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
}
