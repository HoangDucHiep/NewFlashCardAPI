using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories;

public class FolderRepository : IFolderRepository
{
    private readonly AppDbContext _context;
    private readonly IDeskRepository _deskRepository;

    public FolderRepository(AppDbContext context, IDeskRepository deskRepository)
    {
        _context = context;
        _deskRepository = deskRepository;
    }

    public async Task<List<Folder>> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Folders.Where(f => f.OwnerId == ownerId).ToListAsync();
    }

    public async Task<Folder?> GetByIdAsync(string id)
    {
        return await _context.Folders
            .Include(f => f.SubFolders)
            .Include(f => f.Desks)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<Folder> AddAsync(Folder folder)
    {
        await _context.Folders.AddAsync(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    public async Task<Folder> UpdateAsync(Folder folder)
    {
        _context.Folders.Update(folder);
        await _context.SaveChangesAsync();
        return folder;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var folder = await GetByIdAsync(id);
        if (folder == null) return false;

        // Xóa đệ quy subfolders
        foreach (var subFolder in folder.SubFolders.ToList())
        {
            await DeleteAsync(subFolder.Id);
        }

        // Xóa desks
        foreach (var desk in folder.Desks.ToList())
        {
            await _deskRepository.DeleteAsync(desk.Id);
        }

        _context.Folders.Remove(folder);
        await _context.SaveChangesAsync();
        return true;
    }
}