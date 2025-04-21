using FlashcardApi.Domain.Entities;
using System.Threading.Tasks;

namespace FlashcardApi.Domain.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task AddAsync(PasswordResetToken token);
    Task<PasswordResetToken> FindByTokenAsync(string token);
    Task UpdateAsync(PasswordResetToken token);
}