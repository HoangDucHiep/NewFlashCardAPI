using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Review> CreateAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<Review> UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task DeleteAsync(string id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Review> GetByCardIdAsync(string cardId)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.CardId == cardId);
    }

    public async Task<List<Review>> GetReviewsDueTodayAsync(string deskId, string today)
    {
        return await _context
            .Reviews.Join(
                _context.Cards,
                r => r.CardId,
                c => c.Id,
                (r, c) => new { Review = r, Card = c }
            )
            .Where(rc => rc.Card.DeskId == deskId && rc.Review.NextReviewDate.CompareTo(today) <= 0)
            .Select(rc => rc.Review)
            .ToListAsync();
    }
}
