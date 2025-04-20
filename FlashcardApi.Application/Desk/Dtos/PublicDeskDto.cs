namespace FlashcardApi.Application.Desk.Dtos;

public class PublicDeskDto
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public string Owner { get; set; } = string.Empty;
    public int CardCount { get; set; } = 0;
}
