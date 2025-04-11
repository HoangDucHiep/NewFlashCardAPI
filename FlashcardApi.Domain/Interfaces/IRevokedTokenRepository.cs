using FlashcardApi.Domain.Entities;

public interface IRevokedTokenRepository
{
    Task AddAsync(RevokedToken revokedToken);
    Task<bool> IsTokenRevokedAsync(string token);
}
