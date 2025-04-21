using FlashcardApi.Application.Desk.Dtos;
using FlashcardApi.Application.Folder;
using FlashcardApi.Application.Folder.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;
    private readonly IDeskRepository _deskRepository;

    public FolderService(IFolderRepository folderRepository, IDeskRepository deskRepository)
    {
        _folderRepository = folderRepository;
        _deskRepository = deskRepository;
    }

    public async Task<List<FolderDto>> GetUserFoldersAsync(string userId)
    {
        var folders = await _folderRepository.GetByOwnerIdAsync(userId);
        return folders.Select(f => new FolderDto
        {
            Id = f.Id,
            Name = f.Name,
            ParentFolderId = f.ParentFolderId,
            CreatedAt = f.CreatedAt,
            LastModified = f.LastModified
        }).ToList();
    }

    public async Task<FolderDto> CreateFolderAsync(string userId, FolderDto folderDto)
    {
        var folder = new Folder
        {
            OwnerId = userId,
            Name = folderDto.Name,
            ParentFolderId = folderDto.ParentFolderId
        };
        var createdFolder = await _folderRepository.AddAsync(folder);
        return new FolderDto
        {
            Id = createdFolder.Id,
            Name = createdFolder.Name,
            ParentFolderId = createdFolder.ParentFolderId,
            CreatedAt = createdFolder.CreatedAt,
            LastModified = createdFolder.LastModified
        };
    }

    public async Task<FolderDto> UpdateFolderAsync(string id, FolderDto folderDto)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null) throw new Exception("Folder not found");

        folder.Name = folderDto.Name;
        folder.ParentFolderId = folderDto.ParentFolderId;
        folder.LastModified = DateTime.UtcNow;
        await _folderRepository.UpdateAsync(folder);

        return new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
            CreatedAt = folder.CreatedAt,
            LastModified = folder.LastModified
        };
    }

    public async Task<bool> DeleteFolderAsync(string id)
    {
        return await _folderRepository.DeleteAsync(id);
    }

    public async Task<List<NestedFolderDto>> GetNestedFoldersAsync(string userId)
    {
        // Lấy tất cả folders và desks của user
        var folders = await _folderRepository.GetByOwnerIdAsync(userId);
        var desks = await _deskRepository.GetByOwnerIdAsync(userId);

        // Tạo map để tra cứu folder theo Id
        var folderMap = folders.ToDictionary(f => f.Id, f => new NestedFolderDto
        {
            Id = f.Id,
            Name = f.Name,
            ParentFolderId = f.ParentFolderId,
            CreatedAt = f.CreatedAt,
            LastModified = f.LastModified
        });

        // Gán desks vào folders tương ứng
        foreach (var desk in desks)
        {
            if (folderMap.TryGetValue(desk.FolderId, out var folderDto))
            {
                folderDto.Desks.Add(new DeskDto
                {
                    Id = desk.Id,
                    Name = desk.Name,
                    IsPublic = desk.IsPublic,
                    FolderId = desk.FolderId,
                    CreatedAt = desk.CreatedAt,
                    LastModified = desk.LastModified
                });
            }
        }

        // Xây dựng cấu trúc lồng nhau
        foreach (var folder in folderMap.Values)
        {
            if (folder.ParentFolderId != null && folderMap.TryGetValue(folder.ParentFolderId, out var parentFolder))
            {
                parentFolder.SubFolders.Add(folder);
            }
        }

        // Lọc ra các root folders (không có ParentFolderId)
        return folderMap.Values
            .Where(f => f.ParentFolderId == null)
            .ToList();
    }
}