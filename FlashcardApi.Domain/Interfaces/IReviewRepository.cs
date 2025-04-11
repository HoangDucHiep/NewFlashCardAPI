using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface IReviewRepository
{
    Task<Review> CreateAsync(Review review);
    Task<Review> UpdateAsync(Review review);
    Task DeleteAsync(string id);
    Task<Review> GetByCardIdAsync(string cardId);
    Task<List<Review>> GetReviewsDueTodayAsync(string deskId, string today);
}
