using System.Threading.Tasks;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories
{
    public class RevokedTokenRepository : IRevokedTokenRepository
    {
        private readonly AppDbContext _context;

        public RevokedTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RevokedToken revokedToken)
        {
            await _context.RevokedTokens.AddAsync(revokedToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            return await _context.RevokedTokens.AnyAsync(rt =>
                rt.Token == token && rt.ExpiresAt > DateTime.UtcNow
            );
        }
    }
}
