using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface ICardRepository
{
    Task<List<Card>> GetByDeskIdAsync(string deskId);
    Task<Card> GetByIdAsync(string id);
    Task<Card> AddAsync(Card card);
    Task<Card> UpdateAsync(Card card);
    Task<bool> DeleteAsync(string id);
    Task<List<Card>> GetNewCardsAsync(string deskId);
    Task<List<Card>> GetCardsDueTodayAsync(string deskId, string today);
}
