using FlashcardApi.Application.Folder.Dtos;

namespace FlashcardApi.Application.Folder;

public interface IFolderService
{
    Task<List<FolderDto>> GetUserFoldersAsync(string userId);
    Task<FolderDto> CreateFolderAsync(string userId, FolderDto folderDto);
    Task<FolderDto> UpdateFolderAsync(string id, FolderDto folderDto);
    Task<bool> DeleteFolderAsync(string id);
    Task<List<NestedFolderDto>> GetNestedFoldersAsync(string userId);
}
