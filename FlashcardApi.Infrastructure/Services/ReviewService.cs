using FlashcardApi.Application.Interfaces;
using FlashcardApi.Application.Review.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewDto> CreateReviewAsync(ReviewDto reviewDto)
    {
        var review = new Review
        {
            CardId = reviewDto.CardId,
            Ease = reviewDto.Ease,
            Interval = reviewDto.Interval,
            Repetition = reviewDto.Repetition,
            NextReviewDate = reviewDto.NextReviewDate,
            LastReviewed = reviewDto.LastReviewed,
        };
        var createdReview = await _reviewRepository.CreateAsync(review);
        return MapToDto(createdReview);
    }

    public async Task<ReviewDto> UpdateReviewAsync(string id, ReviewDto reviewDto)
    {
        var review = await _reviewRepository.GetByCardIdAsync(reviewDto.CardId);
        if (review == null || review.Id != id)
            throw new Exception("Review not found");

        review.Ease = reviewDto.Ease;
        review.Interval = reviewDto.Interval;
        review.Repetition = reviewDto.Repetition;
        review.NextReviewDate = reviewDto.NextReviewDate;
        review.LastReviewed = reviewDto.LastReviewed;

        await _reviewRepository.UpdateAsync(review);
        return MapToDto(review);
    }

    public async Task DeleteReviewAsync(string id)
    {
        await _reviewRepository.DeleteAsync(id);
    }

    public async Task<ReviewDto> GetReviewByCardIdAsync(string cardId)
    {
        var review = await _reviewRepository.GetByCardIdAsync(cardId);
        return review != null ? MapToDto(review) : null;
    }

    public async Task<List<ReviewDto>> GetReviewsDueTodayAsync(string deskId, string today)
    {
        var reviews = await _reviewRepository.GetReviewsDueTodayAsync(deskId, today);
        return reviews.Select(MapToDto).ToList();
    }

    private ReviewDto MapToDto(Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            CardId = review.CardId,
            Ease = review.Ease,
            Interval = review.Interval,
            Repetition = review.Repetition,
            NextReviewDate = review.NextReviewDate,
            LastReviewed = review.LastReviewed,
        };
    }
}
