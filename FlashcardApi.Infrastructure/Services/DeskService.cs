using FlashcardApi.Application.Desk;
using FlashcardApi.Application.Desk.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class DeskService : IDeskService
{
    private readonly IDeskRepository _deskRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IFolderRepository _folderRepository;

    public DeskService(
        IDeskRepository deskRepository,
        ICardRepository cardRepository,
        IUserRepository userRepository,
        IReviewRepository reviewRepository,
        IFolderRepository folderRepository)
    {
        _deskRepository = deskRepository;
        _cardRepository = cardRepository;
        _userRepository = userRepository;
        _reviewRepository = reviewRepository;
        _folderRepository = folderRepository;
    }

    public async Task<List<DeskDto>> GetUserDesksAsync(string userId)
    {
        var desks = await _deskRepository.GetByOwnerIdAsync(userId);
        return desks.Select(d => new DeskDto
        {
            Id = d.Id,
            Name = d.Name,
            IsPublic = d.IsPublic,
            FolderId = d.FolderId,
            CreatedAt = d.CreatedAt,
            LastModified = d.LastModified
        }).ToList();
    }

    public async Task<DeskDto?> GetDeskByIdAsync(string id)
    {
        var desk = await _deskRepository.GetByIdAsync(id);
        return desk == null ? null : new DeskDto
        {
            Id = desk.Id,
            Name = desk.Name,
            IsPublic = desk.IsPublic,
            FolderId = desk.FolderId,
            CreatedAt = desk.CreatedAt,
            LastModified = desk.LastModified
        };
    }

    public async Task<List<PublicDeskDto>> GetPublicDesksAsync()
    {
        var desks = await _deskRepository.GetPublicDesksAsync();
        return desks.Select(d => new PublicDeskDto
        {
            Id = d.Id,
            Name = d.Name,
            Owner = _userRepository.FindByIdAsync(d.OwnerId).Result?.UserName ?? "Unknown",
            CardCount = _cardRepository.GetByDeskIdAsync(d.Id).Result.Count
        }).ToList();
    }

    public async Task<DeskDto> CreateDeskAsync(string userId, DeskDto deskDto)
    {
        var desk = new Desk
        {
            OwnerId = userId,
            Name = deskDto.Name,
            IsPublic = deskDto.IsPublic,
            FolderId = deskDto.FolderId
        };
        var createdDesk = await _deskRepository.AddAsync(desk);
        return new DeskDto
        {
            Id = createdDesk.Id,
            Name = createdDesk.Name,
            IsPublic = createdDesk.IsPublic,
            FolderId = createdDesk.FolderId,
            CreatedAt = createdDesk.CreatedAt,
            LastModified = createdDesk.LastModified
        };
    }

    public async Task<DeskDto> UpdateDeskAsync(string id, DeskDto deskDto)
    {
        var desk = await _deskRepository.GetByIdAsync(id);
        if (desk == null) throw new Exception("Desk not found");

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
            LastModified = desk.LastModified
        };
    }

    public async Task<bool> DeleteDeskAsync(string id)
    {
        return await _deskRepository.DeleteAsync(id);
    }

    public async Task<DeskDto> CloneDeskAsync(string userId, string deskId, string targetFolderId)
    {
        // Kiểm tra desk gốc tồn tại và là public
        var originalDesk = await _deskRepository.GetByIdAsync(deskId);
        if (originalDesk == null || !originalDesk.IsPublic)
            throw new Exception("Desk not found or not public");

        // Kiểm tra folder đích có thuộc sở hữu của người dùng không
        var targetFolder = await _folderRepository.GetByIdAsync(targetFolderId);
        if (targetFolder == null || targetFolder.OwnerId != userId)
            throw new Exception("Target folder not found or not owned by user");

        // Tạo desk mới
        var clonedDesk = new Desk
        {
            OwnerId = userId,
            Name = originalDesk.Name + " (Cloned)",
            IsPublic = false, // Desk mới không public mặc định
            FolderId = targetFolderId
        };
        var createdDesk = await _deskRepository.AddAsync(clonedDesk);

        // Copy các card
        var cards = await _cardRepository.GetByDeskIdAsync(deskId);
        foreach (var card in cards)
        {
            var clonedCard = new Card
            {
                DeskId = createdDesk.Id,
                Front = card.Front,
                Back = card.Back
            };
            var newCard = await _cardRepository.AddAsync(clonedCard);

            // Tạo review mới
            var newReview = new Review
            {
                CardId = newCard.Id,
                Ease = 2.5,
                Interval = 0,
                Repetition = 0,
                NextReviewDate = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.SSS"),
                LastReviewed = null
            };
            await _reviewRepository.CreateAsync(newReview);
        }

        return new DeskDto
        {
            Id = createdDesk.Id,
            Name = createdDesk.Name,
            IsPublic = createdDesk.IsPublic,
            FolderId = createdDesk.FolderId,
            CreatedAt = createdDesk.CreatedAt,
            LastModified = createdDesk.LastModified
        };
    }
}