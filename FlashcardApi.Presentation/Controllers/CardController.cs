using FlashcardApi.Application.Card;
using FlashcardApi.Application.Card.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardApi.Presentation.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCardsByDeskId([FromQuery] string deskId)
    {
        var cards = await _cardService.GetCardsByDeskIdAsync(deskId);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCard([FromBody] CardDto cardDto)
    {
        var card = await _cardService.CreateCardAsync(cardDto);
        return Ok(card);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCard(string id, [FromBody] CardDto cardDto)
    {
        try
        {
            var updatedCard = await _cardService.UpdateCardAsync(id, cardDto);
            return Ok(updatedCard);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCard(string id)
    {
        var deleted = await _cardService.DeleteCardAsync(id);
        return deleted ? Ok(new { message = "Card deleted successfully" }) : NotFound(new { message = "Card not found" });
    }

    [HttpGet("new")]
    public async Task<IActionResult> GetNewCards([FromQuery] string deskId)
    {
        var cards = await _cardService.GetNewCardsAsync(deskId);
        return Ok(cards);
    }

    [HttpGet("due-today")]
    public async Task<IActionResult> GetCardsDueToday([FromQuery] string deskId, [FromQuery] string today)
    {
        var cards = await _cardService.GetCardsDueTodayAsync(deskId, today);
        return Ok(cards);
    }
}