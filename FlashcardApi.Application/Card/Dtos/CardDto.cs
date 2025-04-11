namespace FlashcardApi.Application.Card.Dtos;

public class CardDto
{
    public string? Id { get; set; }
    public string DeskId { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
}
