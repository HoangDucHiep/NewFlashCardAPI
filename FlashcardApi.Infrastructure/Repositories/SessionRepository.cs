using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _context;

    public SessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Session> CreateAsync(Session session)
    {
        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session> UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return false;

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Session>> GetByDeskIdAsync(string deskId)
    {
        return await _context.Sessions.Where(s => s.DeskId == deskId).ToListAsync();
    }

    public async Task<bool> DeleteAllByDeskIdAsync(string deskId)
    {
        var sessions = await _context.Sessions.Where(s => s.DeskId == deskId).ToListAsync();
        if (!sessions.Any()) return false;

        _context.Sessions.RemoveRange(sessions);
        await _context.SaveChangesAsync();
        return true;
    }
}