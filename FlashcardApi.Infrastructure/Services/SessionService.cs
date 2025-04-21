using FlashcardApi.Application.Interfaces;
using FlashcardApi.Application.Session.Dtos;
using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;

namespace FlashcardApi.Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;

    public SessionService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<SessionDto> CreateSessionAsync(SessionDto sessionDto)
    {
        var session = new Session
        {
            DeskId = sessionDto.DeskId,
            StartTime = sessionDto.StartTime,
            EndTime = sessionDto.EndTime,
            CardsStudied = sessionDto.CardsStudied,
            Performance = sessionDto.Performance
        };
        var createdSession = await _sessionRepository.CreateAsync(session);
        return MapToDto(createdSession);
    }

    public async Task<SessionDto> UpdateSessionAsync(string id, SessionDto sessionDto)
    {
        var session = await _sessionRepository.GetByDeskIdAsync(sessionDto.DeskId)
            .ContinueWith(t => t.Result.FirstOrDefault(s => s.Id == id));
        if (session == null) throw new Exception("Session not found");

        session.StartTime = sessionDto.StartTime;
        session.EndTime = sessionDto.EndTime;
        session.CardsStudied = sessionDto.CardsStudied;
        session.Performance = sessionDto.Performance;
        await _sessionRepository.UpdateAsync(session);

        return MapToDto(session);
    }

    public async Task<bool> DeleteSessionAsync(string id)
    {
        return await _sessionRepository.DeleteAsync(id);
    }

    public async Task<List<SessionDto>> GetSessionsByDeskIdAsync(string deskId)
    {
        var sessions = await _sessionRepository.GetByDeskIdAsync(deskId);
        return sessions.Select(MapToDto).ToList();
    }

    private SessionDto MapToDto(Session session)
    {
        return new SessionDto
        {
            Id = session.Id,
            DeskId = session.DeskId,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            CardsStudied = session.CardsStudied,
            Performance = session.Performance
        };
    }

    public async Task<bool> DeleteAllSessionsByDeskIdAsync(string deskId)
    {
        return await _sessionRepository.DeleteAllByDeskIdAsync(deskId);
    }
}