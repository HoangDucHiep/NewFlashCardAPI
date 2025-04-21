using System.Text.RegularExpressions;
using FlashcardApi.Application.Card;
using FlashcardApi.Application.Card.Dtos;
using FlashcardApi.Application.Image;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IImageService _imageService;
    private readonly IReviewRepository _reviewRepository;

    public CardService(ICardRepository cardRepository, IReviewRepository reviewRepository, IImageService imageService)
    {
        _imageService = imageService;
        _cardRepository = cardRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<List<CardDto>> GetCardsByDeskIdAsync(string deskId)
    {
        var cards = await _cardRepository.GetByDeskIdAsync(deskId);
        return cards.Select(c => new CardDto
        {
            Id = c.Id,
            DeskId = c.DeskId,
            Front = c.Front,
            Back = c.Back,
            ImagePaths = ExtractImagePaths(c.Front, c.Back),
            CreatedAt = c.CreatedAt,
            LastModified = c.LastModified
        }).ToList();
    }

    public async Task<CardDto> CreateCardAsync(CardDto cardDto)
    {
        var card = new Card
        {
            DeskId = cardDto.DeskId,
            Front = cardDto.Front,
            Back = cardDto.Back
        };
        var createdCard = await _cardRepository.AddAsync(card);

        // Tạo review mặc định
        var review = new Review
        {
            CardId = createdCard.Id,
            Ease = 2.5,
            Interval = 0,
            Repetition = 0,
            NextReviewDate = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.SSS"),
            LastReviewed = null
        };
        await _reviewRepository.CreateAsync(review);

        return new CardDto
        {
            Id = createdCard.Id,
            DeskId = createdCard.DeskId,
            Front = createdCard.Front,
            Back = createdCard.Back,
            ImagePaths = ExtractImagePaths(createdCard.Front, createdCard.Back),
            CreatedAt = createdCard.CreatedAt,
            LastModified = createdCard.LastModified
        };
    }

    public async Task<CardDto> UpdateCardAsync(string id, CardDto cardDto)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        if (card == null) throw new Exception("Card not found");

        var oldImagePaths = ExtractImagePaths(card.Front, card.Back);
        card.Front = cardDto.Front;
        card.Back = cardDto.Back;
        card.DeskId = cardDto.DeskId;
        card.LastModified = DateTime.UtcNow;
        await _cardRepository.UpdateAsync(card);

        var newImagePaths = ExtractImagePaths(card.Front, card.Back);
        var unusedImages = oldImagePaths.Except(newImagePaths).ToList();
        foreach (var fileName in unusedImages)
        {
            await _imageService.DeleteImageAsync(fileName); // Sử dụng ImageService thay vì ImageRepository
        }

        return new CardDto
        {
            Id = card.Id,
            DeskId = card.DeskId,
            Front = card.Front,
            Back = card.Back,
            ImagePaths = newImagePaths,
            CreatedAt = card.CreatedAt,
            LastModified = card.LastModified
        };
    }

    public async Task<bool> DeleteCardAsync(string id)
    {
        var card = await _cardRepository.GetByIdAsync(id);
        if (card == null) return false;

        var imagePaths = ExtractImagePaths(card.Front, card.Back);
        var deleted = await _cardRepository.DeleteAsync(id);
        if (deleted)
        {
            foreach (var fileName in imagePaths)
            {
                await _imageService.DeleteImageAsync(fileName); // Gọi ImageService thay vì ImageRepository
            }
        }
        return deleted;
    }

    private List<string> ExtractImagePaths(string front, string back)
    {
        var imagePaths = new List<string>();
        string combinedHtml = (front ?? "") + (back ?? "");
        var matches = Regex.Matches(combinedHtml, @"<img[^>]+src=[""']/Uploads/(.*?)[""'][^>]*>");
        foreach (Match match in matches)
        {
            string fileName = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(fileName)) imagePaths.Add(fileName);
        }
        return imagePaths;
    }

    public async Task<List<CardDto>> GetNewCardsAsync(string deskId)
    {
        var cards = await _cardRepository.GetNewCardsAsync(deskId);
        return cards.Select(c => new CardDto
        {
            Id = c.Id,
            DeskId = c.DeskId,
            Front = c.Front,
            Back = c.Back,
            ImagePaths = ExtractImagePaths(c.Front, c.Back),
            CreatedAt = c.CreatedAt,
            LastModified = c.LastModified
        }).ToList();
    }

    public async Task<List<CardDto>> GetCardsDueTodayAsync(string deskId, string today)
    {
        var cards = await _cardRepository.GetCardsDueTodayAsync(deskId, today);
        return cards.Select(c => new CardDto
        {
            Id = c.Id,
            DeskId = c.DeskId,
            Front = c.Front,
            Back = c.Back,
            ImagePaths = ExtractImagePaths(c.Front, c.Back),
            CreatedAt = c.CreatedAt,
            LastModified = c.LastModified
        }).ToList();
    }
}