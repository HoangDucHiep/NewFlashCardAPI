using FlashcardApi.Application.Desk;
using FlashcardApi.Application.Desk.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class DeskService : IDeskService
{
    private readonly IDeskRepository _deskRepository;
    private readonly ICardRepository _cardRepository;

    public DeskService(IDeskRepository deskRepository, ICardRepository cardRepository)
    {
        _deskRepository = deskRepository;
        _cardRepository = cardRepository;
    }

    public async Task<List<DeskDto>> GetUserDesksAsync(string userId)
    {
        var desks = await _deskRepository.GetByOwnerIdAsync(userId);
        return desks
            .Select(d => new DeskDto
            {
                Id = d.Id,
                Name = d.Name,
                IsPublic = d.IsPublic,
                FolderId = d.FolderId,
                CreatedAt = d.CreatedAt,
                LastModified = d.LastModified,
            })
            .ToList();
    }

    public async Task<List<DeskDto>> GetPublicDesksAsync()
    {
        var desks = await _deskRepository.GetPublicDesksAsync();
        return desks
            .Select(d => new DeskDto
            {
                Id = d.Id,
                Name = d.Name,
                IsPublic = d.IsPublic,
                FolderId = d.FolderId,
                CreatedAt = d.CreatedAt,
                LastModified = d.LastModified,
            })
            .ToList();
    }

    public async Task<DeskDto> CreateDeskAsync(string userId, DeskDto deskDto)
    {
        var desk = new Desk
        {
            OwnerId = userId,
            Name = deskDto.Name,
            IsPublic = deskDto.IsPublic,
            FolderId = deskDto.FolderId,
        };
        await _deskRepository.AddAsync(desk);
        return new DeskDto
        {
            Id = desk.Id,
            Name = desk.Name,
            IsPublic = desk.IsPublic,
            FolderId = desk.FolderId,
            CreatedAt = desk.CreatedAt,
            LastModified = desk.LastModified,
        };
    }

    public async Task<DeskDto> UpdateDeskAsync(string id, DeskDto deskDto)
    {
        var desk = await _deskRepository.GetByIdAsync(id);
        if (desk == null)
            throw new Exception("Desk not found");

        desk.Name = deskDto.Name;
        desk.IsPublic = deskDto.IsPublic;
        desk.FolderId = deskDto.FolderId;
        desk.LastModified = DateTime.UtcNow;

        await _deskRepository.UpdateAsync(desk);
        return new DeskDto
        {
            Id = desk.Id,
            Name = desk.Name,
            IsPublic = desk.IsPublic,
            FolderId = desk.FolderId,
            CreatedAt = desk.CreatedAt,
            LastModified = desk.LastModified,
        };
    }

    public async Task DeleteDeskAsync(string id)
    {
        var desk = await _deskRepository.GetByIdAsync(id);
        if (desk == null)
            throw new Exception("Desk not found");

        await _deskRepository.DeleteAsync(id);
    }

    public async Task<DeskDto> CloneDeskAsync(string userId, string deskId)
    {
        var originalDesk = await _deskRepository.GetByIdAsync(deskId);
        if (originalDesk == null)
            throw new Exception("Desk not found");

        var clonedDesk = new Desk
        {
            OwnerId = userId,
            Name = originalDesk.Name + " (Cloned)",
            IsPublic = false, // Mặc định không công khai
            FolderId = originalDesk.FolderId,
        };
        await _deskRepository.AddAsync(clonedDesk);

        var cards = await _cardRepository.GetByDeskIdAsync(deskId);
        foreach (var card in cards)
        {
            var clonedCard = new Card
            {
                DeskId = clonedDesk.Id,
                Front = card.Front,
                Back = card.Back,
            };
            await _cardRepository.AddAsync(clonedCard);
        }

        return new DeskDto
        {
            Id = clonedDesk.Id,
            Name = clonedDesk.Name,
            IsPublic = clonedDesk.IsPublic,
            FolderId = clonedDesk.FolderId,
            CreatedAt = clonedDesk.CreatedAt,
            LastModified = clonedDesk.LastModified,
        };
    }
}
