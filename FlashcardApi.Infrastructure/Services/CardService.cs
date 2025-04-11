using FlashcardApi.Application.Card;
using FlashcardApi.Application.Card.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;

    public CardService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task<List<CardDto>> GetCardsByDeskIdAsync(string deskId)
    {
        var cards = await _cardRepository.GetByDeskIdAsync(deskId);
        return cards
            .Select(c => new CardDto
            {
                Id = c.Id,
                DeskId = c.DeskId,
                Front = c.Front,
                Back = c.Back,
                CreatedAt = c.CreatedAt,
                LastModified = c.LastModified,
            })
            .ToList();
    }

    public async Task<CardDto> CreateCardAsync(CardDto cardDto)
    {
        var card = new Card
        {
            DeskId = cardDto.DeskId,
            Front = cardDto.Front,
            Back = cardDto.Back,
        };
        await _cardRepository.AddAsync(card);
        return new CardDto
        {
            Id = card.Id,
            DeskId = card.DeskId,
            Front = card.Front,
            Back = card.Back,
            CreatedAt = card.CreatedAt,
            LastModified = card.LastModified,
        };
    }

    public async Task<CardDto> UpdateCardAsync(string id, CardDto cardDto)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        if (card == null)
            throw new Exception("Card not found");

        card.Front = cardDto.Front;
        card.Back = cardDto.Back;
        card.LastModified = DateTime.UtcNow;

        await _cardRepository.UpdateAsync(card);
        return new CardDto
        {
            Id = card.Id,
            DeskId = card.DeskId,
            Front = card.Front,
            Back = card.Back,
            CreatedAt = card.CreatedAt,
            LastModified = card.LastModified,
        };
    }

    public async Task DeleteCardAsync(string id)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        if (card == null)
            throw new Exception("Card not found");

        await _cardRepository.DeleteAsync(id);
    }
}
