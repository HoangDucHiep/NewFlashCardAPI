using FlashcardApi.Application.Card.Dtos;

namespace FlashcardApi.Application.Card;

public interface ICardService
{
    Task<List<CardDto>> GetCardsByDeskIdAsync(string deskId);
    Task<CardDto> CreateCardAsync(CardDto cardDto);
    Task<CardDto> UpdateCardAsync(string id, CardDto cardDto);
    Task DeleteCardAsync(string id);
    Task CleanupUnusedImagesAsync(string deskId, List<string> usedImagePaths);

}
