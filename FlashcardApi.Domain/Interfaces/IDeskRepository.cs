using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface IDeskRepository
{
    Task<List<Desk>> GetByOwnerIdAsync(string ownerId);
    Task<List<Desk>> GetPublicDesksAsync();
    Task<Desk?> GetByIdAsync(string id);
    Task<Desk> AddAsync(Desk desk);
    Task<Desk> UpdateAsync(Desk desk);
    Task<bool> DeleteAsync(string id);
}
