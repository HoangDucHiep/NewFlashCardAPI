using FlashcardApi.Application.Session.Dtos;

namespace FlashcardApi.Application.Interfaces
{
    public interface ISessionService
    {
        Task<SessionDto> CreateSessionAsync(SessionDto sessionDto);
        Task<SessionDto> UpdateSessionAsync(string id, SessionDto sessionDto);
        Task<bool> DeleteSessionAsync(string id);
        Task<bool> DeleteAllSessionsByDeskIdAsync(string deskId);
        Task<List<SessionDto>> GetSessionsByDeskIdAsync(string deskId);
    }
}
