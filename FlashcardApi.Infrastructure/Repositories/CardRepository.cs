using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories;

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _context;

    public CardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Card>> GetByDeskIdAsync(string deskId)
    {
        return await _context.Cards.Where(c => c.DeskId == deskId).ToListAsync();
    }

    public async Task<Card?> GetByIdAsync(string id)
    {
        return await _context.Cards.FindAsync(id);
    }

    public async Task<Card> AddAsync(Card card)
    {
        await _context.Cards.AddAsync(card);
        await _context.SaveChangesAsync();
        return card;
    }

    public async Task<Card> UpdateAsync(Card card)
    {
        _context.Cards.Update(card);
        await _context.SaveChangesAsync();
        return card;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var card = await GetByIdAsync(id);
        if (card == null) return false;

        _context.Cards.Remove(card); // Review sẽ tự động xóa do cascade
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Card>> GetNewCardsAsync(string deskId)
    {
        return await _context.Cards
            .Join(_context.Reviews,
                c => c.Id,
                r => r.CardId,
                (c, r) => new { Card = c, Review = r })
            .Where(cr => cr.Card.DeskId == deskId && cr.Review.Interval == 0)
            .Select(cr => cr.Card)
            .ToListAsync();
    }

    public async Task<List<Card>> GetCardsDueTodayAsync(string deskId, string today)
    {
        return await _context.Cards
            .Join(_context.Reviews,
                c => c.Id,
                r => r.CardId,
                (c, r) => new { Card = c, Review = r })
            .Where(cr => cr.Card.DeskId == deskId && cr.Review.NextReviewDate.CompareTo(today) <= 0 && cr.Review.Interval > 0)
            .Select(cr => cr.Card)
            .ToListAsync();
    }
}