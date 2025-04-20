using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface IDeskRepository
{
    Task<List<Desk>> GetByOwnerIdAsync(string ownerId);
    Task<List<Desk>> GetPublicDesksAsync();
    Task<Desk?> GetByIdAsync(string id);
    Task AddAsync(Desk desk);
    Task UpdateAsync(Desk desk);
    Task DeleteAsync(string id);
}
