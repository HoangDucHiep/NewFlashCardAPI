using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface ICardRepository
{
    Task<List<Card>> GetByDeskIdAsync(string deskId);
    Task<Card> GetByIdAsync(string id);
    Task AddAsync(Card card);
    Task UpdateAsync(Card card);
    Task DeleteAsync(string id);
}
