namespace FlashcardApi.Application.Session.Dtos;

public class SessionDto
{
    public string? Id { get; set; } // Nullable cho POST
    public string DeskId { get; set; }
    public string StartTime { get; set; }
    public string? EndTime { get; set; }
    public int CardsStudied { get; set; }
    public double Performance { get; set; }
}
