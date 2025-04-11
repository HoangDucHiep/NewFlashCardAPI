using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories;

public class FolderRepository : IFolderRepository
{
    private readonly AppDbContext _context;

    public FolderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Folder>> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Folders.Where(f => f.OwnerId == ownerId).ToListAsync();
    }

    public async Task<Folder> GetByIdAsync(string id)
    {
        return await _context.Folders.FindAsync(id);
    }

    public async Task AddAsync(Folder folder)
    {
        await _context.Folders.AddAsync(folder);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Folder folder)
    {
        _context.Folders.Update(folder);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var folder = await GetByIdAsync(id);
        if (folder != null)
        {
            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();
        }
    }
}
