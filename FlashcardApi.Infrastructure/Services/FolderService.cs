using FlashcardApi.Application.Folder;
using FlashcardApi.Application.Folder.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;

    public FolderService(IFolderRepository folderRepository)
    {
        _folderRepository = folderRepository;
    }

    public async Task<List<FolderDto>> GetUserFoldersAsync(string userId)
    {
        var folders = await _folderRepository.GetByOwnerIdAsync(userId);
        return folders
            .Select(f => new FolderDto
            {
                Id = f.Id,
                Name = f.Name,
                ParentFolderId = f.ParentFolderId,
                CreatedAt = f.CreatedAt,
                LastModified = f.LastModified,
            })
            .ToList();
    }

    public async Task<FolderDto> CreateFolderAsync(string userId, FolderDto folderDto)
    {
        var folder = new Folder
        {
            OwnerId = userId,
            Name = folderDto.Name,
            ParentFolderId = folderDto.ParentFolderId,
        };
        await _folderRepository.AddAsync(folder);
        return new FolderDto
        {
            Id = folder.Id,
            Name = folder.Name,
            ParentFolderId = folder.ParentFolderId,
            CreatedAt = folder.CreatedAt,
            LastModified = folder.LastModified,
        };
    }

    public async Task<FolderDto> UpdateFolderAsync(string id, FolderDto folderDto)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null)
            throw new Exception("Folder not found");

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
            LastModified = folder.LastModified,
        };
    }

    public async Task DeleteFolderAsync(string id)
    {
        var folder = await _folderRepository.GetByIdAsync(id);
        if (folder == null)
            throw new Exception("Folder not found");

        await _folderRepository.DeleteAsync(id);
    }
}
