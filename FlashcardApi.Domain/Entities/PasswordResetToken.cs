using System;

namespace FlashcardApi.Domain.Entities;

public class PasswordResetToken
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public ApplicationUser User { get; set; }
}