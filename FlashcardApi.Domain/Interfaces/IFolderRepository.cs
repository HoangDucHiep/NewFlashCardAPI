using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface IFolderRepository
{
    Task<List<Folder>> GetByOwnerIdAsync(string ownerId);
    Task<Folder> GetByIdAsync(string id);
    Task AddAsync(Folder folder);
    Task UpdateAsync(Folder folder);
    Task DeleteAsync(string id);
}
