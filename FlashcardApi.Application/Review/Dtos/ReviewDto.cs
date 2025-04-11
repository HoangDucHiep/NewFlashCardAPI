namespace FlashcardApi.Application.Review.Dtos;

public class ReviewDto
{
    public string? Id { get; set; } // Nullable cho POST
    public string CardId { get; set; }
    public double Ease { get; set; }
    public int Interval { get; set; }
    public int Repetition { get; set; }
    public string NextReviewDate { get; set; }
    public string? LastReviewed { get; set; }
}
