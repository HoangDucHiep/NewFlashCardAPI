using FlashcardApi.Application.Review.Dtos;

namespace FlashcardApi.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateReviewAsync(ReviewDto reviewDto);
        Task<ReviewDto> UpdateReviewAsync(string id, ReviewDto reviewDto);
        Task DeleteReviewAsync(string id);
        Task<ReviewDto> GetReviewByCardIdAsync(string cardId);
        Task<List<ReviewDto>> GetReviewsDueTodayAsync(string deskId, string today);
    }
}
