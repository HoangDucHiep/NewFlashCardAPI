namespace FlashcardApi.Domain.Entities;

public class Review
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CardId { get; set; }
    public double Ease { get; set; }
    public int Interval { get; set; }
    public int Repetition { get; set; }
    public string NextReviewDate { get; set; }
    public string? LastReviewed { get; set; }

    // Navigation property
    public Card Card { get; set; }
}