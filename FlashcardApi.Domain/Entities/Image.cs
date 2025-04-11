namespace FlashcardApi.Domain.Entities;

public class Image
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Url { get; set; }
    public string UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
