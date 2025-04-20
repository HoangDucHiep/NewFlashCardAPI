using System.Text.RegularExpressions;
using FlashcardApi.Application.Card;
using FlashcardApi.Application.Card.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IImageRepository _imageRepository;

    public CardService(ICardRepository cardRepository, IImageRepository imageRepository)
    {
        _cardRepository = cardRepository;
        _imageRepository = imageRepository;
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
                ImagePaths = ExtractImagePaths(c.Front, c.Back), // Trích xuất từ HTML
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
            ImagePaths = cardDto.ImagePaths, // Giữ nguyên danh sách từ client
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
            ImagePaths = cardDto.ImagePaths, // Giữ nguyên danh sách từ client
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

    public async Task CleanupUnusedImagesAsync(string deskId, List<string> usedImagePaths)
    {
        var allCards = await _cardRepository.GetByDeskIdAsync(deskId);
        var allUsedImagePaths = new HashSet<string>();

        // Thu thập tất cả ảnh đang dùng trong Desk
        foreach (var card in allCards)
        {
            var imagePaths = ExtractImagePaths(card.Front, card.Back);
            allUsedImagePaths.UnionWith(imagePaths);
        }

        // Xác định ảnh không còn dùng
        var allImages = await _imageRepository.GetAllAsync(); 
        foreach (var image in allImages)
        {
            string fileName = Path.GetFileName(image.Url);
            if (!allUsedImagePaths.Contains(fileName))
            {
                await _imageRepository.DeleteAsync(image.Id);
            }
        }
    }

    private List<string> ExtractImagePaths(string front, string back)
    {
        var imagePaths = new List<string>();
        string combinedHtml = (front ?? "") + (back ?? "");
        var matches = Regex.Matches(combinedHtml, @"<img[^>]+src=[""'](.*?)[""'][^>]*>");
        foreach (Match match in matches)
        {
            string src = match.Groups[1].Value;
            string fileName = Path.GetFileName(src);
            if (!string.IsNullOrEmpty(fileName))
                imagePaths.Add(fileName);
        }
        return imagePaths;
    }
}
