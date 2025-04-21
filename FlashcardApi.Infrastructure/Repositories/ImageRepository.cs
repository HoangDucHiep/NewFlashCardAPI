using FlashcardApi.Domain.Entities;
using FlashcardApi.Domain.Interfaces;
using FlashcardApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApi.Infrastructure.Repositories;

public class ImageRepository : IImageRepository
{
    private readonly AppDbContext _context;

    public ImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Image> AddAsync(Image image)
    {
        await _context.Images.AddAsync(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var image = await _context.Images.FindAsync(id);
        if (image == null) return false;

        _context.Images.Remove(image);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Image?> GetByFileNameAsync(string fileName)
    {
        return await _context.Images.FirstOrDefaultAsync(i => i.Url.EndsWith(fileName));
    }

    public async Task<List<Image>> GetAllAsync()
    {
        return await _context.Images.ToListAsync();
    }
}