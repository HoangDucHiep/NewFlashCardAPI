using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface IFolderRepository
{
    Task<List<Folder>> GetByOwnerIdAsync(string ownerId);
    Task<Folder?> GetByIdAsync(string id);
    Task<Folder> AddAsync(Folder folder);
    Task<Folder> UpdateAsync(Folder folder);
    Task<bool> DeleteAsync(string id);
}
