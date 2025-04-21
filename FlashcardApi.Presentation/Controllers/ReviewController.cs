using FlashcardApi.Application.Interfaces;
using FlashcardApi.Application.Review.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardApi.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
    {
        var review = await _reviewService.CreateReviewAsync(reviewDto);
        return Ok(review);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(string id, [FromBody] ReviewDto reviewDto)
    {
        try
        {
            var updatedReview = await _reviewService.UpdateReviewAsync(id, reviewDto);
            return Ok(updatedReview);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        var deleted = await _reviewService.DeleteReviewAsync(id);
        return deleted ? Ok(new { message = "Review deleted successfully" }) : NotFound(new { message = "Review not found" });
    }

    [HttpGet("card/{cardId}")]
    public async Task<IActionResult> GetReviewByCardId(string cardId)
    {
        var review = await _reviewService.GetReviewByCardIdAsync(cardId);
        return review != null ? Ok(review) : NotFound();
    }

    [HttpGet("due-today")]
    public async Task<IActionResult> GetReviewsDueToday([FromQuery] string deskId, [FromQuery] string today)
    {
        var reviews = await _reviewService.GetReviewsDueTodayAsync(deskId, today);
        return Ok(reviews);
    }
}