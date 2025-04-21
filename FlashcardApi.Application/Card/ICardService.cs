using FlashcardApi.Application.Card.Dtos;

namespace FlashcardApi.Application.Card;

public interface ICardService
{
    Task<List<CardDto>> GetCardsByDeskIdAsync(string deskId);
    Task<CardDto> CreateCardAsync(CardDto cardDto);
    Task<CardDto> UpdateCardAsync(string id, CardDto cardDto);
    Task<bool> DeleteCardAsync(string id);
    Task<List<CardDto>> GetNewCardsAsync(string deskId);
    Task<List<CardDto>> GetCardsDueTodayAsync(string deskId, string today);
}
