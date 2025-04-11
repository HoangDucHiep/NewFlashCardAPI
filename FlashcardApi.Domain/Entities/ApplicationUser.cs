using Microsoft.AspNetCore.Identity;
namespace FlashcardApi.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CurrentToken { get; set; } = null;
}