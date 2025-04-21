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

    public async Task<Desk?> GetByIdAsync(string id)
    {
        return await _context.Desks
            .Include(d => d.Cards)
            .Include(d => d.Sessions)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Desk> AddAsync(Desk desk)
    {
        await _context.Desks.AddAsync(desk);
        await _context.SaveChangesAsync();
        return desk;
    }

    public async Task<Desk> UpdateAsync(Desk desk)
    {
        _context.Desks.Update(desk);
        await _context.SaveChangesAsync();
        return desk;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var desk = await GetByIdAsync(id);
        if (desk == null) return false;

        _context.Desks.Remove(desk); // Cards, Reviews, Sessions sẽ tự động xóa do cascade
        await _context.SaveChangesAsync();
        return true;
    }
}