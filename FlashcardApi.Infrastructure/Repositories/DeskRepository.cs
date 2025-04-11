using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories;

public class DeskRepository : IDeskRepository
{
    private readonly AppDbContext _context;

    public DeskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Desk>> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Desks.Where(d => d.OwnerId == ownerId).ToListAsync();
    }

    public async Task<List<Desk>> GetPublicDesksAsync()
    {
        return await _context.Desks.Where(d => d.IsPublic).ToListAsync();
    }

    public async Task<Desk> GetByIdAsync(string id)
    {
        return await _context.Desks.FindAsync(id);
    }

    public async Task AddAsync(Desk desk)
    {
        await _context.Desks.AddAsync(desk);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Desk desk)
    {
        _context.Desks.Update(desk);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var desk = await GetByIdAsync(id);
        if (desk != null)
        {
            _context.Desks.Remove(desk);
            await _context.SaveChangesAsync();
        }
    }
}
