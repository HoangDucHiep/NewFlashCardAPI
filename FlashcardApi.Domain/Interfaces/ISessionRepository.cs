using FlashcardApi.Domain.Entities;

namespace FlashcardApi.Domain.Interfaces;

public interface ISessionRepository
{
    Task<Session> CreateAsync(Session session);
    Task<Session> UpdateAsync(Session session);
    Task DeleteAsync(string id);
    Task<List<Session>> GetByDeskIdAsync(string deskId);
}
