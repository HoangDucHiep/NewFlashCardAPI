namespace FlashcardApi.Domain.Entities;

public class Review
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Đổi sang string
    public string CardId { get; set; } // Đổi sang string để khớp với Card
    public double Ease { get; set; }
    public int Interval { get; set; }
    public int Repetition { get; set; }
    public string NextReviewDate { get; set; }
    public string? LastReviewed { get; set; }
}
