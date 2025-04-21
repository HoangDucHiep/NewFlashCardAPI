namespace FlashcardApi.Domain.Entities;

public class Session
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string DeskId { get; set; }
    public string StartTime { get; set; }
    public string? EndTime { get; set; }
    public int CardsStudied { get; set; }
    public double Performance { get; set; }

    // Navigation property
    public Desk Desk { get; set; }
}