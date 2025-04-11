namespace FlashcardApi.Domain.Entities;

public class Card
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DeskId { get; set; }
    public string Front { get; set; }
    public string Back { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
