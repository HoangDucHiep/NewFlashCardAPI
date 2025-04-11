namespace FlashcardApi.Domain.Entities
{
    public class RevokedToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; }
        public DateTime RevokedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } // Thời gian hết hạn của token
    }
}